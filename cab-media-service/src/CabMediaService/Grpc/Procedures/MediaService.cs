using System;
using CabMediaService.Grpc.Protos.MediaServer;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using Grpc.Core;

namespace CabMediaService.Grpc.Procedures
{
	public class MediaService : MediaProtoService.MediaProtoServiceBase
    {
		private readonly IMediaImageRepository _mediaImageRepository;
        public MediaService(IMediaImageRepository mediaImageRepository)
		{
            _mediaImageRepository = mediaImageRepository;
        }

        public override async Task<FileUploadStatus> UploadFile(FileRequest fileRequest, ServerCallContext context)
        {
            Guid uuid = Guid.Parse(fileRequest.UserId);
            var resultUploadFile = await _mediaImageRepository.UploadFileAsync(uuid, fileRequest.Type, null);
            return await Task.FromResult(new FileUploadStatus
            {
                PercentageComplete = 100,
                IsComplete = true
            });
        }

    }
}

