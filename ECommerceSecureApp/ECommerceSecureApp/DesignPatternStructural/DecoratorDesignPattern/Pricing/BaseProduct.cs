using ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Interfaces;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.DesignPatternStructural.DecoratorDesignPattern.Pricing
{
    // A wrapper for the base product to implement IDiscountable
    public class BaseProduct : IDiscountable
    {
        public Product Product { get; }

        public BaseProduct(Product product)
        {
            Product = product;
        }

        public virtual decimal GetPrice() => Product.Price;
    }
}
