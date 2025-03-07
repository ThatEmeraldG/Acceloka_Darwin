using FluentValidation;

namespace Acceloka.Application.Commands.Users
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50);

            RuleFor(x => x.UserEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.")
                .MaximumLength(255);

            RuleFor(x => x.UserPassword)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(255);
        }
    }
}
