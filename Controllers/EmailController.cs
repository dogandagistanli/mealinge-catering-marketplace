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

        public async Task<IActionResult> Index()
        {
            var emails = await _context.EmailRecords
                .OrderByDescending(x => x.SentAt)
                .ToListAsync();

            return View(emails);
        }
    }
}