namespace CabPostService.Models.Dtos
{
    public class GetAllPostFilter : PagingFilterBase
    {
        public string Slug { get; set; }
        public bool? IsSoftDeleted { get; set; }
        public int? Status { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsPersonalPost { get; set; } = true;
    }
}
