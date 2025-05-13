using MediatR;

namespace CabPostService.Handlers.Interfaces
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}