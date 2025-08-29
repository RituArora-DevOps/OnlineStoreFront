using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Grocery", Schema = "Product")]
public partial class Grocery
{
    [Key]
    public int ProductId { get; set; }

    public DateOnly ExpirationDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Grocery")]
    public virtual Product Product { get; set; } = null!;
}
