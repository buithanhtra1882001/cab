using Microsoft.AspNetCore.Identity;
using System;

namespace WCABNetwork.Cab.IdentityService.Models.Entities
{
    public class Account : IdentityUser<int>
    {
        public override int Id { get; set; }
        public string Uuid { get; set; } = Guid.NewGuid().ToString();
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsSoftDeleted { get; set; }
    }
}
