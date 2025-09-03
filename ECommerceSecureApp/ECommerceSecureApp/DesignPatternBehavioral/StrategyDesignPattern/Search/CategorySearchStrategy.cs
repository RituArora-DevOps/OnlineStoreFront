using ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search
{
    public class CategorySearchStrategy : IProductSearchStrategy
    {
        public IQueryable<Product> Apply(IQueryable<Product> products, ProductSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Category))
            {
                products = products.Where(p => p.Category == criteria.Category);
            }
            return products;
        }
    }
}
