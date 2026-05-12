using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize(Roles = "Caterer,Admin")]
    public class CustomizationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogService _logService;

        public CustomizationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
        }

        public async Task<IActionResult> Manage(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .Include(x => x.CustomizationOptions)
                .FirstOrDefaultAsync(x => x.Id == menuItemId);

            if (menuItem == null)
                return NotFound();

            if (!await CanManageMenuItem(menuItem))
                return Forbid();

            return View(menuItem);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int menuItemId, string groupName, string optionName, string optionType, decimal priceChange)
        {
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);

            if (menuItem == null)
                return NotFound();

            if (!await CanManageMenuItem(menuItem))
                return Forbid();

            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(optionName))
                return RedirectToAction("Manage", new { menuItemId });

            var option = new CustomizationOption
            {
                MenuItemId = menuItemId,
                GroupName = groupName,
                OptionName = optionName,
                OptionType = optionType,
                PriceChange = priceChange
            };

            _context.CustomizationOptions.Add(option);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _logService.LogAsync(
                "Customization",
                user?.Email,
                $"Customization option '{option.OptionName}' was added to '{menuItem.Name}'.");

            return RedirectToAction("Manage", new { menuItemId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var option = await _context.CustomizationOptions.FindAsync(id);

            if (option == null)
                return NotFound();

            var menuItem = await _context.MenuItems.FindAsync(option.MenuItemId);

            if (menuItem == null)
                return NotFound();

            if (!await CanManageMenuItem(menuItem))
                return Forbid();

            var menuItemId = option.MenuItemId;

            _context.CustomizationOptions.Remove(option);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _logService.LogAsync(
                "Customization",
                user?.Email,
                $"Customization option '{option.OptionName}' was removed from '{menuItem.Name}'.");

            return RedirectToAction("Manage", new { menuItemId });
        }

        private async Task<bool> CanManageMenuItem(MenuItem menuItem)
        {
            if (User.IsInRole("Admin"))
                return true;

            var user = await _userManager.GetUserAsync(User);

            return menuItem.CatererId == user?.Id;
        }
    }
}
