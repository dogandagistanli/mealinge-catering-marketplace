using System.Text.Json;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    public class PaymentController : Controller
    {
        private const string CartSessionKey = "Cart";

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(new PaymentViewModel());
        }

        [HttpPost]
        public IActionResult Checkout(PaymentViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
                return View(model);

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