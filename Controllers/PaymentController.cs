using System.Text.Json;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                return RedirectToAction("Login", "Auth");

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
                    CatererId = x.CatererId,
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

            var orderLinks =
                $"Receipt: /Orders/Receipt/{order.Id}\nAgreement: /Orders/Agreement/{order.Id}\nLive Call: /Calls/Order/{order.Id}";

            await _emailService.SendAsync(
                user.Email ?? "unknown@mealinge.com",
                $"Mealinge Order Confirmation #{order.Id}",
                $"Your order has been completed.\n\nOrder ID: {order.Id}\nTotal: {order.TotalAmount} TL\n\nItems:\n{itemSummary}\n\n{orderLinks}");

            var catererIds = order.OrderItems
                .Where(x => !string.IsNullOrWhiteSpace(x.CatererId))
                .Select(x => x.CatererId!)
                .Distinct()
                .ToList();

            var caterers = await _context.Users
                .Where(x => catererIds.Contains(x.Id))
                .ToListAsync();

            var fallbackCatererNames = order.OrderItems
                .Where(x => string.IsNullOrWhiteSpace(x.CatererId))
                .Select(x => x.CatererName)
                .Distinct()
                .ToList();

            if (fallbackCatererNames.Any())
            {
                var fallbackCaterers = await _context.Users
                    .Where(x => fallbackCatererNames.Contains(x.FullName))
                    .ToListAsync();

                caterers.AddRange(fallbackCaterers.Where(x => caterers.All(y => y.Id != x.Id)));
            }

            foreach (var caterer in caterers)
            {
                var catererItems = order.OrderItems
                    .Where(x =>
                        x.CatererId == caterer.Id ||
                        (string.IsNullOrWhiteSpace(x.CatererId) && x.CatererName == caterer.FullName))
                    .Select(x => $"{x.MenuItemName} - {x.Quantity} x {x.FinalUnitPrice} TL - {x.SelectedCustomizations}");

                await _emailService.SendAsync(
                    caterer.Email ?? "unknown@mealinge.com",
                    $"New Mealinge Order #{order.Id}",
                    $"A new completed order includes your menu items.\n\nOrder ID: {order.Id}\nCustomer: {user.Email}\nItems:\n{string.Join("\n", catererItems)}\n\nLive Call: /Calls/Order/{order.Id}");
            }

            await _logService.LogAsync(
                "Email",
                user.Email,
                $"Order #{order.Id} email notifications were created.");

            await _logService.LogAsync(
                "Payment",
                user.Email,
                $"Order #{order.Id} completed successfully. Total: {order.TotalAmount} TL");

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        public IActionResult Success(int? orderId)
        {
            ViewBag.OrderId = orderId;
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
