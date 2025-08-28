using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Order
{
    public long OrderId { get; set; }

    public string? ExternalUserId { get; set; }

    public int? PaymentId { get; set; }

    public int OrderStatusId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
