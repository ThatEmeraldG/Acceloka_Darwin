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
    }
}
