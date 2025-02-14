using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class BookedTicketRequest
    {
        [Required]
        public string TicketCode { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string UserName { get; set; }
    }
}
