using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Product
{
    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Clothing? Clothing { get; set; }

    public virtual Electronic? Electronic { get; set; }

    public virtual Grocery? Grocery { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}
