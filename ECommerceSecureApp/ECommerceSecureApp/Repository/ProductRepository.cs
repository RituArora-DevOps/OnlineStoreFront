using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.Utility;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        // Constructor to initialize the OlineStoreDBContext instance via Dependency Injection
        // Initializing the OnlineStoreDbContext instance which it received as an argument
        // MVC Framework DI Container will inject the OnlineStoreDbContext instance
        public ProductRepository(OnlineStoreDbContext context) : base(context) { }

        // Returns all employees from the database
        public async Task<PagedResult<Product>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Products.CountAsync();
            var products = await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
                {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Retrieves products by different criteries
        public async Task<PagedResult<Product>> SearchProductsAsync(ProductSearchCriteria criteria, int pageNumber, int pageSize)
        {
            IQueryable<Product> query = _context.Products;
            // List of strategies to apply
            List<IProductSearchStrategy> strategies = new List<IProductSearchStrategy>
            {
                new NameSearchStrategy(),
                new PriceSearchStrategy(),
                new CategorySearchStrategy()
            };
            // Apply each strategy
            foreach (var strategy in strategies)
            {
                query = strategy.Apply(query, criteria);
            }
            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }

    }
}
