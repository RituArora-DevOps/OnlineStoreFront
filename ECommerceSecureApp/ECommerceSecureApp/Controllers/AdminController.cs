using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSecureApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View(); // Create Views/Admin/Dashboard.cshtml
        }

        public IActionResult ManageProducts()
        {
            // Return a view with product management tools
            return View();
        }

        // Add actions like CreateProduct, EditProduct, DeleteProduct
    }
}
