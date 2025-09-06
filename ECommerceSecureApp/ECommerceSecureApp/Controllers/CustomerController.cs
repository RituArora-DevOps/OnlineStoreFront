using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IOrderRepository _orderRepository;
    private readonly IGenericRepository<ProductReview> _reviewRepository;

    public CustomerController(
        UserManager<IdentityUser> userManager,
        IOrderRepository orderRepository,
        IGenericRepository<ProductReview> reviewRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _reviewRepository = reviewRepository;
    }
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Index", "Home");
        }

        // Fetch recent orders for this user
        var userOrders = await _orderRepository.GetOrdersForUserAsync(userId);
        var recentOrders = userOrders.Take(5).ToList();

        ViewBag.RecentOrders = recentOrders;

        // Fetch all reviews by this user
        var reviews = await _reviewRepository.GetAllAsync();
        var myReviews = reviews
            .Where(r => r.ExternalUserId == userId)
            .OrderByDescending(r => r.CreatedDate)
            .ToList();

        ViewBag.MyReviews = myReviews;

        return View();
    }

    public IActionResult Orders()
    {
        // Show customer's order history
        return View();
    }

    public IActionResult Profile()
    {
        // Show and edit customer profile
        return View();
    }
}
