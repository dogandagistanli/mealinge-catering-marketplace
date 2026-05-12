using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class TwoFactorLoginViewModel
    {
        [Required]
        public string Code { get; set; } = "";
    }
}