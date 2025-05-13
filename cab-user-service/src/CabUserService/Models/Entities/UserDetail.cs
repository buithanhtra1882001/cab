using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class UserDetail : BaseEntity
    {
        public Guid UserDetailId { get; set; }
        
        public Guid UserId { get; set; }
        
        public string Dob { get; set; }
        
        public string Phone { get; set; }
        
        public string City { get; set; }
        
        public string Avatar { get; set; }
        
        public string IdentityCardNumber { get; set; }
        
        public string Sex { get; set; }
        
        public string Description { get; set; }
                
        public string Follower { get; set; }
        
        public bool IsFollower { get; set; }
        
        public string Following { get; set; }
        
        public bool IsUpdateProfile { get; set; }
        
        public string CoverImage { get; set; }

        /// <summary>
        ///null: unknown
        ///0: male
        ///1: female
        ///2: lgbt
        /// </summary>
        public int? SexualOrientation { get; set; }

        /// <summary>
        ///null: unknown
        ///0: marry
        ///1: not marry
        /// </summary>
        public int? Marry { get; set; }

        public string School { get; set; }

        public string Company { get; set; }

        public string HomeLand { get; set; }

        public bool IsShowSexualOrientation { get; set; }

        public bool IsShowMarry { get; set; }

        public bool IsShowSchool { get; set; }

        public bool IsShowCompany { get; set; }

        public bool IsShowHomeLand { get; set; }

        public string IsShowDob { get; set; }

        public bool IsShowEmail { get; set; }

        public bool IsShowPhone { get; set; }

        public bool IsShowCity { get; set; }

        public bool IsShowDescription { get; set; }

        public User User { get; set; }
    }
}
