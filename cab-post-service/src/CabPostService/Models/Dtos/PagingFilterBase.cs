namespace CabPostService.Models.Dtos
{
  public class PagingFilterBase
  {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;

    public string PagingQueryString => $" ORDER BY \"CreatedAt\" DESC OFFSET {Skip} LIMIT {PageSize}";
  }
}
