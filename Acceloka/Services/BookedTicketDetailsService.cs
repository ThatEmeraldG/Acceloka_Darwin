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
        public async Task<BookedTicketDetailsModel> Get(int id)
        {
            var data = await _db.BookedTickets
                .Include(btd => btd.BookedTicketDetails)
                .ThenInclude(tc => tc.TicketCodeNavigation)
                .FirstOrDefaultAsync(x => x.BookedTicketId == id);

            if (data == null)
            {
                return null;
            }

            var bookedTicket = data.BookedTicketDetails.Select(btd => new BookedTicketDetailsModel
            {
                BookedDetailId = btd.BookedTicketId,
                TicketCode = btd.TicketCode,
                TicketName = btd.TicketCodeNavigation.TicketName,
                EventStart = btd.TicketCodeNavigation.EventStart,
                EventEnd = btd.TicketCodeNavigation.EventEnd,
                CreatedAt = btd.CreatedAt,
                CreatedBy = btd.CreatedBy,
                UpdatedAt = btd.UpdatedAt,
                UpdatedBy = btd.UpdatedBy
            }).FirstOrDefault();

            return bookedTicket;
        }

        // DELETE revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}
        public async Task<object> Delete(int bookedTicketId, string ticketCode, int qty)
        {
            var existingData = await _db.BookedTickets
                                    .Include(bt => bt.BookedTicketDetails)
                                    .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                return null; // BookedTicketId not found
            }

            var ticketDetail = existingData.BookedTicketDetails.FirstOrDefault(td => td.TicketCode == ticketCode);
            if (ticketDetail == null)
            {
                return null; // TicketCode not found
            }

            if (qty > ticketDetail.TicketQuantity)
            {
                return null; // Delete qty > actual quantity
            }

            ticketDetail.TicketQuantity -= qty;

            if (ticketDetail.TicketQuantity == 0) // Quantity = 0
            {
                _db.BookedTicketDetails.Remove(ticketDetail);
            }

            if (!existingData.BookedTicketDetails.Any(td => td.TicketQuantity > 0))
            {
                _db.BookedTickets.Remove(existingData);
            }

            await _db.SaveChangesAsync();

            return new
            {
                TicketCode = ticketDetail.TicketCode,
                TicketName = ticketDetail.TicketCodeNavigation.TicketName,
                CategoryName = ticketDetail.TicketCodeNavigation.Category.CategoryName,
                RemainingQuantity = ticketDetail.TicketQuantity
            };
        }

        // PUT edit-booked-ticket/{BookedTicketId}
        public async Task<List<object>> Put(int bookedTicketId, List<BookedTicketRequest> updatedTickets)
        {
            var existingData = await _db.BookedTickets
                .Include(bt => bt.BookedTicketDetails)
                .ThenInclude(td => td.TicketCodeNavigation)
                .FirstOrDefaultAsync(bt => bt.BookedTicketId == bookedTicketId);

            if (existingData == null)
            {
                return null; // BookedTicketId not found
            }

            List<object> updatedResults = new List<object>();

            foreach (var update in updatedTickets)
            {
                var ticketDetail = existingData.BookedTicketDetails.FirstOrDefault(td => td.TicketCode == update.TicketCode);
                if (ticketDetail == null)
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

                ticketDetail.TicketQuantity = update.Quantity;
                ticketDetail.TicketQuantity = update.Quantity;
                ticketDetail.UpdatedAt = DateTime.UtcNow;
                ticketDetail.UpdatedBy = username;

                _db.BookedTicketDetails.Update(ticketDetail);


                updatedResults.Add(new
                {
                    TicketCode = ticketDetail.TicketCode,
                    TicketName = ticketDetail.TicketCodeNavigation.TicketName,
                    CategoryName = ticketDetail.TicketCodeNavigation.Category.CategoryName,
                    Quantity = ticketDetail.TicketQuantity
                });
            }

            await _db.SaveChangesAsync();
            return updatedResults;
        }
    }
}
