﻿using Acceloka.Shared;
using MediatR;

namespace Acceloka.Application.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
