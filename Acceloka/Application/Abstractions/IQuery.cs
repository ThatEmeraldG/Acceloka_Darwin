using Acceloka.Shared;
using MediatR;

namespace Acceloka.Application.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
