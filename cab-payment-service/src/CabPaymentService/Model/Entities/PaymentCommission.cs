using System.ComponentModel.DataAnnotations;
using CabPaymentService.Model.Entities.Base;

namespace CabPaymentService.Model.Entities;

public class PaymentCommission : BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal CommissionPercentage { get; set; }
}