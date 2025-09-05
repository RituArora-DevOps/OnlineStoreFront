using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Services.Pricing
{
    public interface IPriceCalculator
    {
        decimal GetDiscountedUnitPrice(Product product, decimal couponAmount, decimal seasonalPercentage = 0);
    }
}
