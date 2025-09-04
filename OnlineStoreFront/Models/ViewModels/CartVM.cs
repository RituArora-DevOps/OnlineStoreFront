namespace OnlineStoreFront.Models.ViewModels;

public class CartVM
{
    public List<CartItemVM> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal Tax => Math.Round(Subtotal * 0.149m, 2); // adjust for the locale (I didn't do research)
    public decimal Shipping => Subtotal > 100 ? 0 : 10;
    public decimal Total => Subtotal + Tax + Shipping;
}
