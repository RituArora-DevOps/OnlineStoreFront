using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces
{
    public interface IProductSearchStrategy
    {
        IQueryable<Product> Apply(IQueryable<Product> products, ProductSearchCriteria criteria);
    }

}
