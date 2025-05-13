namespace CabPostService.Models.Dtos
{
    public class GetLstReceiveAmountsByIdResponse
    {
        public Guid DonaterId { get; set; }

        public Guid ReceiverId { get; set; }

        public double? TotalAmount { get; set; }
    }
}
