namespace CabUserService.Models.Dtos
{
    public class DonationInfoPayload
    {
        public DonationInfoPayload(string senderId, double donateValue, string message, string audioSource) 
        {
            Id = Guid.NewGuid();
            SenderUserName = senderId;
            DonationValue = donateValue;
            Message = message;
            AudioSource = "https://ntblog.net/wp-content/uploads/2023/07/WIN-PK.mp3";
        }

        public Guid Id { get; }

        public string SenderUserName { get; set; }

        public double DonationValue { get; set; }

        public string Message { get; set; }

        public string AudioSource { get; set; }
    }
}
