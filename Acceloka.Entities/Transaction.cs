using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public int TotalPrice { get; set; }

    public int TotalPayment { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public virtual ICollection<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();
}
