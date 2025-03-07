using Acceloka.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Application.Commands.Bookings
{
    public class BookTicketsCommandHandler : IRequestHandler<BookTicketsCommand, BookTicketsResult>
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<BookTicketsCommandHandler> _logger;

        public BookTicketsCommandHandler(AccelokaContext db, ILogger<BookTicketsCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<BookTicketsResult> Handle(BookTicketsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting ticket booking process for {Count} tickets...", request.Tickets.Count);

            var errors = new List<string>();
            var validBookings = new List<(Ticket ticket, int quantity)>();
            var currentDate = DateTime.UtcNow;

            // Validate each booking item
            foreach (var item in request.Tickets)
            {
                _logger.LogInformation("Validating ticket with code {TicketCode}", item.TicketCode);

                var ticket = await _db.Tickets
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.TicketCode == item.TicketCode, cancellationToken);

                if (ticket == null)
                {
                    errors.Add($"Ticket code '{item.TicketCode}' is not registered");
                    _logger.LogWarning("Ticket code {TicketCode} is not registered", item.TicketCode);
                    continue;
                }

                if (ticket.Quota <= 0)
                {
                    errors.Add($"Ticket code '{item.TicketCode}' is sold out");
                    _logger.LogWarning("Ticket code {TicketCode} is sold out", item.TicketCode);
                    continue;
                }

                if (item.Quantity > ticket.Quota)
                {
                    errors.Add($"Requested quantity {item.Quantity} for ticket '{item.TicketCode}' exceeds available quota ({ticket.Quota})");
                    _logger.LogWarning("Ticket code {TicketCode} quantity {Quantity} exceeds available quota {Quota}",
                        item.TicketCode, item.Quantity, ticket.Quota);
                    continue;
                }

                if (ticket.EventEnd <= currentDate)
                {
                    errors.Add($"Event for ticket '{item.TicketCode}' has already ended");
                    _logger.LogWarning("Ticket code {TicketCode} event has already ended", item.TicketCode);
                    continue;
                }

                validBookings.Add((ticket, item.Quantity));
            }

            // If any errors occurred, return a failure result
            if (errors.Any())
            {
                _logger.LogError("Errors encountered during ticket booking: {Errors}", string.Join(", ", errors));
                return new BookTicketsResult(
                    Success: false,
                    Errors: errors,
                    PriceSummary: 0,
                    TicketsPerCategory: new List<TicketsPerCategory>()
                );
            }

            // Process valid bookings in a transaction
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var totalPrice = validBookings.Sum(b => b.ticket.Price * b.quantity);

                // Create a Transaction record
                var transactionRecord = new Transaction
                {
                    TransactionDate = currentDate,
                    TotalPrice = totalPrice,
                    TotalPayment = totalPrice,
                    PaymentMethod = "Credit Card", // Example; adjust as needed
                    CreatedAt = currentDate,
                    CreatedBy = string.IsNullOrEmpty(request.Username) ? "System" : request.Username
                };

                _db.Transactions.Add(transactionRecord);
                await _db.SaveChangesAsync(cancellationToken);

                // Create a BookedTicket record linked to the transaction
                var bookedTicket = new BookedTicket
                {
                    TransactionId = transactionRecord.TransactionId,
                    BookingDate = currentDate,
                    TotalCategoryPrice = totalPrice,
                    CreatedAt = currentDate,
                    CreatedBy = string.IsNullOrEmpty(request.Username) ? "System" : request.Username
                };

                _db.BookedTickets.Add(bookedTicket);
                await _db.SaveChangesAsync(cancellationToken);

                // For each valid booking, create a BookedTicketDetail record and update Ticket quota
                foreach (var (ticket, quantity) in validBookings)
                {
                    var bookedDetail = new BookedTicketDetail
                    {
                        BookedTicketId = bookedTicket.BookedTicketId,
                        TicketCode = ticket.TicketCode,
                        TicketQuantity = quantity,
                        TotalTicketPrice = ticket.Price * quantity,
                        CreatedAt = currentDate,
                        CreatedBy = string.IsNullOrEmpty(request.Username) ? "System" : request.Username
                    };

                    _db.BookedTicketDetails.Add(bookedDetail);

                    ticket.Quota -= quantity;
                    ticket.UpdatedAt = currentDate;
                    ticket.UpdatedBy = request.Username;
                    _db.Tickets.Update(ticket);

                    _logger.LogInformation("Updated quota for ticket {TicketCode} to {RemainingQuota}",
                        ticket.TicketCode, ticket.Quota);
                }

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Group valid bookings by category to build a summary
                var ticketsPerCategory = validBookings
                    .GroupBy(b => b.ticket.Category.CategoryName)
                    .Select(g => new TicketsPerCategory(
                        CategoryName: g.Key,
                        SummaryPrice: g.Sum(x => x.ticket.Price * x.quantity),
                        Tickets: g.Select(b => new TicketInfo(
                            TicketCode: b.ticket.TicketCode,
                            TicketName: b.ticket.TicketName,
                            Price: b.ticket.Price,
                            Quantity: b.quantity
                        )).ToList()
                    ))
                    .ToList();

                _logger.LogInformation("Booking process completed successfully. Total price: {TotalPrice}", totalPrice);

                return new BookTicketsResult(
                    Success: true,
                    Errors: new List<string>(),
                    PriceSummary: totalPrice,
                    TicketsPerCategory: ticketsPerCategory
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred during booking process");
                return new BookTicketsResult(
                    Success: false,
                    Errors: new List<string> { "An unexpected error occurred during the booking process" },
                    PriceSummary: 0,
                    TicketsPerCategory: new List<TicketsPerCategory>()
                );
            }
        }
    }
}
