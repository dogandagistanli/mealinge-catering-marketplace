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
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogService _logService;

        public RatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
        }

        public async Task<IActionResult> Create(int orderId, int? orderItemId)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == user!.Id && x.Status == "Paid");

            if (order == null)
                return NotFound();

            var orderItem = orderItemId.HasValue
                ? order.OrderItems.FirstOrDefault(x => x.Id == orderItemId.Value)
                : null;

            var ratedMenuItemIds = await _context.Ratings
                .Where(r => r.OrderId == orderId && r.UserId == user!.Id)
                .Select(r => r.MenuItemId)
                .ToListAsync();

            orderItem ??= order.OrderItems
                .FirstOrDefault(item => !ratedMenuItemIds.Contains(item.MenuItemId));

            if (orderItem == null)
                return RedirectToAction("Index", "Orders");

            var alreadyRated = await _context.Ratings.AnyAsync(x =>
                x.OrderId == orderId &&
                x.UserId == user!.Id &&
                x.MenuItemId == orderItem.MenuItemId);

            if (alreadyRated)
                return RedirectToAction("Index", "Orders");

            var rating = new Rating
            {
                OrderId = order.Id,
                OrderItemId = orderItem.Id,
                MenuItemId = orderItem.MenuItemId,
                MenuItemName = orderItem.MenuItemName,
                CatererName = orderItem.CatererName,
                UserId = user!.Id
            };

            return View(rating);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Rating rating)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.Id == rating.OrderId &&
                    x.UserId == user.Id &&
                    x.Status == "Paid");

            if (order == null)
                return NotFound();

            var orderItem = rating.OrderItemId.HasValue
                ? order.OrderItems.FirstOrDefault(x => x.Id == rating.OrderItemId.Value)
                : order.OrderItems.FirstOrDefault(x => x.MenuItemId == rating.MenuItemId);

            if (orderItem == null)
                return NotFound();

            var alreadyRated = await _context.Ratings.AnyAsync(x =>
                x.OrderId == order.Id &&
                x.UserId == user.Id &&
                x.MenuItemId == orderItem.MenuItemId);

            if (alreadyRated)
                return RedirectToAction("Index", "Orders");

            if (rating.MenuItemScore < 1 || rating.MenuItemScore > 5)
                ModelState.AddModelError("MenuItemScore", "Menu score must be between 1 and 5.");

            if (rating.CatererScore < 1 || rating.CatererScore > 5)
                ModelState.AddModelError("CatererScore", "Caterer score must be between 1 and 5.");

            if (!ModelState.IsValid)
            {
                rating.OrderItemId = orderItem.Id;
                rating.MenuItemId = orderItem.MenuItemId;
                rating.MenuItemName = orderItem.MenuItemName;
                rating.CatererName = orderItem.CatererName;
                return View(rating);
            }

            rating.OrderId = order.Id;
            rating.OrderItemId = orderItem.Id;
            rating.MenuItemId = orderItem.MenuItemId;
            rating.MenuItemName = orderItem.MenuItemName;
            rating.CatererName = orderItem.CatererName;
            rating.UserId = user.Id;
            rating.CreatedAt = DateTime.Now;

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                "Rating",
                user.Email,
                $"Order #{rating.OrderId} was rated. Menu score: {rating.MenuItemScore}, Caterer score: {rating.CatererScore}");

            return RedirectToAction("Index", "Orders");
        }
    }
}
