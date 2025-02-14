using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<object> Post(List<BookedTicketRequest> requests)
        {
            if (!requests.Any())
                return new { Message = "No tickets available" };

            try
            {
                var username = requests.Select(r => r.UserName)
                                  .FirstOrDefault(u => !string.IsNullOrEmpty(u)) ?? "System";

                var bookingDate = DateTime.UtcNow;
                var bookedTicket = new BookedTicket
                {
                    BookingDate = bookingDate,
                    CreatedAt = bookingDate,
                    CreatedBy = username,
                };

                _db.Add(bookedTicket);
                await _db.SaveChangesAsync();

                var categorySummaries = new Dictionary<string, CategorySummary>();
                var totalAllCategories = 0;

                foreach (var request in requests)
                {
                    var ticket = await _db.Tickets
                        .Include(t => t.Category)
                        .FirstOrDefaultAsync(t => t.TicketCode == request.TicketCode);

                    if (ticket == null)
                    {
                        throw new Exception($"Ticket {request.TicketCode} is not available");
                    }

                    if (ticket.Quota <= 0)
                    {
                        throw new Exception($"No Quota left for Ticket {request.TicketCode}, it is sold out.");
                    }

                    if (request.Quantity <= 0)
                    {
                        throw new Exception($"Quantity must be at least 1.");
                    }

                    if (ticket.Quota < request.Quantity)
                    {
                        throw new Exception($"Item quantity must not exceed remaining quota. Available: {ticket.Quota}.");
                    }

                    if (ticket.EventEnd <= bookingDate)
                    {
                        throw new Exception($"Event for {ticket.TicketCode} has ended.");
                    }

                    ticket.Quota -= request.Quantity;
                    _db.Update(ticket);

                    var totalPrice = ticket.Price * request.Quantity;
                    totalAllCategories += totalPrice;

                    // Per Categories
                    var categoryName = ticket.Category.CategoryName;
                    if (!categorySummaries.TryGetValue(categoryName, out var categorySummary))
                    {
                        categorySummary = new CategorySummary
                        {
                            CategoryName = categoryName,
                            SummaryPrice = 0,
                            Tickets = new List<TicketInfo>()
                        };
                        categorySummaries[categoryName] = categorySummary;
                    }

                    categorySummary.SummaryPrice += totalPrice;
                    categorySummary.Tickets.Add(new TicketInfo
                    {
                        TicketCode = ticket.TicketCode,
                        TicketName = ticket.TicketName,
                        Price = ticket.Price,
                        Quantity = request.Quantity
                    });

                    _db.BookedTicketDetails.Add(new BookedTicketDetail
                    {
                        BookedTicketId = bookedTicket.BookedTicketId,
                        TicketCode = ticket.TicketCode,
                        TicketQuantity = request.Quantity,
                        TotalTicketPrice = totalPrice,
                        CreatedAt = bookingDate,
                        CreatedBy = username
                    });
                }

                await _db.SaveChangesAsync();

                return new
                {
                    pricesSummary = totalAllCategories,
                    ticketsPerCategories = categorySummaries.Values.Select(cs => new
                    {
                        categoryName = cs.CategoryName,
                        summaryPrice = cs.SummaryPrice,
                        tickets = cs.Tickets.Select(t => new
                        {
                            ticketCode = t.TicketCode,
                            ticketName = t.TicketName,
                            price = t.Price,
                            quantity = t.Quantity
                        })
                    })
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Error = ex.Message,
                    Success = false
                };
            }
        }

        private class CategorySummary
        {
            public string CategoryName { get; set; }
            public int SummaryPrice { get; set; }
            public List<TicketInfo> Tickets { get; set; }
        }

        private class TicketInfo
        {
            public string TicketCode { get; set; }
            public string TicketName { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
