using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class CallsController : Controller
    {
        private readonly OrderAccessService _orderAccessService;

        public CallsController(OrderAccessService orderAccessService)
        {
            _orderAccessService = orderAccessService;
        }

        public async Task<IActionResult> Order(int id)
        {
            var order = await _orderAccessService.GetAccessiblePaidOrderAsync(User, id);

            if (order == null)
                return NotFound();

            ViewBag.OrderId = order.Id;
            ViewBag.CustomerEmail = order.User?.Email ?? "Customer";
            ViewBag.Caterers = order.OrderItems
                .Select(x => x.CatererName)
                .Distinct()
                .ToList();

            return View(order);
        }
    }
}
