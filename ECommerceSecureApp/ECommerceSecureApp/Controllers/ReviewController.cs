using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Services;
using System.Security.Claims;

namespace ECommerceSecureApp.Controllers
{
    // Controller for handling product review operations in the online store system
    // Provides endpoints for creating, reading, updating, and deleting product reviews
    public class ReviewController : Controller
    {
        // Service that handles all the review business logic
        private readonly ReviewService _reviewService;
        // Database context for direct database access when needed
        private readonly OnlineStoreDbContext _context;
        // Logger for tracking what the controller is doing
        private readonly ILogger<ReviewController> _logger;

        // Constructor - gets services from dependency injection
        public ReviewController(ReviewService reviewService, OnlineStoreDbContext context, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _context = context;
            _logger = logger;
        }

        // Displays all reviews in the system (Admin view)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all reviews from the service
                var reviews = await _reviewService.GetAllReviewsAsync();
                return View(reviews);
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                _logger.LogError(ex, "Error retrieving all reviews");
                TempData["ErrorMessage"] = "An error occurred while retrieving reviews.";
                return View(new List<ProductReview>());
            }
        }

        // Displays user's own reviews (Logged-in users only)
        [Authorize]
        public async Task<IActionResult> AllReviews()
        {
            try
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToAction("Index", "Home");
                }

                // Get only the current user's reviews
                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return View("Index", reviews);
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                _logger.LogError(ex, "Error retrieving user reviews");
                TempData["ErrorMessage"] = "An error occurred while retrieving your reviews.";
                return View("Index", new List<ProductReview>());
            }
        }

        // Displays reviews for a specific product
        [AllowAnonymous]
        public async Task<IActionResult> ProductReviews(int productId)
        {
            try
            {
                // Find the product first to make sure it exists
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get all the review data we need for the page
                var reviews = await _reviewService.GetReviewsByProductAsync(productId);
                var averageRating = await _reviewService.GetAverageRatingAsync(productId);
                var reviewCount = await _reviewService.GetReviewCountAsync(productId);

                // Check if current user has purchased this product (for UI display)
                bool canReview = false;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        canReview = await _reviewService.HasUserPurchasedProductAsync(userId, productId);
                    }
                }

                // Pass data to the view using ViewBag
                ViewBag.Product = product;
                ViewBag.AverageRating = averageRating;
                ViewBag.ReviewCount = reviewCount;
                ViewBag.CanReview = canReview;

                return View(reviews);
            }
            catch (Exception ex)
            {
                // Log error and redirect to home page
                _logger.LogError(ex, "Error retrieving reviews for product {ProductId}", productId);
                TempData["ErrorMessage"] = "An error occurred while retrieving product reviews.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Displays the form to create a new review
        [Authorize]
        public async Task<IActionResult> Create(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index", "Home");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if user has purchased this product
                var hasPurchased = await _reviewService.HasUserPurchasedProductAsync(userId, productId);
                if (!hasPurchased)
                {
                    TempData["ErrorMessage"] = "You can only review products you have purchased.";
                    return RedirectToAction("ProductReviews", new { productId });
                }

                // Check if user already reviewed this product
                var existingReview = await _context.ProductReviews
                    .FirstOrDefaultAsync(r => r.ProductId == productId && r.ExternalUserId == userId);
                
                if (existingReview != null)
                {
                    TempData["InfoMessage"] = "You have already reviewed this product.";
                    return RedirectToAction("ProductReviews", new { productId });
                }

                ViewBag.Product = product;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create review form for product {ProductId}", productId);
                TempData["ErrorMessage"] = "An error occurred while loading the review form.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Handles the creation of a new review
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ProductReview review)
        {
            try
            {
                // Check if the form data is valid
                if (!ModelState.IsValid)
                {
                    var product = await _context.Products.FindAsync(review.ProductId);
                    ViewBag.Product = product;
                    return View(review);
                }

                // Get the current logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if user has purchased this product
                var hasPurchased = await _reviewService.HasUserPurchasedProductAsync(userId, review.ProductId);
                if (!hasPurchased)
                {
                    TempData["ErrorMessage"] = "You can only review products you have purchased.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                // Check if this user already reviewed this product
                var existingReview = await _context.ProductReviews
                    .FirstOrDefaultAsync(r => r.ProductId == review.ProductId && r.ExternalUserId == userId);
                
                if (existingReview != null)
                {
                    TempData["InfoMessage"] = "You have already reviewed this product.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                // Set the user ID and save the review
                review.ExternalUserId = userId;
                await _reviewService.CreateReviewAsync(review);

                TempData["SuccessMessage"] = "Your review has been submitted successfully.";
                return RedirectToAction("ProductReviews", new { productId = review.ProductId });
            }
            catch (Exception ex)
            {
                // Log error and show form again with error message
                _logger.LogError(ex, "Error creating review for product {ProductId}", review.ProductId);
                TempData["ErrorMessage"] = "An error occurred while submitting your review.";
                
                var product = await _context.Products.FindAsync(review.ProductId);
                ViewBag.Product = product;
                return View(review);
            }
        }

        // Displays the form to edit an existing review
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewAsync(id);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if user owns this review or is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                
                if (review.ExternalUserId != userId && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You can only edit your own reviews.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                // Check if user has purchased this product (unless admin)
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    var hasPurchased = await _reviewService.HasUserPurchasedProductAsync(userId, review.ProductId);
                    if (!hasPurchased)
                    {
                        TempData["ErrorMessage"] = "You can only edit reviews for products you have purchased.";
                        return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                    }
                }

                return View(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit review form for review {ReviewId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the review.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Handles the update of an existing review
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ProductReview review)
        {
            try
            {
                if (id != review.ReviewId)
                {
                    TempData["ErrorMessage"] = "Review ID mismatch.";
                    return RedirectToAction("Index", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return View(review);
                }

                // Check if user owns this review or is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                
                var existingReview = await _reviewService.GetReviewAsync(id);
                if (existingReview == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("Index", "Home");
                }

                if (existingReview.ExternalUserId != userId && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You can only edit your own reviews.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                // Check if user has purchased this product (unless admin)
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    var hasPurchased = await _reviewService.HasUserPurchasedProductAsync(userId, review.ProductId);
                    if (!hasPurchased)
                    {
                        TempData["ErrorMessage"] = "You can only edit reviews for products you have purchased.";
                        return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                    }
                }

                await _reviewService.UpdateReviewAsync(review);

                TempData["SuccessMessage"] = "Your review has been updated successfully.";
                return RedirectToAction("ProductReviews", new { productId = review.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating your review.";
                return View(review);
            }
        }

        // Displays confirmation for deleting a review
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewAsync(id);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if user owns this review or is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                
                if (review.ExternalUserId != userId && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You can only delete your own reviews.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                // Check if user has purchased this product (unless admin)
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    var hasPurchased = await _reviewService.HasUserPurchasedProductAsync(userId, review.ProductId);
                    if (!hasPurchased)
                    {
                        TempData["ErrorMessage"] = "You can only delete reviews for products you have purchased.";
                        return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                    }
                }

                return View(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete review form for review {ReviewId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the review.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Handles the deletion of a review
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewAsync(id);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if user owns this review or is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                
                if (review.ExternalUserId != userId && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You can only delete your own reviews.";
                    return RedirectToAction("ProductReviews", new { productId = review.ProductId });
                }

                var productId = review.ProductId;
                await _reviewService.DeleteReviewAsync(id);

                TempData["SuccessMessage"] = "Your review has been deleted successfully.";
                return RedirectToAction("ProductReviews", new { productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting your review.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Displays user's own reviews
        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToAction("Index", "Home");
                }

                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user reviews for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                TempData["ErrorMessage"] = "An error occurred while retrieving your reviews.";
                return View(new List<ProductReview>());
            }
        }
    }
}
