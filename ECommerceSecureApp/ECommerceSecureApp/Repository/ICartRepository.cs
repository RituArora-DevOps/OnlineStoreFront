using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Repository
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart> GetOrCreateCartAsync(string? externalUserId, int? sessionCartId = null);
        Task<Cart?> GetCartWithItemsAsync(int cartId);
        Task<List<CartItem>> GetItemsAsync(int cartId);
        Task AddOrUpdateItemAsync(int cartId, int productId, int quantity);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
        Task RemoveItemAsync(int cartItemId);
        Task ClearAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int cartId);
    }
}
