using ECommerceSecureApp.Models;
using ECommerceSecureApp.Services.Coupons;

namespace ECommerceSecureApp.Services.Pricing
{
    public interface IPriceCalculator
    {
        // existing
        decimal GetDiscountedUnitPrice(Product product, decimal couponAmount, decimal seasonalPercentage = 0);

        // new overload
        decimal GetDiscountedUnitPrice(Product product, CouponInfo? coupon, decimal seasonalPercentage = 0);
    }
}
