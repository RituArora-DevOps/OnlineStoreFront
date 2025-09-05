using ECommerceSecureApp.Models;
using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;
using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing;

namespace ECommerceSecureApp.Services.Pricing
{
    public class PriceCalculator : IPriceCalculator
    {
        public decimal GetDiscountedUnitPrice(Product product, decimal couponAmount, decimal seasonalPercentage = 0)
        {
            // Start with the base product
            IDiscountable comp = new BaseProduct(product);

            // Seasonal discount (currently unused since Product has no field)
            if (seasonalPercentage > 0)
            {
                comp = new SeasonalDiscount(comp, seasonalPercentage);
            }

            // Apply coupon discount if > 0
            if (couponAmount > 0)
            {
                comp = new CouponDiscount(comp, couponAmount);
            }

            return comp.GetPrice();
        }
    }
}
