using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Clothing", Schema = "Product")]
public partial class Clothing
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Clothing")]
    public virtual Product Product { get; set; } = null!;
}
