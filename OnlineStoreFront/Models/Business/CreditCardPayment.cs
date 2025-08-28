using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class CreditCardPayment
{
    public int PaymentId { get; set; }

    public string Last4 { get; set; } = null!;

    public string? CardBrand { get; set; }

    public byte? ExpirationMonth { get; set; }

    public short? ExpirationYear { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
