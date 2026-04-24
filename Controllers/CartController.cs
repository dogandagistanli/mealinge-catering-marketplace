using System.Text.Json;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int menuItemId)
        {
            var menuItems = GetSampleMenuItems();
            var menuItem = menuItems.FirstOrDefault(x => x.Id == menuItemId);

            if (menuItem == null)
                return NotFound();

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(x => x.MenuItemId == menuItemId);
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
                    CatererName = menuItem.CatererName,
                    UnitPrice = menuItem.Price,
                    Quantity = 1,
                    Description = menuItem.Description
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int menuItemId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MenuItemId == menuItemId);

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

        private List<MenuItem> GetSampleMenuItems()
        {
            return new List<MenuItem>
            {
                new MenuItem
                {
                    Id = 1,
                    Name = "Chicken Wrap",
                    Description = "Grilled chicken wrap with fries",
                    Price = 180,
                    ImageUrl = "/images/food1.jpg",
                    CatererName = "Taste Kitchen"
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Cheese Burger",
                    Description = "Burger with cheddar and special sauce",
                    Price = 220,
                    ImageUrl = "/images/food2.jpg",
                    CatererName = "Burger House"
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "Pasta Alfredo",
                    Description = "Creamy alfredo pasta with mushrooms",
                    Price = 200,
                    ImageUrl = "/images/food3.jpg",
                    CatererName = "Italian Spoon"
                }
            };
        }
    }
}