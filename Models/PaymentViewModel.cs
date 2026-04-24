using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class PaymentViewModel
    {
        [Required]
        public string CardHolderName { get; set; } = "";

        [Required]
        public string CardNumber { get; set; } = "";

        [Required]
        public string ExpiryDate { get; set; } = "";

        [Required]
        public string CVV { get; set; } = "";
    }
}