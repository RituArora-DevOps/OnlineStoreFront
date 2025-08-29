using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

[Table("Pictures", Schema = "Product")]
public partial class Picture
{
    [Key]
    public int PictureId { get; set; }

    public int ProductId { get; set; }

    [StringLength(40)]
    public string PictureName { get; set; } = null!;

    [StringLength(100)]
    public string? PicFileName { get; set; }

    public byte[]? PictureData { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Pictures")]
    public virtual Product Product { get; set; } = null!;
}
