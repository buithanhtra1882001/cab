using CabUserService.Clients.Interfaces;
using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Extensions;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using MediatR;

namespace CabUserService.Services
{
    public class FileService : BaseService<FileService>, IFileService
    {
        private readonly IMediator _mediator;
        private readonly IMediaClient _mediaClient;

        public FileService(ILogger<FileService> logger
            , IMediator mediator
            , IMediaClient mediaClient)
        : base(logger)
        {
            _mediator = mediator;
            _mediaClient = mediaClient;
        }

        public async Task<IEnumerable<ImageUploadResponse>> UploadUserImagesAsync(string bearerToken, string type, Guid auid, IFormFileCollection files)
        {
            if (files.Any(f => f.Length == 0))
            {
                return null;
            }

            var uploadedImages = await _mediaClient.UploadImagesAsync(bearerToken, type, files);
            var createUserImageCommand = new CreateUserImageCommand
            {
                UserImages = uploadedImages
                    .Where(x => x.Error.IsNullOrEmpty())
                    .Select(item => new UserImage
                    {
                        CreatedAt = item.CreatedAt.Value,
                        Url = item.Url,
                        Size = item.Size,
                        UserId = auid
                    }).ToList()
            };

            await _mediator.Publish(createUserImageCommand);
            return uploadedImages;
        }

        public async Task<IEnumerable<ImageUploadResponse>> UploadAvatarUserImagesAsync(string bearerToken, string type, Guid auid, IFormFile file)
        {
            if (file is null)
                return null;

            var uploadedImages = await _mediaClient.UploadImageAsync(bearerToken, type, file);
            var createUserImageCommand = new CreateUserImageCommand
            {
                UserImages = uploadedImages
                    .Where(x => x.Error.IsNullOrEmpty())
                    .Select(item => new UserImage
                    {
                        CreatedAt = item.CreatedAt.Value,
                        Url = item.Url,
                        Size = item.Size,
                        UserId = auid
                    }).ToList()
            };

            await _mediator.Publish(createUserImageCommand);
            return uploadedImages;
        }

        public async Task<IEnumerable<ImageUploadResponse>> UploadBackgroundUserImagesAsync(string bearerToken, string type, Guid auid, IFormFile file)
        {
            if (file is null)
                return null;

            var uploadedImages = await _mediaClient.UploadImageAsync(bearerToken, type, file);
            //var createUserImageCommand = new CreateUserImageCommand
            //{
            //    UserImages = uploadedImages
            //        .Where(x => x.Error.IsNullOrEmpty())
            //        .Select(item => new UserImage
            //        {
            //            CreatedAt = item.CreatedAt.Value,
            //            Url = item.Url,
            //            Size = item.Size,
            //            UserId = auid
            //        }).ToList()
            //};

            //await _mediator.Publish(createUserImageCommand);
            return uploadedImages;
        }
    }
}