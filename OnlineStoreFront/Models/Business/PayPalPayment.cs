using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class PayPalPayment
{
    public int PaymentId { get; set; }

    public string? PayPalEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
