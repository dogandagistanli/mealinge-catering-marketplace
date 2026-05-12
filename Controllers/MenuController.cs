using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
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

        public MenuController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? search, string? caterer)
        {
            var query = _context.MenuItems
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

            ViewBag.Search = search;
            ViewBag.Caterer = caterer;

            return View(menuItems);
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

            if (!ModelState.IsValid)
                return View(menuItem);

            var user = await _userManager.GetUserAsync(User);

            menuItem.CatererId = user?.Id;
            menuItem.CatererName = user?.FullName ?? "Mealinge Caterer";
            menuItem.ImageUrl = await SaveImage(imageFile);

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Manage");
        }

        [Authorize(Roles = "Caterer,Admin")]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);

            var query = _context.MenuItems.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(x => x.CatererId == user!.Id);
            }

            var items = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

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
    }
}