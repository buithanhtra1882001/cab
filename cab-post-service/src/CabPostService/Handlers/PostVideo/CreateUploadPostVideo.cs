using CabPostService.Clients.Interfaces;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;

namespace CabPostService.Handlers.PostVideo;

public partial class PostVideoHandler :
    ICommandHandler<UploadFileCommand, BaseResponse>
{
    private const string FOLDER_MINIO = "PostVideo";

    public async Task<BaseResponse> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var mediaClient = _seviceProvider.GetRequiredService<IMediaClient>();
            var uploadedVideo = await mediaClient.UploadVideoAsync(BearerToken, FOLDER_MINIO, request.Files);
            return await CreatePostVideo(uploadedVideo.ToList(), request.UserId);
        }
        catch (Exception e)
        {
            var result = new BaseResponse
            {
                IsSuccess = false,
                Message = e.Message
            };
            return result;
        }
    }

    private async Task<BaseResponse> CreatePostVideo(List<VideoUploadResponse> requests, Guid userId)
    {
        var result = new BaseResponse();

        try
        {
            if (!requests.Any())
            {
                result.IsSuccess = false;
                result.Message = "Upload file is error, please try again";
                return result;
            }

            var postVideoRepository = _seviceProvider.GetRequiredService<IPostVideoRepository>();
            var idList = new List<string>();

            foreach (var request in requests)
            {
                var id = Guid.NewGuid().ToString();
                var model = new Models.Entities.PostVideo
                {
                    Id = id,
                    UserId = userId,
                    MediaVideoId = request.Id.ToString(),
                    VideoUrl = request.Url,
                    Description = string.Empty,
                    LengthVideo = request.Duration,
                    AvgViewLength = 0,
                    IsViolence = false,
                    ViewCount = 0
                };
                await postVideoRepository.CreateAsync(model);
                idList.Add(id);
            }

            result.IsSuccess = true;
            result.Message = "Upload Success";
            result.Content = idList;

            return result;
        }
        catch (Exception e)
        {
            result.IsSuccess = false;
            result.Message = e.Message;
            return result;
        }
    }
}