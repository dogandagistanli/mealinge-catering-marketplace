using Ceng382_25_26_202311031.Models;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Data
{
    public static class MenuSeeder
    {
        public static async Task SeedMenusAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var tasteKitchen = await context.Users
                .FirstOrDefaultAsync(x => x.Email == "caterer1@mealinge.com");

            var ankaraKitchen = await context.Users
                .FirstOrDefaultAsync(x => x.Email == "caterer2@mealinge.com");

            var quickKitchen = await context.Users
                .FirstOrDefaultAsync(x => x.Email == "caterer3@mealinge.com");

            if (await context.MenuItems.AnyAsync())
            {
                await BackfillExistingMenusAsync(
                    context,
                    tasteKitchen,
                    ankaraKitchen,
                    quickKitchen);

                return;
            }

            var menuItems = new List<MenuItem>
            {
                new()
                {
                    Name = "Chicken Wrap",
                    Description = "Grilled chicken wrap with fries",
                    Price = 180,
                    ImageUrl = "/images/chicken-wrap.jpg",
                    CatererId = tasteKitchen?.Id,
                    CatererName = tasteKitchen?.FullName ?? "Taste Kitchen",
                    CustomizationOptions = new List<CustomizationOption>
                    {
                        new() { GroupName = "Removable Ingredients", OptionName = "No onion", OptionType = "Removable", PriceChange = 0 },
                        new() { GroupName = "Extras", OptionName = "Extra chicken", OptionType = "Addition", PriceChange = 45 },
                        new() { GroupName = "Sauce Choice", OptionName = "BBQ sauce", OptionType = "Group", PriceChange = 10 }
                    }
                },
                new()
                {
                    Name = "Cheese Burger",
                    Description = "Burger with cheddar and special sauce",
                    Price = 220,
                    ImageUrl = "/images/cheese-burger.jpg",
                    CatererId = ankaraKitchen?.Id,
                    CatererName = ankaraKitchen?.FullName ?? "Ankara Catering House",
                    CustomizationOptions = new List<CustomizationOption>
                    {
                        new() { GroupName = "Removable Ingredients", OptionName = "No pickle", OptionType = "Removable", PriceChange = 0 },
                        new() { GroupName = "Extras", OptionName = "Extra cheddar", OptionType = "Addition", PriceChange = 25 },
                        new() { GroupName = "Side Choice", OptionName = "Large fries", OptionType = "Group", PriceChange = 35 }
                    }
                },
                new()
                {
                    Name = "Pasta Alfredo",
                    Description = "Creamy alfredo pasta with mushrooms",
                    Price = 200,
                    ImageUrl = "/images/pasta-alfredo.jpg",
                    CatererId = quickKitchen?.Id,
                    CatererName = quickKitchen?.FullName ?? "Quick Meal Kitchen",
                    CustomizationOptions = new List<CustomizationOption>
                    {
                        new() { GroupName = "Removable Ingredients", OptionName = "No mushroom", OptionType = "Removable", PriceChange = 0 },
                        new() { GroupName = "Extras", OptionName = "Extra parmesan", OptionType = "Addition", PriceChange = 20 },
                        new() { GroupName = "Protein Choice", OptionName = "Add chicken", OptionType = "Group", PriceChange = 50 }
                    }
                }
            };

            context.MenuItems.AddRange(menuItems);

            await context.SaveChangesAsync();
        }

        private static async Task BackfillExistingMenusAsync(
            ApplicationDbContext context,
            ApplicationUser? tasteKitchen,
            ApplicationUser? ankaraKitchen,
            ApplicationUser? quickKitchen)
        {
            var existingItems = await context.MenuItems
                .Include(x => x.CustomizationOptions)
                .ToListAsync();

            foreach (var item in existingItems)
            {
                if (string.IsNullOrWhiteSpace(item.CatererId))
                {
                    var caterer = item.Name switch
                    {
                        "Chicken Wrap" => tasteKitchen,
                        "Cheese Burger" => ankaraKitchen,
                        "Pasta Alfredo" => quickKitchen,
                        _ => null
                    };

                    if (caterer != null)
                    {
                        item.CatererId = caterer.Id;
                        item.CatererName = caterer.FullName;
                    }
                }

                if (item.CustomizationOptions == null || !item.CustomizationOptions.Any())
                {
                    context.CustomizationOptions.AddRange(CreateDefaultOptions(item));
                }
            }

            await context.SaveChangesAsync();
        }

        private static IEnumerable<CustomizationOption> CreateDefaultOptions(MenuItem item)
        {
            if (item.Name == "Chicken Wrap")
            {
                return new List<CustomizationOption>
                {
                    new() { MenuItemId = item.Id, GroupName = "Removable Ingredients", OptionName = "No onion", OptionType = "Removable", PriceChange = 0 },
                    new() { MenuItemId = item.Id, GroupName = "Extras", OptionName = "Extra chicken", OptionType = "Addition", PriceChange = 45 },
                    new() { MenuItemId = item.Id, GroupName = "Sauce Choice", OptionName = "BBQ sauce", OptionType = "Group", PriceChange = 10 }
                };
            }

            if (item.Name == "Cheese Burger")
            {
                return new List<CustomizationOption>
                {
                    new() { MenuItemId = item.Id, GroupName = "Removable Ingredients", OptionName = "No pickle", OptionType = "Removable", PriceChange = 0 },
                    new() { MenuItemId = item.Id, GroupName = "Extras", OptionName = "Extra cheddar", OptionType = "Addition", PriceChange = 25 },
                    new() { MenuItemId = item.Id, GroupName = "Side Choice", OptionName = "Large fries", OptionType = "Group", PriceChange = 35 }
                };
            }

            if (item.Name == "Pasta Alfredo")
            {
                return new List<CustomizationOption>
                {
                    new() { MenuItemId = item.Id, GroupName = "Removable Ingredients", OptionName = "No mushroom", OptionType = "Removable", PriceChange = 0 },
                    new() { MenuItemId = item.Id, GroupName = "Extras", OptionName = "Extra parmesan", OptionType = "Addition", PriceChange = 20 },
                    new() { MenuItemId = item.Id, GroupName = "Protein Choice", OptionName = "Add chicken", OptionType = "Group", PriceChange = 50 }
                };
            }

            return Enumerable.Empty<CustomizationOption>();
        }
    }
}
