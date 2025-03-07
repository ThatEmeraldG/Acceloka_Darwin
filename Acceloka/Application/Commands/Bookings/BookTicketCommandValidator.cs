using FluentValidation;

namespace Acceloka.Application.Commands.Bookings
{
    public class BookTicketsCommandValidator : AbstractValidator<BookTicketsCommand>
    {
        public BookTicketsCommandValidator()
        {
            RuleFor(x => x.Tickets)
                .NotNull().WithMessage("Tickets list cannot be null.")
                .Must(t => t.Any()).WithMessage("Book at least one ticket.");

            RuleForEach(x => x.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(x => x.TicketCode)
                    .NotEmpty().WithMessage("Ticket code is required.");
                ticket.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            });
        }
    }
}
