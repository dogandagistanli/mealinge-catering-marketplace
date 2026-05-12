using System.Text.Json;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int menuItemId, List<int>? selectedOptions)
        {
            var menuItem = await _context.MenuItems
                .Include(x => x.Caterer)
                .Include(x => x.CustomizationOptions)
                .FirstOrDefaultAsync(x => x.Id == menuItemId);

            if (menuItem == null)
                return NotFound();

            selectedOptions ??= new List<int>();

            var chosenOptions = menuItem.CustomizationOptions?
                .Where(x => selectedOptions.Contains(x.Id))
                .ToList() ?? new List<CustomizationOption>();

            var selectedText = chosenOptions.Any()
                ? string.Join(", ", chosenOptions.Select(x => $"{x.OptionName} ({x.PriceChange} TL)"))
                : "No customization";

            var customizationPrice = chosenOptions.Sum(x => x.PriceChange);

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(x =>
                x.MenuItemId == menuItemId &&
                x.SelectedCustomizations == selectedText);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MenuItemId = menuItem.Id,
                    Name = menuItem.Name,
                    CatererId = menuItem.CatererId,
                    CatererName = menuItem.CatererName,
                    UnitPrice = menuItem.Price,
                    CustomizationPrice = customizationPrice,
                    SelectedCustomizations = selectedText,
                    Quantity = 1,
                    Description = menuItem.Description
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int menuItemId, string selectedCustomizations)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x =>
                x.MenuItemId == menuItemId &&
                x.SelectedCustomizations == selectedCustomizations);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, string selectedCustomizations, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x =>
                x.MenuItemId == menuItemId &&
                x.SelectedCustomizations == selectedCustomizations);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;

                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}
