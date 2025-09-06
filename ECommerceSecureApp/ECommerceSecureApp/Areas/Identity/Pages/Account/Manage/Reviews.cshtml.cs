using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Services;
using System.Security.Claims;

namespace ECommerceSecureApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ReviewsModel : PageModel
    {
        private readonly ReviewService _reviewService;
        private readonly ILogger<ReviewsModel> _logger;

        public ReviewsModel(ReviewService reviewService, ILogger<ReviewsModel> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();

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

                Reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user reviews for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                TempData["ErrorMessage"] = "An error occurred while retrieving your reviews.";
                Reviews = new List<ProductReview>();
                return Page();
            }
        }
    }
}
