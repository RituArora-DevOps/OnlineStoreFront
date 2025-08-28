using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Electronic
{
    public int ProductId { get; set; }

    public int? WarrantyPeriod { get; set; }

    public virtual Product Product { get; set; } = null!;
}
