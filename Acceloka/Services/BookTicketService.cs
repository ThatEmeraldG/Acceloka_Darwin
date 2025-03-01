using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Acceloka.Services
{
    public class BookTicketService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<BookTicketService> _logger;
        public BookTicketService(AccelokaContext db, ILogger<BookTicketService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // POST book-ticket
        public async Task<object> BookTickets(List<BookTicketRequest> tickets, string? username)
        {
            _logger.LogInformation("Starting ticket booking process for {Count} tickets...", tickets.Count);

            var errors = new List<string>();
            var validBookings = new List<(Ticket ticket, int quantity)>(); // Track List of Valid Bookings
            var currentDate = DateTime.UtcNow;

            foreach (var item in tickets)
            {
                _logger.LogInformation("Validating ticket with code {TicketCode}", item.TicketCode);

                var ticket = await _db.Tickets
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.TicketCode == item.TicketCode);

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

            if (errors.Any())
            {
                _logger.LogError("Errors encountered during ticket booking: {Errors}", string.Join(", ", errors));
                return new
                {
                    success = false,
                    errors
                };
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var totalPrice = validBookings.Sum(b => b.ticket.Price * b.quantity);

                // Create Transaction
                var transactionRecord = new Transaction
                {
                    TransactionDate = currentDate,
                    TotalPrice = totalPrice,
                    TotalPayment = totalPrice,
                    PaymentMethod = "Credit Card", // Test sudah dibayar
                    CreatedAt = currentDate,
                    CreatedBy = string.IsNullOrEmpty(username) ? "System" : username
                };

                _db.Transactions.Add(transactionRecord);
                await _db.SaveChangesAsync();

                // Create BookedTicket with transaction
                var bookedTicket = new BookedTicket
                {
                    TransactionId = transactionRecord.TransactionId,
                    BookingDate = currentDate,
                    TotalCategoryPrice = totalPrice,
                    CreatedAt = currentDate,
                    CreatedBy = string.IsNullOrEmpty(username) ? "System" : username
                };

                _db.BookedTickets.Add(bookedTicket);
                await _db.SaveChangesAsync();

                // BookedTicketDetail & Update Quota
                foreach (var (ticket, quantity) in validBookings)
                {
                    var bookedDetail = new BookedTicketDetail
                    {
                        BookedTicketId = bookedTicket.BookedTicketId,
                        TicketCode = ticket.TicketCode,
                        TicketQuantity = quantity,
                        TotalTicketPrice = ticket.Price * quantity,
                        CreatedAt = currentDate,
                        CreatedBy = string.IsNullOrEmpty(username) ? "System" : username
                    };

                    _db.BookedTicketDetails.Add(bookedDetail);

                    ticket.Quota -= quantity;
                    ticket.UpdatedAt = currentDate;
                    ticket.UpdatedBy = username;
                    _db.Tickets.Update(ticket);

                    _logger.LogInformation("Updated quota for ticket {TicketCode} to {RemainingQuota}",
                        ticket.TicketCode, ticket.Quota);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var ticketsPerCategory = validBookings
                    .GroupBy(b => b.ticket.Category.CategoryName)
                    .Select(g => new
                    {
                        categoryName = g.Key,
                        summaryPrice = g.Sum(x => x.ticket.Price * x.quantity),
                        tickets = g.Select(b => new
                        {
                            ticketCode = b.ticket.TicketCode,
                            ticketName = b.ticket.TicketName,
                            price = b.ticket.Price,
                            quantity = b.quantity
                        }).ToList()
                    })
                    .ToList();

                _logger.LogInformation("Booking process completed successfully. Total price: {TotalPrice}", totalPrice);

                return new
                {
                    priceSummary = totalPrice,
                    ticketsPerCategory
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during booking process");
                return new
                {
                    success = false,
                    error = "An unexpected error occurred during the booking process"
                };
            }
        }
    }
}