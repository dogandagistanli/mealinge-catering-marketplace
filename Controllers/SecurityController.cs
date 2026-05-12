using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly LogService _logService;

        public SecurityController(
            UserManager<ApplicationUser> userManager,
            EmailService emailService,
            LogService logService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEmailTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            user.EmailTwoFactorEnabled = !user.EmailTwoFactorEnabled;

            await _userManager.UpdateAsync(user);

            await _emailService.SendAsync(
                user.Email ?? "unknown@mealinge.com",
                "Mealinge Security Setting Updated",
                $"Email two-factor authentication is now {(user.EmailTwoFactorEnabled ? "enabled" : "disabled")} for your account.");

            await _logService.LogAsync(
                "Security",
                user.Email,
                $"Email two-factor authentication was {(user.EmailTwoFactorEnabled ? "enabled" : "disabled")}.");

            return RedirectToAction("Index");
        }
    }
}