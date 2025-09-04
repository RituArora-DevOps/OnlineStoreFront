namespace OnlineStoreFront.Models.ViewModels;

public class CartItemVM
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
