using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize(Roles = "Caterer,Admin")]
    public class CustomizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Manage(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .Include(x => x.CustomizationOptions)
                .FirstOrDefaultAsync(x => x.Id == menuItemId);

            if (menuItem == null)
                return NotFound();

            return View(menuItem);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int menuItemId, string groupName, string optionName, string optionType, decimal priceChange)
        {
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

            return RedirectToAction("Manage", new { menuItemId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var option = await _context.CustomizationOptions.FindAsync(id);

            if (option == null)
                return NotFound();

            var menuItemId = option.MenuItemId;

            _context.CustomizationOptions.Remove(option);
            await _context.SaveChangesAsync();

            return RedirectToAction("Manage", new { menuItemId });
        }
    }
}