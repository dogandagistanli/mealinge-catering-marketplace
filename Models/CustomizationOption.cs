using System.ComponentModel.DataAnnotations;

namespace Ceng382_25_26_202311031.Models
{
    public class CustomizationOption
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        [Required]
        public string GroupName { get; set; } = "";

        [Required]
        public string OptionName { get; set; } = "";

        public string OptionType { get; set; } = "Addition";

        public decimal PriceChange { get; set; }
    }
}