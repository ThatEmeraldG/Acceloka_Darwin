using FluentValidation;
using MediatR;

namespace Acceloka.Application.Commands.Tickets
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("Ticket code is required.")
                .MaximumLength(50);

            RuleFor(x => x.TicketName)
                .NotEmpty().WithMessage("Ticket name is required.")
                .MaximumLength(100);

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(50);

            RuleFor(x => x.EventStart)
                .NotEqual(DateTime.MinValue).WithMessage("A valid event start date is required.");

            RuleFor(x => x.EventEnd)
                .NotEqual(DateTime.MinValue).WithMessage("A valid event end date is required.")
                .GreaterThan(x => x.EventStart).WithMessage("Event end must be after event start.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Quota)
                .GreaterThan(0).WithMessage("Quota must be greater than zero.");
        }
    }
}
