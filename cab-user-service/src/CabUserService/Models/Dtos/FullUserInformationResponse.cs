namespace CabUserService.Models.Dtos
{
    public class FullUserInformationResponse : PublicUserInformationResponse
    {
        public string Phone { get; set; }

        public string IdentityCardNumber { get; set; }

        public int? SexualOrientation { get; set; }

        public int? Marry { get; set; }

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
        public bool IsVerifyEmail { get; set; }
    }
}
