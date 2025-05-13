using CabUserService.Infrastructures.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class UserCreateUpdateRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Fullname { get; set; }

        public string Dob { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string IdentityCardNumber { get; set; }
        //[SexValidation]
        public string Sex { get; set; }

        public string Description { get; set; }

        public bool IsUpdateProfile { get; set; }

        [Required]
        public int SequenceId { get; set; }

        // Update details KienVT

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

        public List<Guid>? CategoryFavorites { get; set; }

        public List<string> Schools { get; set; }

        public List<string> Companys { get; set; }

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

    }
}