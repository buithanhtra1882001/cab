using CabMediaService.Constants;
using CabMediaService.Integration.Aws;
using CabMediaService.Models.Dtos;
using Microsoft.AspNetCore.StaticFiles;
using Xabe.FFmpeg;

namespace CabMediaService.Infrastructures.Common
{
    public class AwsS3IntegrationHelper
    {
        private readonly IS3Client _s3Client;

        public AwsS3IntegrationHelper(
            IS3Client s3Client
        )
        {
            _s3Client = s3Client;
        }

        public async Task<List<UploadImageResponse>> UploadImagesAsync(IFormFileCollection files, Guid uuid, string type, string folderName, string bucket, string objectKey)
        {
            var list = new List<UploadImageResponse>();
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
                try
                {

                    if (file.Length == 0)
                    {
                        fileResponse.Status = UploadStatusConstant.FAILED;
                        fileResponse.Error = "File is empty";

                        list.Add(fileResponse);
                        continue;
                    }

                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = $"{DateTime.UtcNow.ToString("yyyy.MM.dd.HH.mm.ss.FFF")}{extension}";
                    var contentTypeProvider = new FileExtensionContentTypeProvider();

                    if (contentTypeProvider.TryGetContentType(file.FileName, out var contentType))
                    {

                        var filePath = $"media/{folderName}/{type}/{newFileName}";
                        objectKey += filePath;

                        using var fileAsStreamData = file.OpenReadStream();
                        var s3Response = await _s3Client.PutObjectToS3Async(fileAsStreamData, bucket, objectKey, contentType);

                        if (s3Response != null && s3Response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            fileResponse.Id = Guid.NewGuid();
                            fileResponse.FilePath = filePath;
                            fileResponse.Url = _s3Client.GetAbsoluteLink(objectKey);
                            fileResponse.Status = UploadStatusConstant.SUCCESS;
                        }
                        else
                        {
                            fileResponse.Status = UploadStatusConstant.FAILED;
                            fileResponse.Error = s3Response.HttpStatusCode.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    fileResponse.Status = UploadStatusConstant.FAILED;
                    fileResponse.Error = ex.Message;
                }

                list.Add(fileResponse);
            }

            return list;
        }

        public async Task<List<UploadVideoResponse>> UploadVideoFileAsync(IFormFileCollection files, Guid uuid, string type, string folderName, string bucket, string objectKey, long maxFileSize)
        {
            var list = new List<UploadVideoResponse>();
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
                try
                {

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
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var mediaInfo = FFmpeg.GetMediaInfo(tempFilePath);
                    var durationTimeSpan = mediaInfo.Result.Duration;
                    fileResponse.Duration = durationTimeSpan.TotalSeconds;

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = $"{DateTime.UtcNow.ToString("yyyy.MM.dd.HH.mm.ss.FFF")}{extension}";
                    var contentTypeProvider = new FileExtensionContentTypeProvider();

                    if (contentTypeProvider.TryGetContentType(file.FileName, out var contentType))
                    {

                        var filePath = $"media/{folderName}/{type}/{newFileName}";
                        objectKey += filePath;

                        using var fileAsStreamData = file.OpenReadStream();
                        var s3Response = await _s3Client.PutObjectToS3Async(fileAsStreamData, bucket, objectKey, contentType);

                        if (s3Response != null && s3Response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            fileResponse.Id = Guid.NewGuid();
                            fileResponse.FilePath = filePath;
                            fileResponse.Url = _s3Client.GetAbsoluteLink(objectKey);
                            fileResponse.Status = UploadStatusConstant.SUCCESS;
                        }
                        else
                        {
                            fileResponse.Status = UploadStatusConstant.FAILED;
                            fileResponse.Error = s3Response.HttpStatusCode.ToString();
                        }
                    }

                }
                catch (Exception ex)
                {
                    fileResponse.Status = UploadStatusConstant.FAILED;
                    fileResponse.Error = ex.Message;
                }
                list.Add(fileResponse);
            }

            return list;
        }

        public async Task<long> GetSpaceUsageAsync(string bucket, string objectKey)
        {
            return await _s3Client.GetObjectSizeAsync(bucket, objectKey);
        }

        public async Task DeleteObjectAsync(string bucket, string objectKey)
        {
            await _s3Client.DeleteObjectsAsync(bucket, objectKey);
        }

        public async Task DeleteObjectsAsync(string bucket, IEnumerable<string> objectKey)
        {
            await _s3Client.DeleteObjectsAsync(bucket, objectKey);
        }

        public async Task DeleteFileByLinks(params string[] fileLink)
        {
            await _s3Client.DeleteObjectsAsync(fileLink);
        }
    }
}
