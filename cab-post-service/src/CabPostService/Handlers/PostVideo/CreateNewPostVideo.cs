using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;

namespace CabPostService.Handlers.PostVideo
{
    public partial class PostVideoHandler :
        ICommandHandler<CreatePostVideoCommand, string>
    {
        public async Task<string> Handle(
            CreatePostVideoCommand request,
            CancellationToken cancellationToken)
        {
            var postVideo = _mapper.Map<Models.Entities.PostVideo>(request);
            postVideo.Id = Guid.NewGuid().ToString();
            postVideo.AvgViewLength = 0;
            postVideo.ViewCount = 0;
            
            var postVideoRepository = _seviceProvider.GetRequiredService<IPostVideoRepository>();
            await postVideoRepository.CreateAsync(postVideo);
            return postVideo.Id;
        }
    }
}