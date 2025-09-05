namespace ECommerceSecureApp.Models.ViewModels
{
    public class CartItemVM
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal OriginalUnitPrice { get; set; }
        public decimal DiscountedUnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal OriginalLineTotal => OriginalUnitPrice * Quantity;
        public decimal DiscountedLineTotal => DiscountedUnitPrice * Quantity;
        public decimal LineDiscount => OriginalLineTotal - DiscountedLineTotal;
    }
}
