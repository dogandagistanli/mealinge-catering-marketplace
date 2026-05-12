using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;
using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Controllers
{
    public class NearbyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogService _logService;

        public NearbyController(
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            LogService logService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _logService = logService;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveUserLocation([FromBody] UserLocationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            user.Latitude = request.Latitude;
            user.Longitude = request.Longitude;

            await _userManager.UpdateAsync(user);

            await _logService.LogAsync(
                "Location",
                user.Email,
                "User location was updated for nearby restaurant filtering.");

            return Ok();
        }
    }

    public class UserLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
