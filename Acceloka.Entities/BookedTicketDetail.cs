using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class BookedTicketDetail
{
    public int BookedDetailId { get; set; }

    public int BookedTicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public int TicketQuantity { get; set; }

    public int TotalTicketPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public virtual BookedTicket BookedTicket { get; set; } = null!;

    public virtual Ticket TicketCodeNavigation { get; set; } = null!;
}
