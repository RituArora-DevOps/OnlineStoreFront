using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Electronics", Schema = "Product")]
public partial class Electronic
{
    [Key]
    public int ProductId { get; set; }

    public int? WarrantyPeriod { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Electronic")]
    public virtual Product Product { get; set; } = null!;
}
