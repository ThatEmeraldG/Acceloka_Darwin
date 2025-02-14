namespace Acceloka.Models
{
    public class BookedTicketRequest
    {
        public string TicketCode { get; set; }
        public int Quantity { get; set; }
        public string UserName { get; set; }
    }
}
