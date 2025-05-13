using CabPostService.Models.Dtos.PostVideo;

namespace CabPostService.Grpc.Procedures;

public interface IMediaService
{
    Task<UploadPostVideoResponse> UploadFileAsync(UploadPostVideoRequest request);
}