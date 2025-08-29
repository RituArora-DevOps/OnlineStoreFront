using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("CreditCardPayments", Schema = "Payment")]
public partial class CreditCardPayment
{
    [Key]
    public int PaymentId { get; set; }

    [StringLength(4)]
    public string Last4 { get; set; } = null!;

    [StringLength(20)]
    public string? CardBrand { get; set; }

    public byte? ExpirationMonth { get; set; }

    public short? ExpirationYear { get; set; }

    public DateTime CreatedDate { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("CreditCardPayment")]
    public virtual Payment Payment { get; set; } = null!;
}
