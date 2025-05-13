namespace CabMediaService.Models.Dtos
{
    public class UpdateMediaImageRequest
    {
        public IEnumerable<Guid> Guids { get; set; }
        public DateTime LastUsedAt { get; set; }
    }
}