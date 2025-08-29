using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Payments", Schema = "Payment")]
public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Payment")]
    public virtual CreditCardPayment? CreditCardPayment { get; set; }

    [InverseProperty("Payment")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Payment")]
    public virtual PayPalPayment? PayPalPayment { get; set; }
}
