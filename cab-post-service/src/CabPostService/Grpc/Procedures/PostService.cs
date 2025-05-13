using AutoMapper;
using CabPostService.Constants;
using CabPostService.Grpc.Protos.PostServer;
using CabPostService.Infrastructures.Repositories.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CabPostService.Grpc.Procedures
{
    public class PostService : PostProtoService.PostProtoServiceBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }
        public override async Task<PostResponse> GetPostAsync(PostResquest resquest, ServerCallContext context)
        {
            var postId = Guid.Parse(resquest.PostId);
            var post = await _postRepository.GetByIdAsync(postId);
            var postResponse = _mapper.Map<PostResponse>(post);

            return postResponse;
        }
        public override async Task<WeightConstantsResponse> GetWeightConstants(Empty empty, ServerCallContext context)
        {
            return await Task.FromResult(new WeightConstantsResponse
            {
                CategoryScoreWeight = WeightConstants.CategoryScoreWeight,
                UpVoteWeight = WeightConstants.UpVoteWeight,
                DownVoteWeight = WeightConstants.DownVoteWeight,
                TotalCommentsWeight = WeightConstants.TotalCommentsWeight,
                TotalViewsWeight = WeightConstants.TotalViewsWeight,
                AdminBoostWeight = WeightConstants.AdminBoostWeight,

            });
        }
    }
}
