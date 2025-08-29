using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("PayPalPayments", Schema = "Payment")]
public partial class PayPalPayment
{
    [Key]
    public int PaymentId { get; set; }

    [StringLength(100)]
    public string? PayPalEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("PayPalPayment")]
    public virtual Payment Payment { get; set; } = null!;
}
