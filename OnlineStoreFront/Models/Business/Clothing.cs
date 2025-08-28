using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Clothing
{
    public int ProductId { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public virtual Product Product { get; set; } = null!;
}
