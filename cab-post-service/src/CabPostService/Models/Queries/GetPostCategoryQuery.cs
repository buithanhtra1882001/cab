using System;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries
{
    public class GetPostCategoryQuery : IQuery<IList<UserPostCategoryResponse>>
    {
    }
}

