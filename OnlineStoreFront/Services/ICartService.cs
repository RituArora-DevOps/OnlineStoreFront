using OnlineStoreFront.Models.ViewModels;

namespace OnlineStoreFront.Services;

public interface ICartService
{
    Task<int> GetOrCreateCartIdAsync(string? userId, string? guestId);
    Task<CartVM> GetCartAsync(string? userId, string? guestId);
    Task AddAsync(int productId, int qty, string? userId, string? guestId);
    Task UpdateQtyAsync(int cartItemId, int qty, string? userId, string? guestId);
    Task RemoveAsync(int cartItemId, string? userId, string? guestId);
    Task ClearAsync(string? userId, string? guestId);
    Task MergeAsync(string guestId, string userId);
}
