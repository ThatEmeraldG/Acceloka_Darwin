using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class TicketModel
    {
        [Key]
        [StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TicketName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public DateTime EventStart { get; set; }

        [Required]
        public DateTime EventEnd { get; set; }

        [Required]
        public int Quota { get; set; }

        [Required]
        public int Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(50)]
        public string UpdatedBy { get; set; } = string.Empty ;

        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = string.Empty; // ASC or DESC
    }
}
