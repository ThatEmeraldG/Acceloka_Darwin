using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class CreateTicketRequest
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MinValue;
        public int Price { get; set; }
        public int Quota { get; set; }
    }
}
