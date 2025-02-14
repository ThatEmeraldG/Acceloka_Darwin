using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class TicketModel
    {
        [StringLength(10)]
        public string TicketCode { get; set; } = string.Empty;

        [StringLength(255)]
        public string TicketName { get; set; } = string.Empty;

        public int CategoryId { get; set; } = 1;

        public string CategoryName { get; set; } = string.Empty;

        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MinValue;

        public int Quota { get; set; } = 0;
        public int Price { get; set; } = 0;
            
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToLocalTime();
        public DateTime? UpdatedAt { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(50)]
        public string UpdatedBy { get; set; } = string.Empty ;

        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = string.Empty; // ASC or DESC
    }
}
