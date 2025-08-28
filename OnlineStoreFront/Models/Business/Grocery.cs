using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Grocery
{
    public int ProductId { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
