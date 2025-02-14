using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class BookedTicket
{
    public int BookedTicketId { get; set; }

    public int TransactionId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int TotalCategoryPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public virtual ICollection<BookedTicketDetail> BookedTicketDetails { get; set; } = new List<BookedTicketDetail>();

    public virtual Transaction Transaction { get; set; } = null!;
}
