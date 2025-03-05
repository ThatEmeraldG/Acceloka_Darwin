using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class GetTicketRequest
    {
        [StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;

        [StringLength(255)]
        public string TicketName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MinValue;
        
        public int Price { get; set; }

        public string OrderBy { get; set; } = "ticketCode";
        public string OrderDirection { get; set; } = "ASC"; // ASC or DESC

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
