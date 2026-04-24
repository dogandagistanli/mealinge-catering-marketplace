using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; } = "Paid";

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}