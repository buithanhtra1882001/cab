﻿namespace CabPaymentService.Model.Entities
{
    public class BillInfo : Entity
    {
        public Guid Id { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }

        public VnPayTransaction Transaction { get; set; }
    }
}
