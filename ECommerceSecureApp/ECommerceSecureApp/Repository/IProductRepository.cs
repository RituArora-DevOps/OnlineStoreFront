using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.Utility;

namespace ECommerceSecureApp.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // This method returns all the Product entities as an enumerable collection.
        // pagination parameters added
        Task<PagedResult<Product>> GetAllProductsAsync(int pageNumber, int pageSize);

        // This method retrieves a collection of Product entities that belong to a specific category.
        // pagination parameters added
        Task<PagedResult<Product>> SearchProductsAsync(ProductSearchCriteria criteria, int pageNumber, int PageSize);
    }
}
