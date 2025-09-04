using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Create Views/Customer/Dashboard.cshtml
    }

    public IActionResult Orders()
    {
        // Show customer's order history
        return View();
    }

    public IActionResult Profile()
    {
        // Show and edit customer profile
        return View();
    }
}
