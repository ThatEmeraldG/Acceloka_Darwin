﻿using Acceloka.Shared;
using MediatR;

namespace Acceloka.Application.Abstractions
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
     where TCommand : ICommand
    {

    }

    public interface ICommandHandler<TCommand, TResponse>
        : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
