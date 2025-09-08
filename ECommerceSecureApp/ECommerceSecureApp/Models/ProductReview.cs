using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("ProductReviews", Schema = "Product")]
[Index("ExternalUserId", Name = "IX_ProductReviews_ExternalUserId")]
[Index("ProductId", Name = "IX_ProductReviews_ProductId")]
public partial class ProductReview
{
    [Key]
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    [StringLength(128)]
    public string? ExternalUserId { get; set; }

    public int? Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product? Product { get; set; }
}
