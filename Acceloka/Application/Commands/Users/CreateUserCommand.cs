using MediatR;

namespace Acceloka.Application.Commands.Users
{
    public record CreateUserCommand(
        string UserId,
        string UserName,
        string UserEmail,
        string UserPassword
    ) : IRequest<string>;
}
