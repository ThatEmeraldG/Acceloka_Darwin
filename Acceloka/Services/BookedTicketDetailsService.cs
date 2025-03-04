using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acceloka.Services
{
    public class BookedTicketDetailsService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<BookedTicketDetailsService> _logger;
        public BookedTicketDetailsService(AccelokaContext db, ILogger<BookedTicketDetailsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET get-booked-ticket/{BookedTicketId}
        public async Task<object> GetBookedTicket(int bookedTicketId)
        {
            _logger.LogInformation("Fetching ticket details for BookedTicketId: {BookedTicketId}...", bookedTicketId);

            var data = await _db.BookedTicketDetails
                .Where(btd => btd.BookedTicketId == bookedTicketId)
                .Include(btd => btd.BookedTicket)
                .Include(btd => btd.TicketCodeNavigation)
                .ThenInclude(tc => tc.Category)
                .ToListAsync();

            if (!data.Any())
            {
                _logger.LogWarning("No booking found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            var response = new
            {
                BookedTicketId = bookedTicketId,
                Categories = data
                        .GroupBy(btd => btd.TicketCodeNavigation.Category.CategoryName)
                        .Select(res => new
                        {
                            categoryName = res.Key,
                            qtyPerCategory = res.Sum(btd => btd.TicketQuantity),
                            tickets = res.Select(btd => new
                            {
                                ticketCode = btd.TicketCode,
                                ticketName = btd.TicketCodeNavigation.TicketName,
                                eventStart = btd.TicketCodeNavigation.EventStart.ToString("dd-MM-yyyy HH:mm"),
                                eventEnd = btd.TicketCodeNavigation.EventEnd.ToString("dd-MM-yyyy HH:mm"),
                                quantity = btd.TicketQuantity
                            }).ToList()
                        }).ToList(),
                CreatedAt = data.First().BookedTicket.CreatedAt,
                CreatedBy = data.First().BookedTicket.CreatedBy,
                UpdatedAt = data.First().BookedTicket.UpdatedAt,
                UpdatedBy = data.First().BookedTicket.UpdatedBy
            };

            _logger.LogInformation("Successfully retrieved booked ticket details for BookedTicketId: {BookedTicketId}", bookedTicketId);
            return response;
        }

        // DELETE revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}
        public async Task<object> DeleteBookedTicket(int bookedTicketId, string ticketCode, int qty, string? username)
        {
            _logger.LogInformation("Revoking {Quantity} tickets with code {TicketCode} from booking {BookedTicketId}...",
                qty, ticketCode, bookedTicketId);

            var existingBooking = await _db.BookedTicketDetails
                .Where(btd => btd.BookedTicketId == bookedTicketId)
                .Include(btd => btd.BookedTicket)
                .Include(btd => btd.TicketCodeNavigation)
                .ThenInclude(ticket => ticket.Category)
                .ToListAsync();
                
            if (!existingBooking.Any())
            {
                _logger.LogWarning("No booking found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            var bookedTicket = existingBooking.FirstOrDefault(btd => btd.TicketCode == ticketCode);
            if (bookedTicket == null)
            {
                _logger.LogWarning("Ticket code {TicketCode} not found in booking {BookedTicketId}", ticketCode, bookedTicketId);
                return null;
            }

            if (qty > bookedTicket.TicketQuantity)
            {
                _logger.LogWarning("Requested quantity {Quantity} exceeds available quantity {AvailableQty} for ticket {TicketCode}",
                    qty, bookedTicket.TicketQuantity, ticketCode);
                return null; // Delete quantity > Actual quantity
            }

            var ticket = await _db.Tickets.FirstOrDefaultAsync(tc => tc.TicketCode == ticketCode);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketCode} not found in system", ticketCode);
                return null;
            }
            
            ticket.Quota += qty;

            bookedTicket.TicketQuantity -= qty;
            bookedTicket.UpdatedAt = DateTime.UtcNow;
            bookedTicket.UpdatedBy = string.IsNullOrEmpty(username) ? "System" : username;

            if (bookedTicket.TicketQuantity == 0)
            {
                _logger.LogInformation("Removing ticket booking for {TicketCode}, quantity reached zero", ticketCode);
                _db.BookedTicketDetails.Remove(bookedTicket);
            }

            var remainingTickets = existingBooking.Count(btd => btd.TicketQuantity > 0);

            if (remainingTickets == 0)
            {
                var bookingToRemove = await _db.BookedTickets
                    .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

                if (bookingToRemove != null)
                {
                    _logger.LogInformation("Removing entire booking {BookedTicketId}, no tickets remain", bookedTicketId);
                    _db.BookedTickets.Remove(bookingToRemove);
                }
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully revoked {Quantity} tickets of {TicketCode} from booking {BookedTicketId}",
                qty, ticketCode, bookedTicketId);

            return new
            {
                ticketCode = bookedTicket.TicketCode,
                ticketName = bookedTicket.TicketCodeNavigation.TicketName,
                categoryName = bookedTicket.TicketCodeNavigation.Category?.CategoryName ?? "No Category",
                remainingQty = bookedTicket.TicketQuantity
            };
        }

        // PUT edit-booked-ticket/{BookedTicketId}
        public async Task<List<object>> EditBookedTicket(int bookedTicketId, List<BookTicketRequest> updatedTickets, string? username)
        {
            _logger.LogInformation("Updating ticket booking for {BookedTicketId}...",
                bookedTicketId);

            var existingBooking = await _db.BookedTicketDetails
                .Where(btd => btd.BookedTicketId == bookedTicketId)
                .Include(btd => btd.BookedTicket)
                .Include(btd => btd.TicketCodeNavigation)
                .ThenInclude(ticket => ticket.Category)
                .ToListAsync();

            if (!existingBooking.Any())
            {
                _logger.LogWarning("No booking found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            List<object> updatedResults = new List<object>();

            foreach (var update in updatedTickets)
            {
                _logger.LogInformation("Processing update for ticket {TicketCode} with quantity {Quantity}",
                        update.TicketCode, update.Quantity);

                var bookedTicket = existingBooking.FirstOrDefault(btd => btd.TicketCode == update.TicketCode);
                if (bookedTicket == null)
                {
                    _logger.LogWarning("Ticket code {TicketCode} not found in booking {BookedTicketId}",
                            update.TicketCode, bookedTicketId);
                    return null;
                }

                // Quantity kurang dari 1
                if (update.Quantity < 1)
                {
                    _logger.LogWarning("Invalid quantity {Quantity} provided to update ticket: {TicketCode}",
                            update.Quantity, update.TicketCode);
                    return null;
                }

                // Check total remaining quota
                var ticket = await _db.Tickets.FirstOrDefaultAsync(tc => tc.TicketCode == update.TicketCode);
                if (ticket == null)
                {
                    _logger.LogWarning("Ticket with code {TicketCode} is not available", update.TicketCode);
                    return null;
                }

                int quantityDiff = update.Quantity - bookedTicket.TicketQuantity;

                if (quantityDiff > 0 && update.Quantity > ticket.Quota)
                {
                    _logger.LogWarning("Insufficient quota for ticket {TicketCode}. Requested change: {QtyDiff}, Ticket Quantity after change: {Qty}, Available: {Available}",
                        update.TicketCode, quantityDiff, update.Quantity, ticket.Quota);
                    return null;
                }

                // Valid, update quota ticketnya
                if (quantityDiff != 0)
                {
                    ticket.Quota -= quantityDiff;
                    _logger.LogInformation("Updated quota for ticket {TicketCode}. New quota: {NewQuota}",
                        update.TicketCode, ticket.Quota);
                }

                bookedTicket.TicketQuantity = update.Quantity;
                bookedTicket.UpdatedAt = DateTime.UtcNow;
                bookedTicket.UpdatedBy = string.IsNullOrEmpty(username) ? "System" : username;

                _db.BookedTicketDetails.Update(bookedTicket);

                updatedResults.Add(new
                {
                    ticketCode = bookedTicket.TicketCode,
                    ticketName = bookedTicket.TicketCodeNavigation.TicketName,
                    quantity = bookedTicket.TicketQuantity,
                    categoryName = bookedTicket.TicketCodeNavigation.Category.CategoryName
                });
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Successfully updated booked ticket {BookedTicketId}", bookedTicketId);
            return updatedResults;
        }
    }
}
