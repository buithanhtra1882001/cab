using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities;

public class PostNotifyAdmin : BaseEntity
{
    public string Id { get; set; }
    public string PostId { get; set; }
    public bool IsAcceptHide { get; set; }
    public bool IsHandle { get; set; }
    public bool IsRead { get; set; }
    public bool IsDelete { get; set; }
    public int ReportNumber { get; set; }
    public string Description { get; set; }
}