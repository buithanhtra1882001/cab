using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;

namespace CabPostService.Handlers.PostVideo
{
    public partial class PostVideoHandler :
        ICommandHandler<UpdateAVTPCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(
            UpdateAVTPCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var postVideoRepository = _seviceProvider.GetRequiredService<IPostVideoRepository>();
                var result = new BaseResponse();
                result.IsSuccess = true;
                var postVideo = await postVideoRepository.GetByIdAsync(request.PostVideoId);
                if (postVideo is null)
                {
                    _logger.LogWarning($"UpdateAVTPCommand, errors: not found post by PostId={request.PostVideoId}");
                    result.IsSuccess = false;
                    result.Message = $"Not found postVideoId={request.PostVideoId}";
                    return result;
                }

                var videoLength = postVideo.LengthVideo;
                var viewCount = postVideo.ViewCount;
                var timeVideoView = request.TimeVideoView;

                if (timeVideoView > videoLength)
                {
                    _logger.LogWarning($"UpdateAVTPCommand {request.PostVideoId}, errors: timeVideoView > videoLength");
                    result.IsSuccess = false;
                    result.Message = $"Time view video is invalid < {videoLength}";
                    return result;
                }

                var aVTP = (timeVideoView / videoLength) / (viewCount + 1);
                string responseUpdate = await postVideoRepository.UpdateAVTP(request.PostVideoId, aVTP, DateTime.UtcNow);

                if (!string.IsNullOrEmpty(responseUpdate))
                {
                    result.IsSuccess = false;
                    result.Message = responseUpdate;
                }

                return result;
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
    }
}