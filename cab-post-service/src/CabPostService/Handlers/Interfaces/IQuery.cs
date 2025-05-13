using MediatR;

namespace CabPostService.Handlers.Interfaces
{
    public interface IQuery<out TReponse> : IRequest<TReponse>
    {
    }
}