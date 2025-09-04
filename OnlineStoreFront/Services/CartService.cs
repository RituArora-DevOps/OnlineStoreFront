using Microsoft.EntityFrameworkCore;
using OnlineStoreFront.Models.Business;
using OnlineStoreFront.Models.ViewModels;

namespace OnlineStoreFront.Services;

// Business logic for reading/updating the cart in OnlineStore DB
public class CartService : ICartService
{
    private readonly OnlineStoreContext _db;

    public CartService(OnlineStoreContext db) => _db = db;

    public async Task<int> GetOrCreateCartIdAsync(string? userId, string? guestId)
    {
        var key = userId ?? guestId!;
        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.ExternalUserId == key);
        if (cart == null)
        {
            cart = new Cart { ExternalUserId = key, CreatedDate = DateTime.UtcNow };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
        }
        return cart.CartId;
    }

    public async Task AddAsync(int productId, int qty, string? userId, string? guestId)
    {
        var cartId = await GetOrCreateCartIdAsync(userId, guestId);
        var item = await _db.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        if (item == null)
            _db.CartItems.Add(new CartItem { CartId = cartId, ProductId = productId, Quantity = Math.Max(1, qty), CreatedDate = DateTime.UtcNow });
        else
            item.Quantity = Math.Min(99, item.Quantity + Math.Max(1, qty));
        await _db.SaveChangesAsync();
    }

    public async Task<CartVM> GetCartAsync(string? userId, string? guestId)
    {
        var vm = new CartVM();
        var key = userId ?? guestId!;
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.ExternalUserId == key);

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

    public async Task UpdateQtyAsync(int cartItemId, int qty, string? userId, string? guestId)
    {
        var item = await _db.CartItems.FindAsync(cartItemId);
        if (item == null) return;
        item.Quantity = Math.Max(1, Math.Min(qty, 99));
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int cartItemId, string? userId, string? guestId)
    {
        var item = await _db.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ClearAsync(string? userId, string? guestId)
    {
        var key = userId ?? guestId!;
        var cart = await _db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.ExternalUserId == key);
        if (cart?.CartItems?.Any() == true)
        {
            _db.CartItems.RemoveRange(cart.CartItems);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MergeAsync(string guestId, string userId)
    {
        if (guestId == userId) return;
        var guest = await _db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.ExternalUserId == guestId);
        var user = await _db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.ExternalUserId == userId);

        if (guest == null) return;
        if (user == null) { guest.ExternalUserId = userId; await _db.SaveChangesAsync(); return; }

        foreach (var gi in guest.CartItems)
        {
            var ui = user.CartItems.FirstOrDefault(x => x.ProductId == gi.ProductId);
            if (ui == null)
                user.CartItems.Add(new CartItem { ProductId = gi.ProductId, Quantity = gi.Quantity, CreatedDate = DateTime.UtcNow });
            else
                ui.Quantity = Math.Min(99, ui.Quantity + gi.Quantity);
        }

        _db.Carts.Remove(guest);
        await _db.SaveChangesAsync();
    }
}
