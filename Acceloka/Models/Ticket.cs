using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acceloka.Models
{
    [Table("Ticket")]
    public class Ticket
    {
        [Key]
        [Column("ticket_code")]
        [StringLength(10)]
        public String TicketCode { get; set; } = string.Empty;

        [Required]
        [Column("ticket_name")]
        [StringLength(255)]
        public string TicketName { get; set; } = string.Empty;

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("event_start")]
        public DateTime EventStart { get; set; }

        [Required]
        [Column("event_end")]
        public DateTime EventEnd { get; set; }

        [Required]
        [Column("quota")]
        public int Quota { get; set; }

        [Required]
        [Column("price")]
        public int Price { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [Column("updated_by")]
        [StringLength(50)]
        public string UpdatedBy { get; set; } = string.Empty ;
    }
}
