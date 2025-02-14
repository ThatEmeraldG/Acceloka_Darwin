using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;

namespace Acceloka.Services
{
    public class BookedTicketService
    {
        private readonly AccelokaContext _db;
        public BookedTicketService(AccelokaContext db)
        {
            _db = db;
        }

        // POST book-ticket
        public async Task<string> Post(List<BookedTicketRequest> requests)
        {
            if (!requests.Any())
                return "No tickets available";

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var username = requests.Select(r => r.UserName)
                                      .FirstOrDefault(u => !string.IsNullOrEmpty(u))
                                  ?? "System";

                var bookingDate = DateTime.UtcNow;
                var totalAllCategories = 0;

                var ticketCodes = requests.Select(r => r.TicketCode).Distinct().ToList();
                var tickets = await _db.Tickets
                    .Include(t => t.Category)
                    .Where(t => ticketCodes.Contains(t.TicketCode))
                    .ToDictionaryAsync(t => t.TicketCode);

                var bookedTicket = new BookedTicket
                {
                    BookingDate = bookingDate,
                    CreatedAt = bookingDate,
                    CreatedBy = username,
                };
                _db.Add(bookedTicket);
                await _db.SaveChangesAsync();

                foreach (var request in requests)
                {
                    if (!tickets.TryGetValue(request.TicketCode, out var ticket))
                        throw new KeyNotFoundException($"Ticket {request.TicketCode} not found");

                    ValidateTicket(ticket, request, bookingDate);

                    if (ticket.Quota < request.Quantity)
                        throw new InvalidOperationException($"Item quantity must not exceed ticket quota.");

                    ticket.Quota -= request.Quantity;

                    var totalTicketPrice = ticket.Price * request.Quantity;
                    totalAllCategories += totalTicketPrice;

                    _db.BookedTicketDetails.Add(new BookedTicketDetail
                    {
                        BookedTicketId = bookedTicket.BookedTicketId,
                        TicketCode = request.TicketCode,
                        TicketQuantity = request.Quantity,
                        TotalTicketPrice = totalTicketPrice,
                        CreatedAt = bookingDate,
                        CreatedBy = username,
                        UpdatedAt = bookingDate,
                        UpdatedBy = username
                    });
                }

                _db.Transactions.Add(new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    TotalPrice = totalAllCategories,
                    TotalPayment = 0,
                    PaymentMethod = "Pending",
                    CreatedAt = bookingDate,
                    CreatedBy = username
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return $"Successfully booked {requests.Count} tickets. Transaction ID: {bookedTicket.BookedTicketId}";
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Ticket booking failed. See inner exception.", e);
            }
        }
        private void ValidateTicket(Ticket ticket, BookedTicketRequest request, DateTime bookingDate)
        {
            if (ticket == null)
                throw new Exception($"Ticket {request.TicketCode} is not available.");

            if (ticket.Quota <= 0)
                throw new Exception($"Quota for Ticket {request.TicketCode} is sold out.");

            if (request.Quantity <= 0)
                throw new Exception($"Quantity must be at least 1.");

            if (ticket.EventEnd <= bookingDate)
                throw new Exception($"Event for Ticket {request.TicketCode} has expired.");
        }
    }
}
