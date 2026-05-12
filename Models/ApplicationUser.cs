using Microsoft.AspNetCore.Identity;

namespace Ceng382_25_26_202311031.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public string RoleDisplayName { get; set; } = "";

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool EmailTwoFactorEnabled { get; set; }
    }
}