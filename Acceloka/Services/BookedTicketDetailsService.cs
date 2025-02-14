using Acceloka.Entities;

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

    }
}
