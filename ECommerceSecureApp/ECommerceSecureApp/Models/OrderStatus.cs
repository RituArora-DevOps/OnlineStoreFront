using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("OrderStatus", Schema = "Order")]
[Index("Status", Name = "UQ__OrderSta__3A15923FEC3A231E", IsUnique = true)]
public partial class OrderStatus
{
    [Key]
    public int OrderStatusId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [InverseProperty("OrderStatus")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
