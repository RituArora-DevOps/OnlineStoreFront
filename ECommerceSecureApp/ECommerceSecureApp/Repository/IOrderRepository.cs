using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Repository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order> CreateOrderFromCartAsync(int cartId, string? externalUserId, int? paymentId = null);
        Task<Order?> GetOrderWithItemsAsync(long orderId);
        Task<List<Order>> GetOrdersForUserAsync(string externalUserId);
    }
}
