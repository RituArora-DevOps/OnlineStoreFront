using ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search
{
    public class NameSearchStrategy : IProductSearchStrategy
    {
        public IQueryable<Product> Apply(IQueryable<Product> products, ProductSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                products = products.Where(p => p.Name.Contains(criteria.Name));
            }
            return products;
        }

    }
}
