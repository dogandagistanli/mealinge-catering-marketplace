using System.Security.Claims;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Services
{
    public class OrderAccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderAccessService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Order?> GetAccessiblePaidOrderAsync(
            ClaimsPrincipal principal,
            int orderId)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
                return null;

            var order = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == orderId && x.Status == "Paid");

            if (order == null)
                return null;

            if (order.UserId == user.Id)
                return order;

            if (principal.IsInRole("Caterer") && IsOrderForCaterer(order, user))
                return order;

            return null;
        }

        public async Task<bool> CanAccessPaidOrderAsync(
            ClaimsPrincipal principal,
            int orderId)
        {
            return await GetAccessiblePaidOrderAsync(principal, orderId) != null;
        }

        public static bool IsOrderForCaterer(Order order, ApplicationUser user)
        {
            return order.OrderItems.Any(x =>
                (!string.IsNullOrWhiteSpace(x.CatererId) && x.CatererId == user.Id) ||
                (!string.IsNullOrWhiteSpace(x.CatererName) && x.CatererName == user.FullName));
        }
    }
}
