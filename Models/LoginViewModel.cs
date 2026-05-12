using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}