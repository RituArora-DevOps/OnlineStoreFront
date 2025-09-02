using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.Utility;

namespace ECommerceSecureApp.Repository
{
    public interface IProductRepository
    {
        // This method returns all the Product entities as an enumerable collection.
        // pagination parameters added
        Task<PagedResult<Product>> GetAllProductsAsync(int pageNumber, int pageSize);

        // This method retrieves a single Product entity based on its unique identifier.
        Task<Product?> GetProductByIdAsync(int productId);

        // This method retrieves a collection of Product entities that belong to a specific category.
        // pagination parameters added
        Task<PagedResult<Product>> SearchProductsAsync(ProductSearchCriteria criteria, int pageNumber, int PageSize);

        // This method adds a new Product entity to the data source and returns the added entity.
        // This method accepts a Product object as the parameter and adds that Product object to the Product DbSet
        // and marks the entity state as Added
        Task<Product> AddProductAsync(Product product);

        // This method updates an existing Product entity in the data source and returns the updated entity.
        // Also, marks that Product object as amodified Product in the Product DbSet
        Task<Product> UpdateProductAsync(Product product);

        // This method deletes a Product entity from the data source based on its unique identifier.
        // Mark the Entity state as Deleted in the Product DbSet
        Task DeleteProductAsync(int productId);

        // This method save changes to the EFCoreDb database
        Task SaveAsync();


    }
}
