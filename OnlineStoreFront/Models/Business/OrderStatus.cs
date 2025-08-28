using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
