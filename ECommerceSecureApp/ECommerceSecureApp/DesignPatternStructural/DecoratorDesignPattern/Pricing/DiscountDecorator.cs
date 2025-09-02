using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;

namespace ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing
{
    public abstract class DiscountDecorator : IDiscountable
    {
        protected IDiscountable _discountable;
        protected DiscountDecorator(IDiscountable discountable) { 
            _discountable = discountable;
        }

        public virtual decimal GetPrice() => _discountable.GetPrice();

    }
}
