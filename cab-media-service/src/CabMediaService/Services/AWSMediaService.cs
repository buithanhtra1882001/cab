using AutoMapper;
using CabMediaService.Constants;
using CabMediaService.DomainCommands.Commands;
using CabMediaService.Infrastructures.Common;
using CabMediaService.Infrastructures.Exceptions;
using CabMediaService.Infrastructures.Extensions;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.Models.Dtos;
using CabMediaService.Models.Entities;
using CabMediaService.Services.Base;
using CabMediaService.Services.Interfaces;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CabMediaService.Services
{
    public class AWSMediaService : BaseService<AWSMediaService>, IAWSMediaService
    {
        private readonly IMediator _mediator;
        private readonly IMediaImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly AwsS3IntegrationHelper _awsS3IntegrationHelper;
        private readonly string _bucketName;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public AWSMediaService(ILogger<AWSMediaService> logger
            , IMediator mediator
            , IConfiguration configuration
            , IMediaImageRepository imageRepository
            , IMapper mapper
            , AwsS3IntegrationHelper awsS3IntegrationHelper
            ,
            IHttpClientFactory httpClientFactory) : base(logger)
        {
            _bucketName = configuration.GetValue<string>("AWS:S3:Bucket");
            _baseUrl = configuration.GetValue<string>("GlobalService:BaseAddress");
            _mediator = mediator;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _awsS3IntegrationHelper = awsS3IntegrationHelper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MediaImageResponse> GetAsync(Guid id)
        {
            var entity = await _imageRepository.GetOneAsync(x => x.Id == id);

            if (entity is null)
            {
                _logger.LogError($"Cannot find image entity having id: {id}");
                throw new ApiValidationException("The image is not found");
            }

            return _mapper.Map<MediaImageResponse>(entity);
        }

        public async Task<IEnumerable<MediaImageResponse>> GetListAsync(IEnumerable<Guid> ids)
        {
            var entities = await _imageRepository.GetListAsync(x => ids.Contains(x.Id));
            return entities.Select(x => _mapper.Map<MediaImageResponse>(x));
        }

        public async Task<IEnumerable<UploadImageResponse>> UploadAsync(Guid uuid, string type, IFormFileCollection files)
        {
            if (files is null || files.Count == 0)
            {
                return null;
            }

            ValidateImage(files);

            var uploadPath = "file-uploads";
            var response = await _awsS3IntegrationHelper.UploadImagesAsync(files, uuid, type, "images", _bucketName, uploadPath);
            var uploadedImages = response
                .Where(item => item.Status == UploadStatusConstant.SUCCESS)
                .Select(item => new MediaImage
                {
                    Id = item.Id,
                    CreatedAt = item.CreatedAt.Value,
                    FileName = item.FileName,
                    FilePath = item.Url,
                    Size = item.Size,
                    LastUsedAt = DateTime.UtcNow,
                    CreatedBy = uuid
                }).ToList();

            if (uploadedImages != null && uploadedImages.Count > 0)
            {
                await _mediator.Publish(new UploadImagesCommand
                {
                    UserId = uuid,
                    MediaImages = uploadedImages
                });
            }

            return response;
        }

        public async Task<IEnumerable<UploadVideoResponse>> UploadVideosAsync(Guid uuid, string type, IFormFileCollection files, string bearerToken)
        {
            if (files is null || files.Count == 0)
            {
                return null;
            }
            long maxFileSize;
            var userType = await GetUserTypeByUserIdFromUserServiceAsync(uuid, bearerToken);

            switch (userType)
            {
                case UserType.NORMAL:
                    maxFileSize = 15 * 1024 * 1024; // 15MB
                    break;
                case UserType.CONTENT_CREATOR:
                    maxFileSize = 100 * 1024 * 1024; // 100MB
                    break;
                default:
                    throw new Exception("Invaild UserType");
            }

            var uploadPath = "file-uploads";
            var response = await _awsS3IntegrationHelper.UploadVideoFileAsync(files, uuid, type, "videos", _bucketName, uploadPath, maxFileSize);

            var uploadedImages = response
                .Where(item => item.Status == UploadStatusConstant.SUCCESS)
                .Select(item => new MediaImage
                {
                    Id = item.Id,
                    CreatedAt = item.CreatedAt.Value,
                    FileName = item.FileName,
                    FilePath = item.Url,
                    Size = item.Size,
                    LastUsedAt = DateTime.UtcNow,
                    CreatedBy = uuid
                }).ToList();

            if (uploadedImages != null && uploadedImages.Count > 0)
            {
                await _mediator.Publish(new UploadImagesCommand
                {
                    UserId = uuid,
                    MediaImages = uploadedImages
                });
            }

            return response;
        }

        public async Task UpdateLastUsedAsync(IEnumerable<Guid> guids, DateTime lastUsedAt)
        {
            var entities = (await _imageRepository.GetListAsync(x => guids.Contains(x.Id))).ToList();

            if (entities is null || entities.Count == 0)
            {
                throw new ApiValidationException("The images are not found");
            }

            var taskList = entities.Select(x => _imageRepository.UpdateAsync(x,
                p => p.Id == x.Id && p.CreatedAt == x.CreatedAt,
                s => new MediaImage
                {
                    LastUsedAt = lastUsedAt
                }));

            await Task.WhenAll(taskList);
        }

        public async Task DeleteAsync(Guid uuid, Guid id)
        {
            var entity = await _imageRepository.GetOneAsync(x => x.Id == id);

            if (entity is null)
            {
                _logger.LogWarning($"Cannot delete the image {id}, errors: image is not found");
                throw new ApiValidationException("The image is not found");
            }

            await _awsS3IntegrationHelper.DeleteObjectAsync(_bucketName, entity.FilePath);
            await _mediator.Publish(new DeleteImagesCommand
            {
                UserId = uuid,
                Ids = new[] { id }
            });
        }

        public async Task DeleteManyAsync(Guid uuid, IEnumerable<Guid> ids)
        {
            var entities = await _imageRepository.GetListAsync(x => ids.Contains(x.Id));

            if (entities is null || entities.Count() != ids.Count())
            {
                _logger.LogWarning($"Cannot delete images, errors: image is not found");
                throw new ApiValidationException("The images are not found");
            }

            var urls = entities.Select(x => x.FilePath).ToArray();
            await _awsS3IntegrationHelper.DeleteFileByLinks(urls);
            await _mediator.Publish(new DeleteImagesCommand
            {
                UserId = uuid,
                Ids = ids
            });
        }

        public async Task<int> CleanAsync()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var entities = (await _imageRepository.GetListAsync(x => x.LastUsedAt <= last30Days)).ToList();

            if (entities is null || entities.Count == 0)
            {
                return 0;
            }

            var urls = entities.Select(x => x.FilePath).ToList();
            await _awsS3IntegrationHelper.DeleteFileByLinks(urls.ToArray());
            await _mediator.Publish(new DeleteImagesCommand
            {
                UserId = Guid.Empty,
                Ids = entities.Select(x => x.Id)
            });

            return entities.Count;
        }

        private async Task<UserType?> GetUserTypeByUserIdFromUserServiceAsync(Guid userId, string bearerToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await client.GetAsync($"{_baseUrl}/v1/user-service/me?auid={userId}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserServiceResponse>(responseContent);

                return result?.Data?.UserType ?? null;
            }
            return null;
        }

        private void ValidateImage(IFormFileCollection files)
        {
            var invalidImage = files.FirstOrDefault(x => !x.IsValidImage());
            if (invalidImage != null)
            {
                throw new ApiValidationException($"File '{invalidImage.FileName}' is invalid.");
            }
        }
    }
}