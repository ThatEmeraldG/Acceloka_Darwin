using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class CreateTicketRequest
    {
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public string TicketName { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public DateTime EventStart { get; set; } = DateTime.MinValue;
        [Required]
        public DateTime EventEnd { get; set; } = DateTime.MinValue;
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quota { get; set; }
    }
}
