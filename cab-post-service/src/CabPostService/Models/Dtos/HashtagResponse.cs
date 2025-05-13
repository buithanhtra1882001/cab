namespace CabPostService.Models.Dtos
{
    public class HashtagResponse
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActived { get; set; }
        public double Point { get; set; }
    }
}
