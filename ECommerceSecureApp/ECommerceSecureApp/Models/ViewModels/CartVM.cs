namespace ECommerceSecureApp.Models.ViewModels
{
    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public int CartId { get; set; }
        public string? AppliedCoupon { get; set; }

        public int ItemCount => Items.Sum(i => i.Quantity);
        public decimal SubtotalOriginal => Items.Sum(i => i.OriginalLineTotal);
        public decimal DiscountTotal => Items.Sum(i => i.LineDiscount);
        public decimal Total => SubtotalOriginal - DiscountTotal;
    }
}
