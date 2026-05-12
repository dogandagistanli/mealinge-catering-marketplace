using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Identity;

namespace Ceng382_25_26_202311031.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Caterer", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@mealinge.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@mealinge.com",
                    Email = "admin@mealinge.com",
                    FullName = "System Admin",
                    RoleDisplayName = "Admin",
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(admin, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("caterer1@mealinge.com") == null)
            {
                var caterer1 = new ApplicationUser
                {
                    UserName = "caterer1@mealinge.com",
                    Email = "caterer1@mealinge.com",
                    FullName = "Taste Kitchen",
                    RoleDisplayName = "Caterer",
                    EmailConfirmed = true,
                    Latitude = 39.9208,
                    Longitude = 32.8541
                };

                var result =
                    await userManager.CreateAsync(caterer1, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(caterer1, "Caterer");
                }
            }

            if (await userManager.FindByEmailAsync("caterer2@mealinge.com") == null)
            {
                var caterer2 = new ApplicationUser
                {
                    UserName = "caterer2@mealinge.com",
                    Email = "caterer2@mealinge.com",
                    FullName = "Ankara Catering House",
                    RoleDisplayName = "Caterer",
                    EmailConfirmed = true,
                    Latitude = 39.9250,
                    Longitude = 32.8450
                };

                var result =
                    await userManager.CreateAsync(caterer2, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(caterer2, "Caterer");
                }
            }

            if (await userManager.FindByEmailAsync("caterer3@mealinge.com") == null)
            {
                var caterer3 = new ApplicationUser
                {
                    UserName = "caterer3@mealinge.com",
                    Email = "caterer3@mealinge.com",
                    FullName = "Quick Meal Kitchen",
                    RoleDisplayName = "Caterer",
                    EmailConfirmed = true,
                    Latitude = 39.9180,
                    Longitude = 32.8600
                };

                var result =
                    await userManager.CreateAsync(caterer3, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(caterer3, "Caterer");
                }
            }

            if (await userManager.FindByEmailAsync("user1@mealinge.com") == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = "user1@mealinge.com",
                    Email = "user1@mealinge.com",
                    FullName = "Demo User One",
                    RoleDisplayName = "User",
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(user1, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "User");
                }
            }

            var personalUser =
                await userManager.FindByEmailAsync("dogandagistanli@gmail.com");

            if (personalUser != null)
            {
                if (!await userManager.IsInRoleAsync(personalUser, "User"))
                {
                    await userManager.AddToRoleAsync(personalUser, "User");
                }
            }
        }
    }
}