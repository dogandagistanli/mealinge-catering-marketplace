using Ceng382_25_26_202311031.Data;
using Ceng382_25_26_202311031.Models;

namespace Ceng382_25_26_202311031.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string action,
            string? userEmail,
            string description)
        {
            var log = new AppLog
            {
                Action = action,
                UserEmail = userEmail,
                Description = description,
                CreatedAt = DateTime.Now
            };

            _context.AppLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}