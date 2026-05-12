using Ceng382_25_26_202311031.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    public class NearbyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NearbyController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var caterers = await _context.Users
                .Where(x => x.RoleDisplayName == "Caterer" && x.Latitude != null && x.Longitude != null)
                .Select(x => new
                {
                    x.FullName,
                    x.Email,
                    x.Latitude,
                    x.Longitude
                })
                .ToListAsync();

            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.Caterers = caterers;

            return View();
        }
    }
}