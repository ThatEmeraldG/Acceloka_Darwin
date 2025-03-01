using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class BookedTicketDetailsModel
    {
        [Required]
        public int BookedDetailId { get; set; }

        [Required]
        public int BookedTicketId { get; set; }

        [StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;

        public int TicketQuantity { get; set; }

        public int TotalTicketPrice { get; set; }

        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
