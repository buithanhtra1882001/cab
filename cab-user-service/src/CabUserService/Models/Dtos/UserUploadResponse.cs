namespace CabUserService.Models.Dtos
{
    public class UserUploadResponse
    {
        public string UploadedFileName { get; set; }
        public string Url { get; set; }
        public double Size { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
}
