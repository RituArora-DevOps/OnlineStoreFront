using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class ProductReview
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public string? ExternalUserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
