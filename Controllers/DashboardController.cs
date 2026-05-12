using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin");

            if (User.IsInRole("Caterer"))
                return RedirectToAction("Caterer");

            return RedirectToAction("UserDashboard");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalMenus = await _context.MenuItems.CountAsync();
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(x => x.TotalAmount);

            return View();
        }

        [Authorize(Roles = "Caterer")]
        public async Task<IActionResult> Caterer()
        {
            var user = await _userManager.GetUserAsync(User);
            var catererName = user?.FullName ?? "";

            ViewBag.TotalMenus = await _context.MenuItems
                .CountAsync(x => x.CatererId == user!.Id);

            ViewBag.TotalOrders = await _context.OrderItems
                .CountAsync(x => x.CatererName == catererName);

            ViewBag.TotalRevenue = await _context.OrderItems
                .Where(x => x.CatererName == catererName)
                .SumAsync(x => x.UnitPrice * x.Quantity);

            var ratings = await _context.Ratings
                .Where(x => x.CatererName == catererName)
                .ToListAsync();

            ViewBag.AverageRating = ratings.Any()
                ? ratings.Average(x => x.CatererScore)
                : 0;

            ViewBag.RecentRatings = ratings
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .ToList();

            return View();
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserDashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.TotalOrders = await _context.Orders
                .CountAsync(x => x.UserId == user!.Id);

            ViewBag.TotalSpent = await _context.Orders
                .Where(x => x.UserId == user!.Id)
                .SumAsync(x => x.TotalAmount);

            ViewBag.TotalRatings = await _context.Ratings
                .CountAsync(x => x.UserId == user!.Id);

            return View();
        }
    }
}