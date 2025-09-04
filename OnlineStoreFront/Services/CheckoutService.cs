using Microsoft.EntityFrameworkCore;
using OnlineStoreFront.Models.Business;
using OnlineStoreFront.Models.ViewModels;

namespace OnlineStoreFront.Services;

// Creates Order + OrderItems and clears the cart (transactional)
public class CheckoutService : ICheckoutService
{
    private readonly OnlineStoreContext _db;
    public CheckoutService(OnlineStoreContext db) => _db = db;

    public async Task<CartVM> GetSummaryAsync(string userId)
    {
        // Delegate to cart projection
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.ExternalUserId == userId);

        var vm = new CartVM();
        if (cart == null) return vm;

        vm.Items = cart.CartItems.Select(ci => new CartItemVM
        {
            CartItemId = ci.CartItemId,
            ProductId = ci.ProductId,
            Name = ci.Product.Name,
            UnitPrice = ci.Product.Price,
            Quantity = ci.Quantity
        }).ToList();

        return vm;
    }

    public async Task<long> PlaceOrderAsync(string userId, CheckoutAddressVM addr)
    {
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.ExternalUserId == userId)
            ?? throw new InvalidOperationException("Cart not found.");

        using var tx = await _db.Database.BeginTransactionAsync();

        var order = new Order
        {
            ExternalUserId = userId,
            OrderStatusId = 1,                 // Pending
            CreatedDate = DateTime.UtcNow
            // Optional: if schema has address columns, copy from addr here
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        foreach (var ci in cart.CartItems)
        {
            _db.OrderItems.Add(new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                PriceAtOrder = ci.Product.Price
            });
            // Optional: decrement inventory here if we want to track stock
        }

        _db.CartItems.RemoveRange(cart.CartItems);
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
        return order.OrderId;
    }
}
