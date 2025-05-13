namespace CabPaymentService.Model.Entities
{
    public class Invoice : Entity
    {
        public Guid Id { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Customer { get; set; }
        public string? Address { get; set; }
        public string? Company { get; set; }
        public string? Taxcode { get; set; }
        public string? Type { get; set; }

        public VnPayTransaction Transaction { get; set; }
    }
}
