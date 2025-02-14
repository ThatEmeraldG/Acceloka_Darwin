using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public string TicketName { get; set; } = null!;

    public int CategoryId { get; set; }

    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; } 

    public int Quota { get; set; }

    public int Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public virtual ICollection<BookedTicketDetail> BookedTicketDetails { get; set; } = new List<BookedTicketDetail>();

    public virtual Category Category { get; set; } = null!;
}
