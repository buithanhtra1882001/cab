namespace CabUserService.Models.Dtos
{
    public class ImageUploadResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public double Size { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}