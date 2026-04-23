using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Identity;

namespace Ceng382_25_26_202311031.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Caterer", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@mealora.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@mealora.com",
                    Email = "admin@mealora.com",
                    FullName = "System Admin",
                    RoleDisplayName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("caterer1@mealora.com") == null)
            {
                var caterer1 = new ApplicationUser
                {
                    UserName = "caterer1@mealora.com",
                    Email = "caterer1@mealora.com",
                    FullName = "Taste Kitchen",
                    RoleDisplayName = "Caterer",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(caterer1, "123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(caterer1, "Caterer");
                }
            }

            if (await userManager.FindByEmailAsync("user1@mealora.com") == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = "user1@mealora.com",
                    Email = "user1@mealora.com",
                    FullName = "Demo User One",
                    RoleDisplayName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user1, "123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "User");
                }
            }
        }
    }
}