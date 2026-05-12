using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly LogService _logService;

        public MenuController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
            _logService = logService;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? caterer,
            double? userLat,
            double? userLng,
            bool nearbyOnly = false,
            int page = 1)
        {
            if (User.IsInRole("User") && (!userLat.HasValue || !userLng.HasValue))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user?.Latitude != null && user.Longitude != null)
                {
                    userLat = user.Latitude;
                    userLng = user.Longitude;
                    nearbyOnly = true;
                }
            }

            var query = _context.MenuItems
                .Include(x => x.Caterer)
                .Include(x => x.Ratings)
                .Include(x => x.CustomizationOptions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(caterer))
            {
                query = query.Where(x =>
                    x.CatererName.Contains(caterer));
            }

            var menuItems = await query
                .OrderBy(x => x.Name)
                .ToListAsync();

            if (nearbyOnly && userLat.HasValue && userLng.HasValue)
            {
                menuItems = menuItems
                    .Where(x =>
                        x.Caterer?.Latitude != null &&
                        x.Caterer?.Longitude != null &&
                        CalculateDistance(
                            userLat.Value,
                            userLng.Value,
                            x.Caterer.Latitude.Value,
                            x.Caterer.Longitude.Value) <= 10)
                    .ToList();
            }

            const int pageSize = 6;
            var totalItems = menuItems.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(totalPages, 1)));

            var pagedMenuItems = menuItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.Caterer = caterer;
            ViewBag.UserLat = userLat;
            ViewBag.UserLng = userLng;
            ViewBag.NearbyOnly = nearbyOnly;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedMenuItems);
        }

        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _context.MenuItems
                .Include(x => x.Caterer)
                .Include(x => x.CustomizationOptions)
                .Include(x => x.Ratings!)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (menuItem == null)
                return NotFound();

            ViewBag.CatererAverage = await _context.Ratings
                .Where(x => x.CatererName == menuItem.CatererName)
                .Select(x => (double?)x.CatererScore)
                .AverageAsync() ?? 0;

            return View(menuItem);
        }

        [Authorize(Roles = "Caterer,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Create(MenuItem menuItem, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Image upload is required.");
            }
            else
            {
                ValidateImageFile(imageFile);
            }

            if (!ModelState.IsValid)
                return View(menuItem);

            var user = await _userManager.GetUserAsync(User);

            menuItem.CatererId = user?.Id;
            menuItem.CatererName = user?.FullName ?? "Mealinge Caterer";
            menuItem.ImageUrl = await SaveImage(imageFile!);

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(
                "Menu",
                user?.Email,
                $"Menu item '{menuItem.Name}' was created.");

            return RedirectToAction("Manage");
        }

        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            var query = _context.MenuItems.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(x => x.CatererId == user!.Id);
            }

            const int pageSize = 8;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(totalPages, 1)));

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);

            if (item == null)
                return NotFound();

            if (!await CanManageItem(item))
                return Forbid();

            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Edit(int id, MenuItem updatedItem, IFormFile? imageFile)
        {
            var item = await _context.MenuItems.FindAsync(id);

            if (item == null)
                return NotFound();

            if (!await CanManageItem(item))
                return Forbid();

            if (imageFile != null && imageFile.Length > 0)
            {
                ValidateImageFile(imageFile);
            }

            if (!ModelState.IsValid)
                return View(updatedItem);

            item.Name = updatedItem.Name;
            item.Description = updatedItem.Description;
            item.Price = updatedItem.Price;

            if (imageFile != null && imageFile.Length > 0)
            {
                item.ImageUrl = await SaveImage(imageFile);
            }

            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _logService.LogAsync(
                "Menu",
                user?.Email,
                $"Menu item '{item.Name}' was updated.");

            return RedirectToAction("Manage");
        }

        [HttpPost]
        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);

            if (item == null)
                return NotFound();

            if (!await CanManageItem(item))
                return Forbid();

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _logService.LogAsync(
                "Menu",
                user?.Email,
                $"Menu item '{item.Name}' was deleted.");

            return RedirectToAction("Manage");
        }

        private async Task<bool> CanManageItem(MenuItem item)
        {
            if (User.IsInRole("Admin"))
                return true;

            var user = await _userManager.GetUserAsync(User);

            return item.CatererId == user?.Id;
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(imageFile.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await imageFile.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        private void ValidateImageFile(IFormFile imageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageUrl", "Only JPG, PNG, and WEBP images are allowed.");
            }

            if (imageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageUrl", "Image size must be 2 MB or smaller.");
            }
        }

        private static double CalculateDistance(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            const double radius = 6371;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) *
                Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return radius * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
