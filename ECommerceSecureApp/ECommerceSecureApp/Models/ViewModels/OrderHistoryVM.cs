using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Models.ViewModels
{
    public class OrderHistoryVM
    {
        public long OrderId { get; set; }
        public string? ExternalUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<OrderItemVM> OrderItems { get; set; } = new List<OrderItemVM>();
        public Payment? Payment { get; set; }
    }

    public class OrderItemVM
    {
        public long OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
        public decimal Subtotal => Quantity * PriceAtOrder;
        public bool HasImage { get; set; }
    }
}
