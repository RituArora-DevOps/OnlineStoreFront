using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.ViewModels;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.Services;
using System.Security.Claims;

namespace ECommerceSecureApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class OrderDetailsModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ReviewService _reviewService;
        private readonly ILogger<OrderDetailsModel> _logger;

        public OrderDetailsModel(IOrderRepository orderRepository, ReviewService reviewService, ILogger<OrderDetailsModel> logger)
        {
            _orderRepository = orderRepository;
            _reviewService = reviewService;
            _logger = logger;
        }

        public OrderHistoryVM Order { get; set; } = new OrderHistoryVM();

        public async Task<IActionResult> OnGetAsync(long id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToPage("/Account/Login");
                }

                var order = await _orderRepository.GetOrderWithItemsAsync(id);
                
                if (order == null || order.ExternalUserId != userId)
                {
                    TempData["ErrorMessage"] = "Order not found or you don't have permission to view this order.";
                    return RedirectToPage("./OrderHistory");
                }

                Order = new OrderHistoryVM
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
                        HasImage = oi.Product?.Pictures?.Any() == true
                    }).ToList() ?? new List<OrderItemVM>(),
                    Payment = order.Payment
                };

                // Verify user has already reviewed each product (only for delivered orders)
                if (order.OrderStatus?.Status == "Delivered" && Order.OrderItems.Any())
                {
                    var reviewedProducts = new Dictionary<int, bool>();
                    foreach (var item in Order.OrderItems)
                    {
                        reviewedProducts[item.ProductId] = await _reviewService.HasUserReviewedProductAsync(userId, item.ProductId);
                    }
                    ViewData["ReviewedProducts"] = reviewedProducts;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for order {OrderId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving order details.";
                return RedirectToPage("./OrderHistory");
            }
        }
    }
}
