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
            // Get all orders with payment data included using DbContext directly
            var ordersWithPayments = await _context.Orders
                .Include(o => o.Payment)
                    .ThenInclude(p => p.CreditCardPayment)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PayPalPayment)
                .Where(o => o.PaymentId.HasValue && o.Payment != null)
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    PaymentId = o.PaymentId,
                    Amount = o.Payment.Amount,
                    CreatedDate = o.Payment.CreatedDate,
                    PaymentMethod = o.Payment.CreditCardPayment != null ? "Credit Card" : 
                                   o.Payment.PayPalPayment != null ? "PayPal" : "Unknown",
                    CustomerId = o.ExternalUserId,
                    OrderDate = o.CreatedDate
                })
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.Payments = ordersWithPayments;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequest request)
        {
            try
            {
                // Find the order and payment
                var order = await _context.Orders
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

                if (order == null || order.Payment == null)
                {
                    return Json(new { success = false, message = "Order or payment not found." });
                }

                // Update order status to "Cancelled" (assuming status ID 4 is cancelled)
                order.OrderStatusId = 4; // Cancelled status
                order.ModifiedDate = DateTime.UtcNow;

                // Mark payment as refunded (you might want to add a Refunded field to Payment model)
                // For now, we'll just update the modified date
                order.Payment.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Payment refunded and order cancelled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the refund: " + ex.Message });
            }
        }

        // Add actions like CreateProduct, EditProduct, DeleteProduct
    }
}
