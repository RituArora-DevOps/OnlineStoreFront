namespace ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces
{
    public interface IDiscountable
    {
        decimal GetPrice();

        // Expand this later to include things like IsEligibleForDiscount, ApplyCoupon(string code), etc.
    }
}
