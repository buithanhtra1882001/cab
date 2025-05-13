using CabUserService.Models.Entities;
using System.Net;

namespace CabUserService.Models.Dtos
{
    public class DonateReceiverRequestDto
    {
        public string NationalId { get; set; }
        public string ReferenceLinks { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
    }
}
