using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Orders", Schema = "Order")]
[Index("ExternalUserId", Name = "IX_Orders_ExternalUserId")]
[Index("OrderStatusId", Name = "IX_Orders_OrderStatusId")]
[Index("PaymentId", Name = "IX_Orders_PaymentId")]
[Index("ExternalUserId", "OrderStatusId", Name = "IX_Orders_User_Status")]
public partial class Order
{
    [Key]
    public long OrderId { get; set; }

    [StringLength(128)]
    public string? ExternalUserId { get; set; }

    public int? PaymentId { get; set; }

    public int OrderStatusId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("OrderStatusId")]
    [InverseProperty("Orders")]
    public virtual OrderStatus OrderStatus { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("Orders")]
    public virtual Payment? Payment { get; set; }
}
