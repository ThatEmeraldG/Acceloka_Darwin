using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acceloka.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        public int TotalPayment { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        public ICollection<BookedTicket>? BookedTickets { get; set; }
    }
}
