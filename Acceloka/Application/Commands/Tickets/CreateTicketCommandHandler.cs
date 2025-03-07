using Acceloka.Entities;
using Acceloka.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Application.Commands.Tickets
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, string>
    {
        private readonly AccelokaContext _context;

        public CreateTicketCommandHandler(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == request.CategoryName, cancellationToken);

            if (category == null)
            {
                throw new Exception($"Category for '{request.CategoryName}' not found.");
            }

            var ticket = new Ticket
            {
                TicketCode = request.TicketCode,
                TicketName = request.TicketName,
                CategoryId = category.CategoryId,
                EventStart = request.EventStart,
                EventEnd = request.EventEnd,
                Price = request.Price,
                Quota = request.Quota,
                CreatedBy = request.Username,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.TicketCode;
        }
    }
}
