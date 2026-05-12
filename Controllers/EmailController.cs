using Ceng382_25_26_202311031.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 10;

            var query = _context.EmailRecords.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.RecipientEmail.Contains(search) ||
                    x.Subject.Contains(search) ||
                    x.Body.Contains(search));
            }

            var totalEmails = await query.CountAsync();

            var emails = await query
                .OrderByDescending(x => x.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalEmails / (double)pageSize);

            return View(emails);
        }
    }
}
