using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.DesignPatternBehavioral.StrategyDesignPattern.Interfaces;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.Utility;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class ProductRepository : IProductRepository
    {
        // The following variable is  going to hold the OnlineStoreDbContext instance
        private readonly OnlineStoreDbContext _context;

        // Constructor to initialize the OlineStoreDBContext instance via Dependency Injection
        // Initializing the OnlineStoreDbContext instance which it received as an argument
        // MVC Framework DI Container will inject the OnlineStoreDbContext instance
        public ProductRepository(OnlineStoreDbContext context) 
        {
            _context = context;
        }

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

        // Retrieves a single product by their Id
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var product = await  _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == productId);

            return product;
        }

        // Retrieves products by category
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

        // Adds a new product to the database
        public async Task<Product> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await SaveAsync();
            return product;
        }

        // Updates an existing product in the database
        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await SaveAsync();
            return product;
        }

        // Deletes a product from the database by their Id
        public async Task DeleteProductAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        // Saves changes to the database
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
