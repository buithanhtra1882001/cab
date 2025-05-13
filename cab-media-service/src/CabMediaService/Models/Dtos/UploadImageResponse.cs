using Newtonsoft.Json;

namespace CabMediaService.Models.Dtos
{
    public class UploadImageResponse
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public double Size { get; set; }
        public string ContentType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}