using CabPostService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Entities
{
    // User of CassandraDbContext or ScyllaDbContext
    public class User : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
    }
}
