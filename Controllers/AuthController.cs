using System.Security.Cryptography;
using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly LogService _logService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            EmailService emailService,
            LogService logService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _logService = logService;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                await _logService.LogAsync(
                    "FAILED LOGIN",
                    model.Email,
                    "Login failed because user was not found.");

                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                await _logService.LogAsync(
                    "FAILED LOGIN",
                    user.Email,
                    "Login failed because password was incorrect.");

                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (user.EmailTwoFactorEnabled)
            {
                var activeCodes = await _context.TwoFactorCodes
                    .Where(x => x.UserId == user.Id && !x.IsUsed)
                    .ToListAsync();

                foreach (var activeCode in activeCodes)
                {
                    activeCode.IsUsed = true;
                }

                var code = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                _context.TwoFactorCodes.Add(new TwoFactorCode
                {
                    UserId = user.Id,
                    Code = code,
                    ExpiresAt = DateTime.Now.AddMinutes(5),
                    IsUsed = false
                });

                await _context.SaveChangesAsync();

                await _emailService.SendAsync(
                    user.Email ?? "",
                    "Mealinge Two-Factor Login Code",
                    $"Your Mealinge login verification code is: {code}\n\nThis code expires in 5 minutes.");

                HttpContext.Session.SetString("TwoFactorUserId", user.Id);
                HttpContext.Session.SetString("TwoFactorReturnUrl", model.ReturnUrl ?? "");

                return RedirectToAction("VerifyTwoFactor");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            await _logService.LogAsync(
                "SUCCESS LOGIN",
                user.Email,
                "User logged into the system.");

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Register(string? returnUrl = null)
        {
            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "This email is already registered.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                RoleDisplayName = "User",
                EmailConfirmed = true,
                EmailTwoFactorEnabled = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "User");

            await _logService.LogAsync(
                "REGISTER",
                user.Email,
                "New user account was created.");

            return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });
        }

        public IActionResult VerifyTwoFactor()
        {
            return View(new TwoFactorLoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFactor(TwoFactorLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetString("TwoFactorUserId");

            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login");

            var codeRecord = await _context.TwoFactorCodes
                .Where(x => x.UserId == userId && x.Code == model.Code && !x.IsUsed)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (codeRecord == null || codeRecord.ExpiresAt < DateTime.Now)
            {
                await _logService.LogAsync(
                    "FAILED TWO FACTOR LOGIN",
                    null,
                    $"Invalid or expired two-factor code was submitted for user id {userId}.");

                ModelState.AddModelError("", "Invalid or expired verification code.");
                return View(model);
            }

            codeRecord.IsUsed = true;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return RedirectToAction("Login");

            await _signInManager.SignInAsync(user, isPersistent: false);

            await _logService.LogAsync(
                "SUCCESS TWO FACTOR LOGIN",
                user.Email,
                "User completed two-factor authentication.");

            var returnUrl = HttpContext.Session.GetString("TwoFactorReturnUrl");

            HttpContext.Session.Remove("TwoFactorUserId");
            HttpContext.Session.Remove("TwoFactorReturnUrl");

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _logService.LogAsync(
                    "LOGOUT",
                    user.Email,
                    "User logged out from the system.");
            }

            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
