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
            _logger.LogInformation("Fetching booked ticket details for BookedTicketId: {BookedTicketId}...", bookedTicketId);

            var data = await _db.BookedTickets
                .Include(bt => bt.BookedTicketDetails)
                .ThenInclude(bt => bt.TicketCodeNavigation)
                .ThenInclude(bt => bt.Category)
                .FirstOrDefaultAsync(x => x.BookedTicketId == bookedTicketId);

            if (data == null)
            {
                _logger.LogWarning("Booked ticket not found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            if (data.BookedTicketDetails == null || !data.BookedTicketDetails.Any())
            {
                return new List<object>();
            }

            var bookedTicketDetails = data.BookedTicketDetails
                        .GroupBy(btd => btd.TicketCodeNavigation.Category.CategoryName)
                        .Select(d => new
                        {
                            categoryName = d.Key,
                            qtyPerCategory = d.Sum(btd => btd.TicketQuantity),
                            tickets = d.Select(btd => new
                            {
                                ticketCode = btd.TicketCode,
                                ticketName = btd.TicketCodeNavigation.TicketName,
                                eventStart = btd.TicketCodeNavigation.EventStart.ToString("dd-MM-yyyy HH:mm"),
                                eventEnd = btd.TicketCodeNavigation.EventEnd.ToString("dd-MM-yyyy HH:mm"),
                                quantity = btd.TicketQuantity,
                                createdAt = btd.CreatedAt,
                                createdBy = btd.CreatedBy,
                                updatedAt = btd.UpdatedAt,
                                updatedBy = btd.UpdatedBy
                            }).ToList()
                        }).ToList();

            _logger.LogInformation("Successfully retrieved booked ticket details for BookedTicketId: {BookedTicketId}", bookedTicketId);
            return bookedTicketDetails;
        }

        // DELETE revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}
        public async Task<object> DeleteBookedTicket(int bookedTicketId, string ticketCode, int qty)
        {
            _logger.LogInformation("Revoking {Quantity} tickets with code {TicketCode} from booking {BookedTicketId}...",
                qty, ticketCode, bookedTicketId);

            var existingData = await _db.BookedTickets
                                    .Include(bt => bt.BookedTicketDetails)
                                    .ThenInclude(bt => bt.TicketCodeNavigation)
                                    .ThenInclude(bt => bt.Category)
                                    .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                _logger.LogWarning("Booked ticket not found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            var ticketDetails = existingData.BookedTicketDetails.FirstOrDefault(td => td.TicketCode == ticketCode);
            if (ticketDetails == null)
            {
                _logger.LogWarning("Ticket code {TicketCode} not found in booking {BookedTicketId}", ticketCode, bookedTicketId);
                return null; // TicketCode not found
            }

            if (qty > ticketDetails.TicketQuantity)
            {
                _logger.LogWarning("Requested quantity {Quantity} exceeds available quantity {AvailableQty} for ticket {TicketCode}",
                    qty, ticketDetails.TicketQuantity, ticketCode);
                return null; // Delete quantity > Actual quantity
            }

            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketCode == ticketCode);
            if (ticket != null)
            {
                ticket.Quota += qty;
            }

            ticketDetails.TicketQuantity -= qty;
            ticketDetails.UpdatedAt = DateTime.UtcNow;
            ticketDetails.UpdatedBy = "System";

            if (ticketDetails.TicketQuantity == 0)
            {
                _logger.LogInformation("Removing ticket booking for {TicketCode}, quantity reached zero", ticketCode);
                _db.BookedTicketDetails.Remove(ticketDetails);
            }

            if (!existingData.BookedTicketDetails.Any(td => td.TicketQuantity > 0))
            {
                _logger.LogInformation("Removing entire booking {BookedTicketId}, no tickets remain", bookedTicketId);
                _db.BookedTickets.Remove(existingData);
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully revoked {Quantity} tickets of {TicketCode} from booking {BookedTicketId}",
                qty, ticketCode, bookedTicketId);

            return new
            {
                ticketCode = ticketDetails.TicketCode,
                ticketName = ticketDetails.TicketCodeNavigation.TicketName,
                categoryName = ticketDetails.TicketCodeNavigation.Category?.CategoryName ?? "No Category",
                remainingQty = ticketDetails.TicketQuantity
            };
        }

        // PUT edit-booked-ticket/{BookedTicketId}
        public async Task<List<object>> EditBookedTicket(int bookedTicketId, List<BookTicketRequest> updatedTickets)
        {
            _logger.LogInformation("Updating booked ticket {BookedTicketId}...",
                bookedTicketId);

            var existingData = await _db.BookedTickets
                .Include(bt => bt.BookedTicketDetails)
                .ThenInclude(bt => bt.TicketCodeNavigation)
                .ThenInclude(bt => bt.Category)
                .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                _logger.LogWarning("Booked ticket not found for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return null;
            }

            List<object> updatedResults = new List<object>();

            foreach (var update in updatedTickets)
            {
                _logger.LogInformation("Processing update for ticket {TicketCode} with quantity {Quantity}",
                        update.TicketCode, update.Quantity);

                var bookedDetail = existingData.BookedTicketDetails.FirstOrDefault(btd => btd.TicketCode == update.TicketCode);
                if (bookedDetail == null)
                {
                    _logger.LogWarning("Ticket code {TicketCode} not found in booking {BookedTicketId}",
                            update.TicketCode, bookedTicketId);
                    return null;
                }

                // Quantity kurang dari 1
                if (update.Quantity < 1)
                {
                    _logger.LogWarning("Invalid quantity {Quantity} provided for ticket {TicketCode}",
                            update.Quantity, update.TicketCode);
                    return null;
                }

                // Check total remaining quota
                var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketCode == update.TicketCode);
                if (ticket == null || update.Quantity > ticket.Quota)
                {
                    _logger.LogWarning("Ticket with code {TicketCode} not found in database", update.TicketCode);
                    return null;
                }

                int quantityDiff = update.Quantity - bookedDetail.TicketQuantity;

                if (quantityDiff > 0 && quantityDiff > ticket.Quota)
                {
                    _logger.LogWarning("Insufficient quota for ticket {TicketCode}. Requested increase: {Requested}, Available: {Available}",
                        update.TicketCode, quantityDiff, ticket.Quota);
                    return null;
                }

                // Valid, update quota ticketnya
                if (quantityDiff != 0)
                {
                    ticket.Quota -= quantityDiff;
                    _logger.LogInformation("Updated quota for ticket {TicketCode}. New quota: {NewQuota}",
                        update.TicketCode, ticket.Quota);
                }

                var username = "System";

                bookedDetail.TicketQuantity = update.Quantity;
                bookedDetail.UpdatedAt = DateTime.UtcNow;
                bookedDetail.UpdatedBy = username;

                _db.BookedTicketDetails.Update(bookedDetail);

                updatedResults.Add(new
                {
                    ticketCode = bookedDetail.TicketCode,
                    ticketName = bookedDetail.TicketCodeNavigation.TicketName,
                    categoryName = bookedDetail.TicketCodeNavigation.Category.CategoryName,
                    quantity = bookedDetail.TicketQuantity
                });
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Successfully updated booked ticket {BookedTicketId}", bookedTicketId);
            return updatedResults;
        }
    }
}
