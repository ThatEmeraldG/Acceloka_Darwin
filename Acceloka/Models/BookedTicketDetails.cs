using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Acceloka.Models
{
    [Table("BookedTicketDetail")]
    public class BookedTicketDetails
    {
        [Key]
        public int BookedDetailId { get; set; }

        [Required]
        public int BookedTicketId { get; set; }

        [Required, StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        public int TicketQuantity { get; set; }

        [Required]
        public int TotalTicketPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        [ForeignKey("BookedTicketId")]
        public BookedTicket? BookedTicket { get; set; }

        [ForeignKey("TicketCode")]
        public Ticket? Ticket { get; set; }
    }
}
