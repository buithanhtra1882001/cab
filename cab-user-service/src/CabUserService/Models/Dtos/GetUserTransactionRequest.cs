using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class GetUserTransactionRequest : PagingRequest
    {
        public TransactionType? Type { get; set; }
    }
}
