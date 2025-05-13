using CabMediaService.Services.Base;
using CabMediaService.Services.Interfaces;
using Dropbox.Api.Files;
using Dropbox.Api;
using CabMediaService.Integration.Dropbox;
using Dropbox.Api.Sharing;
using CabMediaService.Infrastructures.Exceptions;
using CabMediaService.Infrastructures.Extensions;
using Microsoft.AspNetCore.Http;
using CabMediaService.Constants;
using CabMediaService.Models.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using CabMediaService.DomainCommands.Commands;
using CabMediaService.Models.Entities;
using MediatR;
using Xabe.FFmpeg;

namespace CabMediaService.Services
{
    public class DropBoxMediaService : BaseService<DropBoxMediaService>, IDropBoxMediaService
    {
        private readonly IMediator _mediator;
        private readonly IDropboxService _dropbox;
        private readonly DropboxClient _dropboxClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        public DropBoxMediaService(IDropboxService dropbox, IHttpClientFactory httpClientFactory, IConfiguration configuration, IMediator mediator, ILogger<DropBoxMediaService> logger) : base(logger)
        {
            _dropbox = dropbox;
            _httpClientFactory = httpClientFactory; 
            _baseUrl = configuration.GetValue<string>("GlobalService:BaseAddress");
            _dropboxClient = new DropboxClient(_dropbox.GetToken().Result);
            _mediator = mediator;
        }

        public async Task<List<UploadImageResponse>> UploadImageAsync(IFormFileCollection files, Guid userId)
        {
            try
            {
                if (files is null || files.Count == 0)
                    return null;

                ValidateImage(files);
                var response = await UploadImageAsync(files, _dropbox.GetUploadImagePath());
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
                   CreatedBy = userId
               }).ToList();

               if (uploadedImages != null && uploadedImages.Count > 0)
               {
                    await _mediator.Publish(new UploadImagesCommand
                    {
                        UserId = userId,
                        MediaImages = uploadedImages
                    });
               }
               return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UploadVideoResponse>> UploadVideoAsync(Guid userId, IFormFileCollection files, string bearerToken)
        {
            try
            {
                if (files is null || files.Count == 0)
                    return null;

                long maxFileSize = 0;
                var userType = await GetUserTypeByUserIdFromUserServiceAsync(userId, bearerToken);

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

                var response = await UploadVideoAsync(files, maxFileSize,_dropbox.GetUploadVideoPath());
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
                       CreatedBy = userId
                   }).ToList();

                if (uploadedImages != null && uploadedImages.Count > 0)
                {
                    await _mediator.Publish(new UploadImagesCommand
                    {
                        UserId = userId,
                        MediaImages = uploadedImages
                    });
                }
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<UploadImageResponse>> UploadImageAsync(IFormFileCollection files, string folderPath)
        {
            try
            {
                var list = new List<UploadImageResponse>();
                await EnsureFolderExistsAsync(folderPath);
                foreach (var file in files)
                {
                    var size = Math.Round((double)file.Length / 1024, 2);
                    var fileResponse = new UploadImageResponse
                    {
                        FileName = file.FileName,
                        Size = size,
                        ContentType = file.ContentType,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (file.Length == 0)
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "File is empty";
                        list.Add(fileResponse);
                        continue;
                    }

                    var extension = Path.GetExtension(file.FileName);
                    var fileName = Path.GetRandomFileName() + extension;
                    var filePath = folderPath + "/" + fileName;
                    Stream fileStream = file.OpenReadStream();
                    var upload = await _dropboxClient.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: fileStream);
                    if (upload != null)
                    {
                        SharedLinkMetadata shared = await _dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(upload.PathDisplay);
                        string url = shared != null ? shared.Url : string.Empty;
                        url = url.Replace("&dl=0", "&raw=1");

                        fileResponse.Id = Guid.NewGuid();
                        fileResponse.FilePath = filePath;
                        fileResponse.Url = url;
                        fileResponse.Status = UploadStatusConstant.SUCCESS;
                    }
                    else
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "Upload failed!";
                    }
                    list.Add(fileResponse);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<UploadVideoResponse>> UploadVideoAsync(IFormFileCollection files, long maxFileSize, string folderPath)
        {
            try
            {
                var list = new List<UploadVideoResponse>();
                await EnsureFolderExistsAsync(folderPath);
                foreach (var file in files)
                {
                    var size = Math.Round((double)file.Length / 1024, 2);
                    var fileResponse = new UploadVideoResponse
                    {
                        FileName = file.FileName,
                        Size = size,
                        ContentType = file.ContentType,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (file.Length == 0)
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "File is empty";
                        list.Add(fileResponse);
                        continue;
                    }

                    if (file.Length > maxFileSize)
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "Video exceeds the allowed size";
                        list.Add(fileResponse);
                        continue;
                    }

                    var tempFilePath = Path.GetTempFileName();
                    using (var fileStreamTemp = new FileStream(tempFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStreamTemp);
                    }

                    var mediaInfo = FFmpeg.GetMediaInfo(tempFilePath);
                    var durationTimeSpan = mediaInfo.Result.Duration;
                    fileResponse.Duration = durationTimeSpan.TotalSeconds;

                    if (File.Exists(tempFilePath))
                        File.Delete(tempFilePath);

                    var extension = Path.GetExtension(file.FileName);
                    var fileName = Path.GetRandomFileName() + extension;
                    var filePath = folderPath + "/" + fileName;
                    Stream fileStream = file.OpenReadStream();
                    var upload = await _dropboxClient.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: fileStream);
                    if (upload != null)
                    {
                        SharedLinkMetadata shared = await _dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(upload.PathDisplay);
                        string url = shared != null ? shared.Url : string.Empty;
                        url = url.Replace("&dl=0", "&raw=1");

                        fileResponse.Id = Guid.NewGuid();
                        fileResponse.FilePath = filePath;
                        fileResponse.Url = url;
                        fileResponse.Status = UploadStatusConstant.SUCCESS;
                    }
                    else
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "Upload failed!";
                    }
                    list.Add(fileResponse);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task EnsureFolderExistsAsync(string folderPath)
        {
            try
            {
                await _dropboxClient.Files.ListFolderAsync(folderPath);
            }
            catch (ApiException<ListFolderError>)
            {
                await _dropboxClient.Files.CreateFolderV2Async(folderPath);
            }
        }

        private void ValidateImage(IFormFileCollection files)
        {
            var invalidImage = files.FirstOrDefault(x => !x.IsValidImage());
            if (invalidImage != null)
                throw new ApiValidationException($"File '{invalidImage.FileName}' is invalid.");
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
    }
}
