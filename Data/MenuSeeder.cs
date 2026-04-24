using Ceng382_25_26_202311031.Models;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Data
{
    public static class MenuSeeder
    {
        public static async Task SeedMenusAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.MenuItems.AnyAsync())
                return;

            context.MenuItems.AddRange(
                new MenuItem
                {
                    Name = "Chicken Wrap",
                    Description = "Grilled chicken wrap with fries",
                    Price = 180,
                    ImageUrl = "/images/food1.jpg",
                    CatererName = "Taste Kitchen"
                },
                new MenuItem
                {
                    Name = "Cheese Burger",
                    Description = "Burger with cheddar and special sauce",
                    Price = 220,
                    ImageUrl = "/images/food2.jpg",
                    CatererName = "Burger House"
                },
                new MenuItem
                {
                    Name = "Pasta Alfredo",
                    Description = "Creamy alfredo pasta with mushrooms",
                    Price = 200,
                    ImageUrl = "/images/food3.jpg",
                    CatererName = "Italian Spoon"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}