namespace ECommerceSecureApp.Models.ViewModels
{
    public class CheckoutVM : CartVM
    {
        // Payment method: "Credit" or "PayPal"
        public string PaymentMethod { get; set; } = "Credit";

        // Credit card fields 
        public string? CardBrand { get; set; }
        public string? CardLast4 { get; set; }
        public byte? ExpMonth { get; set; }
        public short? ExpYear { get; set; }

        // PayPal
        public string? PayPalEmail { get; set; }
    }
}
