using System.Text.Json;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    public class PaymentController : Controller
    {
        private const string CartSessionKey = "Cart";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(new PaymentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                UserId = user?.Id,
                TotalAmount = cart.Sum(x => x.TotalPrice),
                OrderDate = DateTime.Now,
                Status = "Paid",
                OrderItems = cart.Select(x => new OrderItem
                {
                    MenuItemId = x.MenuItemId,
                    MenuItemName = x.Name,
                    CatererName = x.CatererName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }
    }
}