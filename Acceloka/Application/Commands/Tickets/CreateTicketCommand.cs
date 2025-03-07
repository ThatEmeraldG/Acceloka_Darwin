using MediatR;

namespace Acceloka.Application.Commands.Tickets
{
    public record CreateTicketCommand(
            string Username,
            string TicketCode,
            string TicketName,
            string CategoryName,
            DateTime EventStart,
            DateTime EventEnd,
            int Price,
            int Quota
    ) : IRequest<string>;
}
