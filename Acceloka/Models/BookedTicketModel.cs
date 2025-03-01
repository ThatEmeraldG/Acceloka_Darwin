using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class BookedTicketModel
    {
        [Key]
        public int BookedTicketId { get; set; }

        public int TransactionId { get; set; }
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public int Price { get; set; }
        public int TotalCategoryPrice { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
