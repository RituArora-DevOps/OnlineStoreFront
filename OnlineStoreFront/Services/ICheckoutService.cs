using OnlineStoreFront.Models.ViewModels;

namespace OnlineStoreFront.Services;

public interface ICheckoutService
{
    Task<CartVM> GetSummaryAsync(string userId);
    Task<long> PlaceOrderAsync(string userId, CheckoutAddressVM addr);
}
