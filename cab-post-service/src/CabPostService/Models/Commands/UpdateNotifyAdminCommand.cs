using System.ComponentModel.DataAnnotations;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands;

public class UpdateNotifyAdminCommand : ICommand<bool>
{
    [Required]
    public string IdNotify { get; set; }

    [Required]
    public bool IsAcceptHide { get; set; }
}