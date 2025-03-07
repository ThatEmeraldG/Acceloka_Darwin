using Acceloka.Shared;
using MediatR;

namespace Acceloka.Application.Abstractions
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
