using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;

namespace ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing
{
    public class SeasonalDiscount : DiscountDecorator
    {
        private readonly decimal _seasonalPercentage;
        public SeasonalDiscount(IDiscountable product, decimal seasonalPercentage) : base(product)
        {
            _seasonalPercentage = seasonalPercentage;
        }
        public override decimal GetPrice()
        {
            var basePrice = base.GetPrice();
            var discountAmount = basePrice * _seasonalPercentage / 100;
            return basePrice - discountAmount;
        }
    }
}
