using MediatR;

namespace CabPostService.Handlers.Interfaces
{
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
}