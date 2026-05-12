using System.Text.Json;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private const string CartSessionKey = "Cart";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogService _logService;
        private readonly EmailService _emailService;

        public PaymentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            LogService logService,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
            _emailService = emailService;
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

            if (user == null)
                return Redirect("/Identity/Account/Login");

            var order = new Order
            {
                UserId = user.Id,
                TotalAmount = cart.Sum(x => x.TotalPrice),
                OrderDate = DateTime.Now,
                Status = "Paid",
                OrderItems = cart.Select(x => new OrderItem
                {
                    MenuItemId = x.MenuItemId,
                    MenuItemName = x.Name,
                    CatererName = x.CatererName,
                    UnitPrice = x.UnitPrice,
                    CustomizationPrice = x.CustomizationPrice,
                    SelectedCustomizations = x.SelectedCustomizations,
                    Quantity = x.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var itemSummary = string.Join("\n", order.OrderItems.Select(x =>
                $"{x.MenuItemName} - {x.Quantity} x {x.FinalUnitPrice} TL - {x.SelectedCustomizations}"));

            await _emailService.SendAsync(
                user.Email ?? "unknown@mealinge.com",
                $"Mealinge Order Confirmation #{order.Id}",
                $"Your order has been completed.\n\nOrder ID: {order.Id}\nTotal: {order.TotalAmount} TL\n\nItems:\n{itemSummary}");

            var catererEmails = order.OrderItems
                .Select(x => $"{x.CatererName.Replace(" ", "").ToLower()}@mealinge.com")
                .Distinct()
                .ToList();

            foreach (var catererEmail in catererEmails)
            {
                await _emailService.SendAsync(
                    catererEmail,
                    $"New Mealinge Order #{order.Id}",
                    $"A new order has been placed.\n\nOrder ID: {order.Id}\nCustomer: {user.Email}\nTotal: {order.TotalAmount} TL\n\nItems:\n{itemSummary}");
            }

            await _logService.LogAsync(
                "Email",
                user.Email,
                $"Order #{order.Id} email notifications were created.");

            await _logService.LogAsync(
                "Payment",
                user.Email,
                $"Order #{order.Id} completed successfully. Total: {order.TotalAmount} ₺");

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