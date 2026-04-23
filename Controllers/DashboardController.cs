using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin");

            if (User.IsInRole("Caterer"))
                return RedirectToAction("Caterer");

            return RedirectToAction("UserDashboard");
        }

        [Authorize]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public IActionResult Caterer()
        {
            return View();
        }

        [Authorize]
        public IActionResult UserDashboard()
        {
            return View();
        }
    }
}