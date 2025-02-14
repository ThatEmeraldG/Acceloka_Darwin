using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;
        public TicketService(AccelokaContext db)
        {
            _db = db;
        }
        
        // GET List Tickets
        public async Task<List<TicketModel>> Get()
        {
            var datas = await _db.Tickets
                .Select(Q => new TicketModel
                {
                    TicketCode = Q.TicketCode,
                    TicketName = Q.TicketName,
                    CategoryId = Q.CategoryId,
                    CategoryName = Q.CategoryId,
                    EventStart = Q.EventStart,
                    EventEnd = Q.EventEnd,
                    CreatedAt = Q.CreatedAt,
                    CreatedBy = Q.CreatedBy,
                    UpdatedAt = Q.UpdatedAt,
                    UpdatedBy = Q.UpdatedBy
                }).ToListAsync();

            return datas;
        }

        // POST new Ticket
        public async Task<string> Post(TicketModel request)
        {
            var newData = new Ticket
            {
                TicketCode = request.TicketCode,
                TicketName = request.TicketName,
                Price = request.Price,
                Quota = request.Quota,
                EventStart = request.EventStart,
                EventEnd = request.EventEnd,
                CategoryId = request.CategoryId,
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
