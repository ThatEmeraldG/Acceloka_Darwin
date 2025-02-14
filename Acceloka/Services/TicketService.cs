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
        public async Task<List<TicketModel>> Get(TicketModel request)
        {
            var query = _db.Tickets
                .Include(c => c.Category)
                .Where(t => t.Quota > 0);

            if (!string.IsNullOrEmpty(request.TicketCode))
            {
                query = query.Where(t => t.TicketCode.Contains(request.TicketCode));
            }

            if (!string.IsNullOrEmpty(request.TicketName))
            {
                query = query.Where(t => t.TicketName.Contains(request.TicketName));
            }

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                query = query.Where(t => t.Category != null && t.Category.CategoryName.Contains(request.CategoryName));
            }

            if (request.CategoryId != 0)
            {
                query = query.Where(t => t.CategoryId == request.CategoryId);
            }

            if (request.Price != 0)
            {
                query = query.Where(t => t.Price <= request.Price);
            }

            if (request.EventStart != DateTime.MinValue)
            {
                query = query.Where(t => t.EventStart.Date >= request.EventStart.Date);
            }

            if (request.EventEnd != DateTime.MinValue)
            {
                query = query.Where(t => t.EventEnd.Date <= request.EventEnd.Date);
            }

            query = request.OrderBy?.ToLower() switch
            {
                "ticketname" => request.OrderDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.TicketName)
                    : query.OrderBy(t => t.TicketName),
                "price" => request.OrderDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.Price)
                    : query.OrderBy(t => t.Price),
                "eventstart" => request.OrderDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.EventStart)
                    : query.OrderBy(t => t.EventStart),
                // Default Order = ASC
                _ => query.OrderBy(t => t.TicketCode)
            };

            var tickets = await query.Select(Q => new TicketModel
            {
                TicketCode = Q.TicketCode,
                TicketName = Q.TicketName,
                CategoryId = Q.CategoryId,
                CategoryName = Q.Category.CategoryName,
                EventStart = Q.EventStart,
                EventEnd = Q.EventEnd,
                CreatedAt = Q.CreatedAt,
                CreatedBy = Q.CreatedBy,
                UpdatedAt = Q.UpdatedAt,
                UpdatedBy = Q.UpdatedBy
            }).ToListAsync();

            return tickets;
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
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToLocalTime(),
                CreatedBy = "System",
            };

            _db.Add(newData);

            await _db.SaveChangesAsync();

            return "Success";
        }
    }
}
