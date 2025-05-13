namespace CabPaymentService.Model.Entities.Base
{
    public class BaseEntity : Entity
    {
        public Guid UpdatedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid DeletedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
