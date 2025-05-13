using CabUserService.Models.Entities;
using Google.Protobuf.WellKnownTypes;

namespace CabUserService.Models.Dtos
{
    public class DonateReceiverRequestResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string NationalId { get; set; }
        public string ReferenceLinks { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public DonateReceiverRequestStatus Status { get; set; }
        public Timestamp CreatedAt { get; set; }
        public Timestamp UpdatedAt { get; set; }
    }
}
