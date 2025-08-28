using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Cart
{
    public int CartId { get; set; }

    public string? ExternalUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
