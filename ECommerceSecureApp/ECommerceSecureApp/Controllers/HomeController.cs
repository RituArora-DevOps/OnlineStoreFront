using System.Diagnostics;
using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.MinimalApi;

namespace ECommerceSecureApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        // Storefron (Product Browsing)
        public async Task<IActionResult> Index(string name, string category, decimal minPrice, decimal maxPrice, int page=1)
        {
            int pageSize = 10;

            var criteria = new ProductSearchCriteria
            {
                Name = name,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var pagedProducts = await _productRepository.SearchProductsAsync(criteria, page, pageSize);

            var viewModel = new ProductCatalogViewModel
            {
                PagedProducts = pagedProducts,
                Name = name,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
    
            return View(viewModel);
        }

    }
}
