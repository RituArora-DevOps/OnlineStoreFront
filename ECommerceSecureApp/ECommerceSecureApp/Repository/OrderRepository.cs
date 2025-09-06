using ECommerceSecureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(OnlineStoreDbContext context) : base(context) { }

        public async Task<Order> CreateOrderFromCartAsync(int cartId, string? externalUserId, int? paymentId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId)
                ?? throw new InvalidOperationException("Cart not found.");

            if (!cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var status = await _context.OrderStatuses
                .OrderBy(os => os.OrderStatusId)
                .FirstOrDefaultAsync(os => os.Status == "Pending")
                ?? await _context.OrderStatuses.OrderBy(os => os.OrderStatusId).FirstAsync(); // OrderStatus

            var order = new Order
            {
                ExternalUserId = externalUserId,
                PaymentId = paymentId, 
                OrderStatusId = status.OrderStatusId,
                CreatedDate = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtOrder = ci.Product.Price,
                    CreatedDate = DateTime.UtcNow
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return order;
        }

        public Task<Order?> GetOrderWithItemsAsync(long orderId) =>
            _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.OrderStatus)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId)!;

        public Task<List<Order>> GetOrdersForUserAsync(string externalUserId) =>
            _context.Orders.Include(o => o.OrderStatus)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Where(o => o.ExternalUserId == externalUserId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
    }
}
