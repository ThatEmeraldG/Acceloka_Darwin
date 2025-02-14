using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace Acceloka.Models
{
    public class BookedTicketModel
    {
        [Key]
        public int BookedTicketId { get; set; }

        [Required]
        public int TransactionId { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int TotalCategoryPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        // Foreign Key Relationship
        [ForeignKey("TransactionId")]
        public System.Transactions.Transaction? Transaction { get; set; }

        public ICollection<BookedTicketDetailsModel>? BookedTicketDetails { get; set; }
    }
}
