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
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<ProductReview> _reviewRepository;

    public CustomerController(
        UserManager<IdentityUser> userManager,
        IGenericRepository<Order> orderRepository,
        IGenericRepository<ProductReview> reviewRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _reviewRepository = reviewRepository;
    }
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);

        // Fetch all orders for this user
        var orders = await _orderRepository.GetAllAsync();
        var userOrders = orders
            .Where(o => o.ExternalUserId == userId)
            .OrderByDescending(o => o.CreatedDate)
            .Take(5)
            .ToList();

        ViewBag.RecentOrders = userOrders;

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
