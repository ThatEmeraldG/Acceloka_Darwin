using MediatR;

namespace Acceloka.Application.Commands.Bookings
{
    public record BookTicketItem(
        string TicketCode,
        int Quantity
    );

    public record BookTicketsCommand(
        List<BookTicketItem> Tickets,
        string Username
    ) : IRequest<BookTicketsResult>;

    public record TicketInfo(
        string TicketCode,
        string TicketName,
        int Price,
        int Quantity
    );

    public record TicketsPerCategory(
        string CategoryName,
        int SummaryPrice,
        List<TicketInfo> Tickets
    );

    public record BookTicketsResult(
        bool Success,
        List<string> Errors,
        int PriceSummary,
        List<TicketsPerCategory> TicketsPerCategory
    );
}
