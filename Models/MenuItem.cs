using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string CatererName { get; set; } = string.Empty;

        public string? CatererId { get; set; }

        public ApplicationUser? Caterer { get; set; }

        public ICollection<Rating>? Ratings { get; set; }
        public ICollection<CustomizationOption>? CustomizationOptions { get; set; }
    }
}