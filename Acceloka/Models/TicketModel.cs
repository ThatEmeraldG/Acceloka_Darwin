using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class TicketModel
    {
        [StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;

        [StringLength(255)]
        public string TicketName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int Price { get; set; }
        public int Quota { get; set; }

        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MinValue;
            
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }
        
        [StringLength(50)]
        public string UpdatedBy { get; set; } = string.Empty ;
    }
}
