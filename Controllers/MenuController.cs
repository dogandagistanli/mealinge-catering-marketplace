using Microsoft.AspNetCore.Mvc;
using Ceng382_25_26_202311031.Models;

namespace Ceng382_25_26_202311031.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Id = 1,
                    Name = "Chicken Wrap",
                    Description = "Grilled chicken wrap with fries",
                    Price = 180,
                    ImageUrl = "/images/chicken-wrap.jpg",
                    CatererName = "Taste Kitchen"
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Cheese Burger",
                    Description = "Burger with cheddar and special sauce",
                    Price = 220,
                    ImageUrl = "/images/cheese-burger.jpg",
                    CatererName = "Burger House"
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "Pasta Alfredo",
                    Description = "Creamy alfredo pasta with mushrooms",
                    Price = 200,
                    ImageUrl = "/images/pasta-alfredo.jpg",
                    CatererName = "Italian Spoon"
                }
            };

            return View(menuItems);
        }
    }
}