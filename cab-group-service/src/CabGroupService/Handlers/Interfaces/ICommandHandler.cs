﻿using MediatR;

namespace CabGroupService.Handlers.Interfaces
{
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
}