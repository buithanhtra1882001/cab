using CabMediaService.Constants;
using CabMediaService.DomainCommands.Commands;
using CabMediaService.Infrastructures.Common;
using CabMediaService.Infrastructures.DbContexts;
using CabMediaService.Infrastructures.Repositories.Base;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.Models.Dtos;
using CabMediaService.Models.Entities;
using Cassandra;
using MediatR;

namespace CabMediaService.Infrastructures.Repositories
{
    public class MediaImageRepository : BaseRepository<MediaImage>, IMediaImageRepository
    {
        private readonly Cassandra.ISession _session;
        private readonly IMediator _mediator;
        private readonly AwsS3IntegrationHelper _awsS3IntegrationHelper;

        private readonly string _bucketName;

        //public MediaImageRepository(CassandraDbContext context
        //    , IMediator mediator
        //    , MinioClient minioClient
        //    , IConfiguration configuration)
        //    : base(context)
        //{
        //    _bucketName = configuration.GetValue<string>("MinIOSettings:BucketName");
        //    _bucketEndpoint = configuration.GetValue<string>("MinIOSettings:Endpoint");
        //    _minioClient = minioClient;
        //    _mediator = mediator;
        //    _session = context._session;
        //}

        public MediaImageRepository(ScyllaDbContext context
            , IMediator mediator
            , AwsS3IntegrationHelper awsS3IntegrationHelper
            , IConfiguration configuration)
            : base(context)
        {
            _bucketName = configuration.GetValue<string>("AWS:S3:Bucket");
            _mediator = mediator;
            _session = context._session;
        }

        public async Task CreateManyAsync(IEnumerable<MediaImage> entities)
        {
            if (entities is null || entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var preparedStatement = await _session.PrepareAsync("INSERT INTO media_images (id, file_name, file_path, size, last_used_at, created_at, created_by) VALUES (?, ?, ?, ?, ?, ?, ?)");
            var batchStatement = new BatchStatement();

            foreach (var item in entities)
            {
                batchStatement.Add(preparedStatement.Bind(item.Id, item.FileName, item.FilePath, item.Size, item.LastUsedAt, item.CreatedAt, item.CreatedBy));
            }

            await _session.ExecuteAsync(batchStatement);
        }

        public async Task<IEnumerable<UploadImageResponse>> UploadFileAsync(Guid uuid, string type, IFormFileCollection files)
        {
            if (files is null || files.Count == 0)
            {
                return null;
            }

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

        //private async Task<IEnumerable<UploadImageResponse>> UploadImagesToMinioAsync(Guid uuid, string type, string folderName, IFormFileCollection files)
        //{
        //    var list = new List<UploadImageResponse>();

        //    foreach (var file in files)
        //    {
        //        var size = Math.Round((double)file.Length / 1024, 2);

        //        if (file.Length == 0)
        //        {
        //            list.Add(new UploadImageResponse
        //            {
        //                FileName = file.FileName,
        //                Size = size,
        //                Status = UploadStatusConstant.FAILED,
        //                Error = "File is empty",
        //                CreatedAt = DateTime.UtcNow
        //            });
        //            continue;
        //        }

        //        var extension = Path.GetExtension(file.FileName);
        //        var newFileName = $"{DateTime.UtcNow.ToString("yyyy.MM.dd.HH.mm.ss.FFF")}{extension}";
        //        var contentTypeProvider = new FileExtensionContentTypeProvider();

        //        if (contentTypeProvider.TryGetContentType(file.FileName, out var contentType))
        //        {
        //            try
        //            {
        //                var filePath = $"media/{folderName}/{type}/{newFileName}";
        //                var metadata = new Dictionary<string, string>
        //                {
        //                    { "userId", uuid.ToString() },
        //                    { "fileName", file.FileName }
        //                };

        //                using (var fileAsStreamData = file.OpenReadStream())
        //                {
        //                    // Upload a file to bucket.
        //                    await _minioClient.PutObjectAsync(_bucketName, filePath, fileAsStreamData, fileAsStreamData.Length, contentType, metadata);
        //                }

        //                list.Add(new UploadImageResponse
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FileName = file.FileName,
        //                    FilePath = filePath,
        //                    Size = size,
        //                    Url = $"https://{_bucketEndpoint}/{_bucketName}/{filePath}",
        //                    Status = UploadStatusConstant.SUCCESS,
        //                    CreatedAt = DateTime.UtcNow
        //                });
        //            }
        //            catch (MinioException ex)
        //            {
        //                list.Add(new UploadImageResponse
        //                {
        //                    FileName = file.FileName,
        //                    Size = size,
        //                    Status = UploadStatusConstant.FAILED,
        //                    Error = ex.Message,
        //                    CreatedAt = DateTime.UtcNow
        //                });
        //            }
        //        }
        //    }

        //    return list;
        //}
    }
}