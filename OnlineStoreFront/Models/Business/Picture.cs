using System;
using System.Collections.Generic;

namespace OnlineStoreFront.Models.Business;

public partial class Picture
{
    public int PictureId { get; set; }

    public int ProductId { get; set; }

    public string PictureName { get; set; } = null!;

    public string? PicFileName { get; set; }

    public byte[]? PictureData { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
