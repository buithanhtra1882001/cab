using MediatR;

namespace CabGroupService.Handlers.Interfaces
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}