using MediatR;

namespace CabGroupService.Handlers.Interfaces
{
    public interface IQuery<out TReponse> : IRequest<TReponse>
    {
    }
}