namespace CabPostService.Models.Dtos
{
    public class PublicUserInformationResponse
    {
        public Guid UserId { get; set; }
        
        public string Username { get; set; }
        
        public string Fullname { get; set; }
        
        public string Email { get; set; }
        
        public string Dob { get; set; }
        
        public string City { get; set; }
        
        public string Avatar { get; set; }
        
        public string Sex { get; set; }

        public long Coin { get; set; }
    }
}
