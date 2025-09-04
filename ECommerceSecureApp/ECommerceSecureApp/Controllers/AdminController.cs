using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSecureApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<ProductReview> _reviewRepo;

        public AdminController(
            IProductRepository productRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<ProductReview> reviewRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _reviewRepo = reviewRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalProducts = (await _productRepo.GetAllAsync()).Count();
            ViewBag.TotalOrders = (await _orderRepo.GetAllAsync()).Count();

            var reviews = await _reviewRepo.GetAllAsync();
            ViewBag.TotalReviews = reviews.Count();

            var recentOrders = (await _orderRepo.GetAllAsync())
                                .OrderByDescending(o => o.CreatedDate)
                                .Take(5)
                                .ToList();

            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public IActionResult ManageProducts()
        {
            // Return a view with product management tools
            return View();
        }

        // Add actions like CreateProduct, EditProduct, DeleteProduct
    }
}
