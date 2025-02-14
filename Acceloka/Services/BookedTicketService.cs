using Acceloka.Entities;
using Acceloka.Models;

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
        public async Task<string> Post(TicketModel request)
        {
            var newData = new BookedTicket
            {
                
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System",
            };

            _db.Add(newData);

            await _db.SaveChangesAsync();

            return "Success";
        }
    }
}
