using System.Security.Claims;
using ECommerceSecureApp.Models.ViewModels;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.Services.Coupons;
using ECommerceSecureApp.Services.Pricing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSecureApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPriceCalculator _priceCalc;
        private readonly ICouponService _couponSvc;

        private const string CartSessionKey = "CartId";
        private const string CouponSessionKey = "CartCouponCode";

        public CartController(
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IPaymentRepository paymentRepo,
            IPriceCalculator priceCalc,
            ICouponService couponSvc)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _priceCalc = priceCalc;
            _couponSvc = couponSvc;
        }

        private string? GetExternalUserId() => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        private int? GetSessionCartId() => HttpContext.Session.GetInt32(CartSessionKey);
        private void SetSessionCartId(int cartId) => HttpContext.Session.SetInt32(CartSessionKey, cartId);
        private string? GetCoupon() => HttpContext.Session.GetString(CouponSessionKey);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var externalUserId = GetExternalUserId();
            var cart = await _cartRepo.GetOrCreateCartAsync(externalUserId, GetSessionCartId());
            SetSessionCartId(cart.CartId);

            var couponCode = GetCoupon();
            var coupon = _couponSvc.Resolve(couponCode); 

            var items = await _cartRepo.GetItemsAsync(cart.CartId);

            var vm = new CartVM
            {
                CartId = cart.CartId,
                AppliedCoupon = couponCode,
                Items = items.Select(ci =>
                {
                    var p = ci.Product;
                    var orig = p.Price;

                    var discUnit = _priceCalc.GetDiscountedUnitPrice(p, coupon);


                    return new CartItemVM
                    {
                        CartItemId = ci.CartItemId,
                        ProductId = ci.ProductId,
                        Name = p.Name,
                        OriginalUnitPrice = orig,
                        DiscountedUnitPrice = discUnit,
                        Quantity = ci.Quantity
                    };
                }).ToList()
            };
            return View(vm);
        }



        // ========= Supports AJAX (stays on page) =========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var cart = await _cartRepo.GetOrCreateCartAsync(GetExternalUserId(), GetSessionCartId());
            await _cartRepo.AddOrUpdateItemAsync(cart.CartId, productId, quantity);
            SetSessionCartId(cart.CartId);

            // If AJAX request, return 200 OK with plain text (used by toast)
            if (Request.Headers.TryGetValue("X-Requested-With", out var xrw) &&
                string.Equals(xrw, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                return Content("Added to cart");
            }

            // Fallback (non-AJAX): go to cart
            TempData["Success"] = "Item added to cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            await _cartRepo.UpdateQuantityAsync(cartItemId, quantity);

            // If it's an AJAX request, return recalculated totals as JSON
            if (Request.Headers.TryGetValue("X-Requested-With", out var xrw) &&
                string.Equals(xrw, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                // Rebuild current cart view-model (discount-aware)
                var externalUserId = GetExternalUserId();
                var cart = await _cartRepo.GetOrCreateCartAsync(externalUserId, GetSessionCartId());
                var coupon = _couponSvc.Resolve(GetCoupon());
                var items = await _cartRepo.GetItemsAsync(cart.CartId);

                var rows = items.Select(ci =>
                {
                    var p = ci.Product;
                    var orig = p.Price;
                    var discUnit = _priceCalc.GetDiscountedUnitPrice(p, coupon);
                    return new
                    {
                        ci.CartItemId,
                        ci.Quantity,
                        OriginalUnitPrice = orig,
                        DiscountedUnitPrice = discUnit,
                        DiscountedLineTotal = discUnit * ci.Quantity,
                        LineDiscount = (orig - discUnit) * ci.Quantity
                    };
                }).ToList();

                var subtotalOriginal = rows.Sum(r => r.OriginalUnitPrice * r.Quantity);
                var discountTotal = rows.Sum(r => r.LineDiscount);
                var total = subtotalOriginal - discountTotal;

                // Find the row we changed (may be missing if quantity went to 0)
                var row = rows.FirstOrDefault(r => r.CartItemId == cartItemId);

                return Json(new
                {
                    success = true,
                    // For footer:
                    subtotalOriginal,
                    discountTotal,
                    total,
                    // For the edited row (can be null if removed):
                    row
                });
            }

            // Fallback for non-AJAX
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartRepo.RemoveItemAsync(cartItemId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear(int cartId)
        {
            await _cartRepo.ClearAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
                TempData["Warning"] = "Please enter a coupon code.";
            else
            {
                HttpContext.Session.SetString(CouponSessionKey, couponCode.Trim());
                TempData["Success"] = "Coupon applied.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ClearCoupon()
        {
            HttpContext.Session.Remove(CouponSessionKey);
            TempData["Success"] = "Coupon cleared.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Checkout()
        {
            var externalUserId = GetExternalUserId();
            var cart = await _cartRepo.GetOrCreateCartAsync(externalUserId, GetSessionCartId());
            var items = await _cartRepo.GetItemsAsync(cart.CartId);

            if (!items.Any())
            {
                TempData["Warning"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var couponCode = GetCoupon();
            var coupon = _couponSvc.Resolve(couponCode); 

            var vm = new CheckoutVM
            {
                CartId = cart.CartId,
                AppliedCoupon = couponCode,
                Items = items.Select(ci =>
                {
                    var p = ci.Product;
                    var orig = p.Price;

                    var discUnit = _priceCalc.GetDiscountedUnitPrice(p, coupon);
                    // If you don't have the overload yet:
                    // var discUnit = _priceCalc.GetDiscountedUnitPrice(p, coupon?.AmountOff ?? 0);

                    return new CartItemVM
                    {
                        CartItemId = ci.CartItemId,
                        ProductId = ci.ProductId,
                        Name = p.Name,
                        OriginalUnitPrice = orig,
                        DiscountedUnitPrice = discUnit,
                        Quantity = ci.Quantity
                    };
                }).ToList()
            };
            return View(vm);
        }


        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVM input)
        {
            var externalUserId = GetExternalUserId();
            var cart = await _cartRepo.GetOrCreateCartAsync(externalUserId, GetSessionCartId());
            var items = await _cartRepo.GetItemsAsync(cart.CartId);
            if (!items.Any())
            {
                TempData["Warning"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var coupon = _couponSvc.Resolve(GetCoupon());
            var total = items.Sum(ci =>
            {
                var unit = _priceCalc.GetDiscountedUnitPrice(ci.Product, coupon);

                return unit * ci.Quantity;
            });


            int? paymentId = null;
            if (string.Equals(input.PaymentMethod, "PayPal", StringComparison.OrdinalIgnoreCase))
            {
                var pay = await _paymentRepo.CreatePayPalAsync(total, input.PayPalEmail);
                paymentId = pay.PaymentId;
            }
            else
            {
                var last4 = (input.CardLast4 ?? "").Trim();
                if (last4.Length > 4) last4 = last4[^4..];
                var pay = await _paymentRepo.CreateCreditCardAsync(total, last4, input.CardBrand, input.ExpMonth, input.ExpYear);
                paymentId = pay.PaymentId;
            }

            var order = await _orderRepo.CreateOrderFromCartAsync(cart.CartId, externalUserId, paymentId);
            HttpContext.Session.Remove(CartSessionKey);

            TempData["Success"] = $"Order #{order.OrderId} placed successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
