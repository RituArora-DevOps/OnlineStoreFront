using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Controllers
{
    public class RefundRequest
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<ProductReview> _reviewRepo;
        private readonly OnlineStoreDbContext _context;

        public AdminController(
            IProductRepository productRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<ProductReview> reviewRepo,
            OnlineStoreDbContext context)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _reviewRepo = reviewRepo;
            _context = context;
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

        public async Task<IActionResult> Payments()
        {
            // Get active orders with payment data (exclude cancelled orders)
            var activeOrders = await _context.Orders
                .Include(o => o.Payment)
                    .ThenInclude(p => p.CreditCardPayment)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PayPalPayment)
                .Where(o => o.PaymentId.HasValue && o.Payment != null && o.OrderStatusId != 4)
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    PaymentId = o.PaymentId,
                    Amount = o.Payment.Amount,
                    CreatedDate = o.Payment.CreatedDate,
                    PaymentMethod = o.Payment.CreditCardPayment != null ? "Credit Card" : 
                                   o.Payment.PayPalPayment != null ? "PayPal" : "Unknown",
                    CustomerId = o.ExternalUserId,
                    OrderDate = o.CreatedDate,
                    Status = "Active"
                })
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            // Get cancelled/refunded orders
            var cancelledOrders = await _context.Orders
                .Include(o => o.Payment)
                    .ThenInclude(p => p.CreditCardPayment)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PayPalPayment)
                .Where(o => o.PaymentId.HasValue && o.Payment != null && o.OrderStatusId == 4)
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    PaymentId = o.PaymentId,
                    Amount = o.Payment.Amount,
                    CreatedDate = o.Payment.CreatedDate,
                    PaymentMethod = o.Payment.CreditCardPayment != null ? "Credit Card" : 
                                   o.Payment.PayPalPayment != null ? "PayPal" : "Unknown",
                    CustomerId = o.ExternalUserId,
                    OrderDate = o.CreatedDate,
                    Status = "Refunded"
                })
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.Payments = activeOrders;
            ViewBag.CancelledPayments = cancelledOrders;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequest request)
        {
            try
            {
                Console.WriteLine($"RefundPayment called: OrderId={request.OrderId}, Amount={request.Amount}, Method={request.PaymentMethod}");
                
                // Find the order and payment
                var order = await _context.Orders
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

                if (order == null)
                {
                    Console.WriteLine($"Order {request.OrderId} not found");
                    return Json(new { success = false, message = "Order not found." });
                }

                if (order.Payment == null)
                {
                    Console.WriteLine($"Payment not found for order {request.OrderId}");
                    return Json(new { success = false, message = "Payment not found for this order." });
                }

                Console.WriteLine($"Found order {order.OrderId} with payment {order.PaymentId}");

                // Update order status to "Cancelled" (assuming status ID 4 is cancelled)
                order.OrderStatusId = 4; // Cancelled status
                order.ModifiedDate = DateTime.UtcNow;

                // Mark payment as refunded (you might want to add a Refunded field to Payment model)
                // For now, we'll just update the modified date
                order.Payment.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully refunded order {request.OrderId}");
                return Json(new { success = true, message = "Payment refunded and order cancelled successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RefundPayment: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing the refund: " + ex.Message });
            }
        }

        // Add actions like CreateProduct, EditProduct, DeleteProduct
    }
}
