using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;

namespace CabPostService.Services.Abstractions
{
    public interface IDonateService
    {
        Task<ResponseDto> DonateReceiverAsync(DonateReceiverCommand command, CancellationToken cancellationToken = default);

        Task<bool> HandleDonateReceiverAsync(HandleDonateReceiverCommand command, CancellationToken cancellationToken = default);

        Task<StatisticalDonateDto> HandlesSatisticalDonateAsync(StatisticalDonateQuery request, CancellationToken cancellationToken);

        Task<List<DonateDetailResponse>> GetDetailDonateAsync(GetDetailDonateQuery request, CancellationToken cancellationToken);

        Task<List<GetLstReceiveAmountsByIdResponse>> GetLstReceiveAmountsByIdAsync(GetLstReceiveAmountsByIdQuery request, CancellationToken cancellationToken);
    }
}
