using ECommerceSecureApp.Models;
using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;
using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing;
using ECommerceSecureApp.Services.Coupons;

namespace ECommerceSecureApp.Services.Pricing
{

    public class PriceCalculator : IPriceCalculator
    {
        public decimal GetDiscountedUnitPrice(Product product, decimal couponAmount, decimal seasonalPercentage = 0)
        {
            CouponInfo ci = new() { AmountOff = couponAmount };
            return GetDiscountedUnitPrice(product, ci, seasonalPercentage);
        }

        public decimal GetDiscountedUnitPrice(Product product, CouponInfo? coupon, decimal seasonalPercentage = 0)
        {
            IDiscountable comp = new BaseProduct(product);

            // 1) Seasonal percentage (enable later if you add a product seasonal field)
            if (seasonalPercentage > 0)
                comp = new SeasonalDiscount(comp, seasonalPercentage);

            // 2) Percentage coupon (reuse SeasonalDiscount as a generic percent-off layer)
            if (coupon is not null && coupon.PercentOff > 0)
                comp = new SeasonalDiscount(comp, coupon.PercentOff);

            // 3) Amount coupon
            if (coupon is not null && coupon.AmountOff > 0)
                comp = new CouponDiscount(comp, coupon.AmountOff);

            return comp.GetPrice();
        }
    }
}
