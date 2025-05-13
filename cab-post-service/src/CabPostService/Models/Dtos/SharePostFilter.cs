using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Dtos;

public class SharePostFilter : PagingFilterBase
{
  [Required]
  public Guid UserId { get; set; }
}