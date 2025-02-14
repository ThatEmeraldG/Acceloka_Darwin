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
        public BookedTicketDetailsService(AccelokaContext db)
        {
            _db = db;
        }

        // GET get-booked-ticket/{BookedTicketId}
        public async Task<object> Get(int id)
        {
            var data = await _db.BookedTickets
                .Include(btd => btd.BookedTicketDetails)
                .ThenInclude(tc => tc.TicketCodeNavigation)
                .FirstOrDefaultAsync(x => x.BookedTicketId == id);

            if (data == null)
            {
                return null;
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

            return bookedTicketDetails;
        }

        // DELETE revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}
        public async Task<object> Delete(int bookedTicketId, string ticketCode, int qty)
        {
            var existingData = await _db.BookedTickets
                                    .Include(bt => bt.BookedTicketDetails)
                                    .ThenInclude(bt => bt.TicketCodeNavigation)
                                    .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                return null;
            }

            var ticketDetails = existingData.BookedTicketDetails.FirstOrDefault(td => td.TicketCode == ticketCode);
            if (ticketDetails == null)
            {
                return null; // TicketCode not found
            }

            if (qty > ticketDetails.TicketQuantity)
            {
                return null; // Delete quantity > Actual quantity
            }

            ticketDetails.TicketQuantity -= qty;

            if (ticketDetails.TicketQuantity == 0) // Quantity = 0
            {
                _db.BookedTicketDetails.Remove(ticketDetails);
            }

            if (!existingData.BookedTicketDetails.Any(td => td.TicketQuantity > 0))
            {
                _db.BookedTickets.Remove(existingData);
            }

            await _db.SaveChangesAsync();

            return new
            {
                ticketCode = ticketDetails.TicketCode,
                ticketName = ticketDetails.TicketCodeNavigation.TicketName,
                categoryName = ticketDetails.TicketCodeNavigation.Category.CategoryName,
                remainingQty = ticketDetails.TicketQuantity
            };
        }

        // PUT edit-booked-ticket/{BookedTicketId}
        public async Task<List<object>> Put(int bookedTicketId, List<BookedTicketRequest> updatedTickets)
        {
            var existingData = await _db.BookedTickets
                .Include(bt => bt.BookedTicketDetails)
                .ThenInclude(bt => bt.TicketCodeNavigation)
                .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                return null;
            }

            List<object> updatedResults = new List<object>();

            foreach (var update in updatedTickets)
            {
                var bookedDetail = existingData.BookedTicketDetails.FirstOrDefault(btd => btd.TicketCode == update.TicketCode);
                if (bookedDetail == null)
                {
                    return null; // TicketCode not found
                }

                // Quantity kurang dari 1
                if (update.Quantity < 1)
                {
                    return null;
                }

                // Check total remaining quota
                var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketCode == update.TicketCode);
                if (ticket == null || update.Quantity > ticket.Quota)
                {
                    return null;
                }

                var username = updatedTickets.Any() && !string.IsNullOrEmpty(updatedTickets[0].UserName)
               ? updatedTickets[0].UserName
               : "System";

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
            return updatedResults;
        }
    }
}
