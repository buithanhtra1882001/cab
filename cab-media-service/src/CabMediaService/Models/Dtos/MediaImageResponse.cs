namespace CabMediaService.Models.Dtos
{
    public class MediaImageResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public double Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}