using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual CreditCardPayment? CreditCardPayment { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PayPalPayment? PayPalPayment { get; set; }
}
