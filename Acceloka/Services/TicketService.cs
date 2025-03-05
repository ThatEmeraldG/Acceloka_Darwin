using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<TicketService> _logger;

        public TicketService(AccelokaContext db, ILogger<TicketService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET List Tickets
        public async Task<PaginationResponse<TicketModel>> GetTickets(GetTicketRequest request)
        {
            _logger.LogInformation("Fetching all available tickets...");

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
                "eventdate" => request.OrderDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.EventStart)
                    : query.OrderBy(t => t.EventStart),
                _ => request.OrderDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.TicketCode)
                    : query.OrderBy(t => t.TicketCode)
            };

            var totalTickets = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalTickets / (double)request.PageSize);

            var tickets = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(Q => new TicketModel
                {
                    TicketCode = Q.TicketCode,
                    TicketName = Q.TicketName,
                    CategoryId = Q.CategoryId,
                    CategoryName = Q.Category.CategoryName,
                    Price = Q.Price,
                    Quota = Q.Quota,
                    EventStart = Q.EventStart,
                    EventEnd = Q.EventEnd,
                    CreatedAt = Q.CreatedAt,
                    CreatedBy = Q.CreatedBy,
                    UpdatedAt = Q.UpdatedAt,
                    UpdatedBy = Q.UpdatedBy
                }).ToListAsync();

            _logger.LogInformation("Successfully fetched {TicketCount} tickets from page {PageNumber}", tickets.Count, request.PageNumber);

            return new PaginationResponse<TicketModel>
            {
                Data = tickets,
                TotalRecords = totalTickets,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                TotalPages = totalPages
            };
        }

        // POST new Ticket
        public async Task<string> PostTicket(CreateTicketRequest request, string? username)
        {
            var ticketCategory = await _db.Categories
                                .FirstOrDefaultAsync(category => category.CategoryName == request.CategoryName);

            if (ticketCategory == null)
            {
                _logger.LogWarning("Category not found: {CategoryName}", request.CategoryName);
                return null;
            }

            var categoryId = ticketCategory.CategoryId;

            var newData = new Ticket
            {
                TicketCode = request.TicketCode,
                TicketName = request.TicketName,
                Price = request.Price,
                Quota = request.Quota,
                EventStart = request.EventStart,
                EventEnd = request.EventEnd,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = string.IsNullOrEmpty(username) ? "System" : username,
            };

            _db.Tickets.Add(newData);

            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully created new ticket: {TicketCode} - {TicketName}", request.TicketCode, request.TicketName);

            return "Success";
        }

        // DELETE ticket
        public async Task<string> DeleteTicket(string ticketCode)
        {
            var data = await _db.Tickets.FindAsync(ticketCode);

            if (data == null)
            {
                _logger.LogWarning("Ticket not found: {TicketCode}", ticketCode);
                return "Ticket not found";
            }

            try
            {
                _db.Tickets.Remove(data);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted ticket: {TicketCode}", ticketCode);
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to delete ticket: {TicketCode}", ticketCode);
                return $"Failed to delete ticket: {ex.Message}";
            }
        }
    }
}
