using Acceloka.Entities;
using MediatR;

namespace Acceloka.Application.Commands.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly AccelokaContext _context;

        public CreateUserCommandHandler(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserId = request.UserId,
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                UserPassword = request.UserPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user.UserId;
        }
    }
}
