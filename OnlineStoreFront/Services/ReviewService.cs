using Microsoft.EntityFrameworkCore;
using OnlineStoreFront.Models.Business;

namespace OnlineStoreFront.Services
{
    // Service for managing product reviews in the online store
    public class ReviewService
    {
        // Database connection for accessing product reviews
        private readonly OnlineStoreContext _context;
        // Logger for tracking what the service is doing
        private readonly ILogger<ReviewService> _logger;

        // Constructor - gets database and logger from dependency injection
        public ReviewService(OnlineStoreContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Gets all reviews from the database
        public async Task<List<ProductReview>> GetAllReviewsAsync()
        {
            // Log that we're starting to fetch reviews
            _logger.LogInformation("Fetching all reviews");
            
            // Get all reviews from database, including the related product info
            var reviews = await _context.ProductReviews
                .Include(r => r.Product)
                .ToListAsync();
                
            // Log how many reviews we found
            _logger.LogInformation("Found {Count} reviews", reviews.Count);
            
            // If no reviews found, log a warning
            if (reviews.Count == 0)
            {
                _logger.LogWarning("No reviews found in the database");
            }
            else
            {
                // Log details about each review found
                foreach (var review in reviews)
                {
                    _logger.LogInformation("Review: id={ReviewId}, productId={ProductId}, rating={Rating}", 
                        review.ReviewId, 
                        review.ProductId,
                        review.Rating);
                }
            }
            
            return reviews;
        }

        // Gets a specific review using its ID
        public async Task<ProductReview?> GetReviewAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Review ID must be greater than 0", nameof(id));
            }

            return await _context.ProductReviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        // Creates a new review in the database
        public async Task<ProductReview> CreateReviewAsync(ProductReview review)
        {
            // Check if review object is null
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            // Make sure product ID is valid
            if (review.ProductId <= 0)
            {
                throw new ArgumentException("Product ID must be specified", nameof(review));
            }

            // Make sure user ID is provided
            if (string.IsNullOrEmpty(review.ExternalUserId))
            {
                throw new ArgumentException("User ID must be specified", nameof(review));
            }

            // Make sure rating is between 1 and 5 stars
            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5", nameof(review));
            }

            // Check if the product actually exists in the database
            var product = await _context.Products.FindAsync(review.ProductId);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {review.ProductId} not found", nameof(review));
            }

            // Set the creation date to now if not already set
            if (review.CreatedDate == default)
            {
                review.CreatedDate = DateTime.Now;
            }

            // Add the review to the database
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            // Log that we successfully created the review
            _logger.LogInformation("Created review {ReviewId} for product {ProductId} by user {UserId}", 
                review.ReviewId, review.ProductId, review.ExternalUserId);

            return review;
        }

        // Creates a new review with individual fields
        public async Task<ProductReview> CreateReviewAsync(int productId, string userId, int rating, string? comment = null)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID must be specified", nameof(userId));
            }

            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
            }

            // Verify product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {productId} not found", nameof(productId));
            }

            var review = new ProductReview
            {
                ProductId = productId,
                ExternalUserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created review {ReviewId} for product {ProductId} by user {UserId}", 
                review.ReviewId, productId, userId);

            return review;
        }

        // Updates an existing review
        public async Task<ProductReview> UpdateReviewAsync(ProductReview review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            if (review.ReviewId <= 0)
            {
                throw new ArgumentException("Review ID must be specified", nameof(review));
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5", nameof(review));
            }

            var existingReview = await _context.ProductReviews.FindAsync(review.ReviewId);
            if (existingReview == null)
            {
                throw new ArgumentException($"Review with ID {review.ReviewId} not found", nameof(review));
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated review {ReviewId}", review.ReviewId);

            return existingReview;
        }

        // Deletes a review from the database
        public async Task DeleteReviewAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Review ID must be greater than 0", nameof(id));
            }

            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
            {
                throw new ArgumentException($"Review with ID {id} not found", nameof(id));
            }

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted review {ReviewId}", id);
        }

        // Gets all reviews for a specific product
        public async Task<List<ProductReview>> GetReviewsByProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            }

            return await _context.ProductReviews
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        // Gets all reviews written by a specific user
        public async Task<List<ProductReview>> GetReviewsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID must be specified", nameof(userId));
            }

            return await _context.ProductReviews
                .Include(r => r.Product)
                .Where(r => r.ExternalUserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        // Gets the average rating for a product
        public async Task<double?> GetAverageRatingAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            }

            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId && r.Rating.HasValue)
                .Select(r => r.Rating.Value)
                .ToListAsync();

            return reviews.Count > 0 ? reviews.Average() : null;
        }

        // Gets the total number of reviews for a product
        public async Task<int> GetReviewCountAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            }

            return await _context.ProductReviews
                .CountAsync(r => r.ProductId == productId);
        }
    }
}
