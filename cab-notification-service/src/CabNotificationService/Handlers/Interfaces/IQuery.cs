using MediatR;

namespace CabNotificationService.Handlers.Interfaces
{
    public interface IQuery<out TReponse> : IRequest<TReponse>
    {
    }
}