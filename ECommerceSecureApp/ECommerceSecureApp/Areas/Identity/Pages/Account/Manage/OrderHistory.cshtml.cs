using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.ViewModels;
using ECommerceSecureApp.Repository;
using System.Security.Claims;

namespace ECommerceSecureApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class OrderHistoryModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderHistoryModel> _logger;

        public OrderHistoryModel(IOrderRepository orderRepository, ILogger<OrderHistoryModel> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public List<OrderHistoryVM> Orders { get; set; } = new List<OrderHistoryVM>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToPage("/Account/Login");
                }

                var userOrders = await _orderRepository.GetOrdersForUserAsync(userId);
                
                Orders = userOrders.Select(order => new OrderHistoryVM
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
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order history for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                TempData["ErrorMessage"] = "An error occurred while retrieving your order history.";
                Orders = new List<OrderHistoryVM>();
                return Page();
            }
        }
    }
}
