using ECommerceSecureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(OnlineStoreDbContext context) : base(context) { }

        public async Task<Cart> GetOrCreateCartAsync(string? externalUserId, int? sessionCartId = null)
        {
            Cart? cart = null;

            if (!string.IsNullOrWhiteSpace(externalUserId))
            {
                cart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.ExternalUserId == externalUserId);

                if (cart == null && sessionCartId.HasValue)
                {
                    cart = await _context.Carts
                        .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.CartId == sessionCartId.Value);

                    if (cart != null)
                    {
                        cart.ExternalUserId = externalUserId;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            if (cart == null)
            {
                if (sessionCartId.HasValue)
                {
                    cart = await _context.Carts
                        .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.CartId == sessionCartId.Value);
                }

                if (cart == null)
                {
                    cart = new Cart
                    {
                        ExternalUserId = externalUserId,
                        CreatedDate = DateTime.UtcNow
                    };
                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }
            }

            return cart;
        }

        public Task<Cart?> GetCartWithItemsAsync(int cartId) =>
            _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId)!;

        public Task<List<CartItem>> GetItemsAsync(int cartId) =>
            _context.CartItems.Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .OrderBy(ci => ci.CartItemId)
                .ToListAsync();

        public async Task AddOrUpdateItemAsync(int cartId, int productId, int quantity)
        {
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (existing == null)
            {
                var product = await _context.Products.FindAsync(productId)
                    ?? throw new InvalidOperationException("Product not found.");

                var item = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = Math.Max(1, quantity),
                    CreatedDate = DateTime.UtcNow
                };
                await _context.CartItems.AddAsync(item);
            }
            else
            {
                existing.Quantity = Math.Max(1, existing.Quantity + quantity);
                existing.ModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId)
                ?? throw new InvalidOperationException("Cart item not found.");

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
                item.ModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearAsync(int cartId)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cartId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public Task<decimal> GetCartTotalAsync(int cartId) =>
            _context.CartItems.Where(ci => ci.CartId == cartId)
                .Join(_context.Products, ci => ci.ProductId, p => p.ProductId,
                    (ci, p) => p.Price * ci.Quantity)
                .SumAsync();
    }
}
