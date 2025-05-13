using CabMediaService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabMediaService.Models.Entities
{
    public class MediaImage : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public double Size { get; set; }
        public DateTime LastUsedAt { get; set; }
    }
}