using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Helpers;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetHashtagByTotalRecordQuery, IList<HashtagResponse>>
    {

        public async Task<IList<HashtagResponse>> Handle(
            GetHashtagByTotalRecordQuery request,
            CancellationToken cancellationToken)
        {

            List<HashtagResponse> result = new List<HashtagResponse>();
            var postHashtagRepository = _seviceProvider.GetRequiredService<IPostHashtagRepository>();
            if (request.Type == "GET")
            {   
                var lstHashTag = await postHashtagRepository.GetDataByLimit(request.TotalRecord);
                result = _mapper.Map<List<HashtagResponse>>(lstHashTag);
            }
            else if(request.Type == "SEARCH")
            {
                if(!string.IsNullOrEmpty(request.Keyword))
                {
                    var lstHashTag = await postHashtagRepository.SearchDataBySlug(Utils.ToSlug(request.Keyword));
                    result = _mapper.Map<List<HashtagResponse>>(lstHashTag);
                }else
                {
                    var lstHashTag = await postHashtagRepository.GetDataByLimit(request.TotalRecord);
                    result = _mapper.Map<List<HashtagResponse>>(lstHashTag);
                }
            }

            return result;
        }
    }
}