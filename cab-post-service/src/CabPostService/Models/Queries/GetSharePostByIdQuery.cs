using System.ComponentModel.DataAnnotations;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries;

public class GetSharePostByIdQuery : IQuery<SharePostResponse>
{
  [Required]
  public Guid Id { get; set; }
}