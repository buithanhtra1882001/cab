using System;
using WCABNetwork.Cab.IdentityService.Models.Entities.Base;

namespace WCABNetwork.Cab.IdentityService.Models.Entities
{
    public class UserRefreshToken : BaseEntity
    {
        public int SubjectId { get; set; }
        public string TokenValue { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime Expiration { get; set; }
    }
}