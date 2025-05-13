using CabUserService.Infrastructures.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class TotalFollowResponse : Response
    {
        public int TotalFollower { get; set; }
        public int TotalFollowing { get; set; }
    }
}