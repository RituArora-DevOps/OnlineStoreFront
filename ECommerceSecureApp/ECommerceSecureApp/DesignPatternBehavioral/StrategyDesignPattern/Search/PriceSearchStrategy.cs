using ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search
{
    public class PriceSearchStrategy : IProductSearchStrategy
    {
        public IQueryable<Product> Apply(IQueryable<Product> products, ProductSearchCriteria criteria)
        {
            if (criteria.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= criteria.MinPrice.Value);
            }
            if (criteria.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= criteria.MaxPrice.Value);
            }
            return products;
        }
    }
}
