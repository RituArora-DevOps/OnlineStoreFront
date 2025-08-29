using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Carts", Schema = "Cart")]
[Index("ExternalUserId", Name = "IX_Carts_ExternalUserId")]
public partial class Cart
{
    [Key]
    public int CartId { get; set; }

    [StringLength(128)]
    public string? ExternalUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
