using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreFront.Services;

namespace OnlineStoreFront.Controllers;

[AllowAnonymous]
public class CartController : Controller
{
    private const string GuestCookie = "GuestCartId";
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    private string? CurrentUserId => User?.Identity?.IsAuthenticated == true
        ? User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        : null;

    private string? EnsureGuestId()
    {
        if (CurrentUserId != null) return null;
        if (!Request.Cookies.TryGetValue(GuestCookie, out var gid))
        {
            gid = Guid.NewGuid().ToString("N");
            Response.Cookies.Append(GuestCookie, gid, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
        return gid;
    }

    public async Task<IActionResult> Index()
        => View(await _cart.GetCartAsync(CurrentUserId, EnsureGuestId()));

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int qty = 1)
    { await _cart.AddAsync(productId, qty, CurrentUserId, EnsureGuestId()); return RedirectToAction(nameof(Index)); }

    [HttpPost]
    public async Task<IActionResult> Update(int cartItemId, int qty)
    { await _cart.UpdateQtyAsync(cartItemId, qty, CurrentUserId, EnsureGuestId()); return RedirectToAction(nameof(Index)); }

    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    { await _cart.RemoveAsync(cartItemId, CurrentUserId, EnsureGuestId()); return RedirectToAction(nameof(Index)); }

    [HttpPost]
    public async Task<IActionResult> Clear()
    { await _cart.ClearAsync(CurrentUserId, EnsureGuestId()); return RedirectToAction(nameof(Index)); }
}
