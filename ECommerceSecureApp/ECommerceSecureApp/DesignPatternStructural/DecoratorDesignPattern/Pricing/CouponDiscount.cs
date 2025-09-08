using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;

namespace ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing
{
    public class CouponDiscount : DiscountDecorator
    {
        private readonly decimal _couponAmount;
        public CouponDiscount(IDiscountable product, decimal couponAmount) : base(product)
        {
            _couponAmount = couponAmount;
        }
        public override decimal GetPrice()
        {
            var basePrice = base.GetPrice();
            var discountedPrice = basePrice - _couponAmount;
            return discountedPrice < 0 ? 0 : discountedPrice; // Prevent negative pricing
        }
    }
}
