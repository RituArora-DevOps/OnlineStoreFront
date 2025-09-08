using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.ViewModels;
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
    private readonly ReviewService _reviewService;

    public CustomerController(
        UserManager<IdentityUser> userManager,
        IOrderRepository orderRepository,
        IGenericRepository<ProductReview> reviewRepository,
        ReviewService reviewService)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _reviewRepository = reviewRepository;
        _reviewService = reviewService;
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

    public async Task<IActionResult> Orders()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Index", "Home");
        }

        // Get orders for this user
        var userOrders = await _orderRepository.GetOrdersForUserAsync(userId);
        
        // Convert to ViewModel
        var orderHistoryVMs = userOrders.Select(order => new OrderHistoryVM
        {
            OrderId = order.OrderId,
            ExternalUserId = order.ExternalUserId,
            CreatedDate = order.CreatedDate,
            ModifiedDate = order.ModifiedDate,
            OrderStatus = order.OrderStatus?.Status ?? "Unknown",
            Total = order.OrderItems?.Sum(oi => oi.Quantity * oi.PriceAtOrder) ?? 0,
            OrderItems = order.OrderItems?.Select(oi => new OrderItemVM
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown Product",
                Quantity = oi.Quantity,
                PriceAtOrder = oi.PriceAtOrder,
                HasImage = oi.Product?.Pictures?.Any() ?? false
            }).ToList() ?? new List<OrderItemVM>()
        }).ToList();

        return View(orderHistoryVMs);
    }

    public async Task<IActionResult> OrderDetails(long id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Index", "Home");
        }

        // Get the order with all related data
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        if (order == null || order.ExternalUserId != userId)
        {
            TempData["ErrorMessage"] = "Order not found or access denied.";
            return RedirectToAction("Orders");
        }

        // Convert to ViewModel
        var orderHistoryVM = new OrderHistoryVM
        {
            OrderId = order.OrderId,
            ExternalUserId = order.ExternalUserId,
            CreatedDate = order.CreatedDate,
            ModifiedDate = order.ModifiedDate,
            OrderStatus = order.OrderStatus?.Status ?? "Unknown",
            Total = order.OrderItems?.Sum(oi => oi.Quantity * oi.PriceAtOrder) ?? 0,
            OrderItems = order.OrderItems?.Select(oi => new OrderItemVM
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown Product",
                Quantity = oi.Quantity,
                PriceAtOrder = oi.PriceAtOrder,
                HasImage = oi.Product?.Pictures?.Any() ?? false
            }).ToList() ?? new List<OrderItemVM>(),
            Payment = order.Payment
        };

        // Verify user has already reviewed each product (only for delivered orders)
        if (order.OrderStatus?.Status == "Delivered" && orderHistoryVM.OrderItems.Any())
        {
            var reviewedProducts = new Dictionary<int, bool>();
            foreach (var item in orderHistoryVM.OrderItems)
            {
                reviewedProducts[item.ProductId] = await _reviewService.HasUserReviewedProductAsync(userId, item.ProductId);
            }
            ViewBag.ReviewedProducts = reviewedProducts;
        }

        return View(orderHistoryVM);
    }

    public IActionResult Profile()
    {
        // Show and edit customer profile
        return View();
    }
}
