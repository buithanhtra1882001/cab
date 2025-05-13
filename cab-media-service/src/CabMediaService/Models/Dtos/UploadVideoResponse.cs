namespace CabMediaService.Models.Dtos
{
    public class UploadVideoResponse
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public double Size { get; set; }
        public double Duration { get; set; }
        public string ContentType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}