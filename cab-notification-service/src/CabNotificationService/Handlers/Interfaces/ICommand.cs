using MediatR;

namespace CabNotificationService.Handlers.Interfaces
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}