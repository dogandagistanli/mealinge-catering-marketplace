using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _context.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.UserId == user!.Id)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}