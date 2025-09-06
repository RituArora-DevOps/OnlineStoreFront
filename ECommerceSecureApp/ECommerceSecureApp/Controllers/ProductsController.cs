using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search;
using ECommerceSecureApp.Services;

namespace ECommerceSecureApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ReviewService _reviewService;
        
        // Follows Dependency Inversion principle
        public ProductsController(IProductRepository productRepository, ReviewService reviewService)
        {
            _productRepository = productRepository;
            _reviewService = reviewService;
        }

        private void PopulateCategoryList()
        {
            ViewBag.CategoryList = new SelectList(new[] { "Electronics", "Clothing", "Grocery" });
        }

        // GET: Products
        public async Task<IActionResult> Index(int page = 1)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;
            var pagedProducts = await _productRepository.GetAllProductsAsync(page, pageSize);
            return View(pagedProducts);
        }

        // GET: Products/Search?name=Shoes&category=Clothing&minPrice=50&maxPrice=100&page=1
        public async Task<IActionResult> Search(string name, string category, decimal minPrice, decimal maxPrice, int page=1)
        {
            var criteria = new ProductSearchCriteria
            {
                Name = name,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
            };

            var results = await _productRepository.SearchProductsAsync(criteria, page, 10);
            return View("Index", results.Items);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            // Get reviews for this product
            var reviews = await _reviewService.GetPublicReviewsByProductAsync(id.Value);
            var averageRating = await _reviewService.GetAverageRatingAsync(id.Value);
            var reviewCount = await _reviewService.GetReviewCountAsync(id.Value);

            // Check if current user has purchased this product (for UI display)
            bool canReview = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    canReview = await _reviewService.HasUserPurchasedProductAsync(userId, id.Value);
                }
            }

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.CanReview = canReview;

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            PopulateCategoryList();

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Price,Category,Name,Description,CreatedDate,ModifiedDate")] Product product)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                await _productRepository.InsertAsync(product);
                await _productRepository.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategoryList();

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            PopulateCategoryList();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Price,Category,Name,Description,CreatedDate,ModifiedDate")] Product product)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _productRepository.GetByIdAsync(product.ProductId);
                    if (exists == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateCategoryList();

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Redirect non-admin users to the public product catalog
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product.ProductId);
            }

            await _productRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null;
        }


    }
}
