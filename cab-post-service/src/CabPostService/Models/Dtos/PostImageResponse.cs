namespace CabPostService.Models.Dtos
{
    public class PostImageResponse
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; }
        public bool IsViolence { get; set; }
    }
}
