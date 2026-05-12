namespace Ceng382_25_26_202311031.Models
{
    public class CartItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = "";
        public string? CatererId { get; set; }
        public string CatererName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = "";

        public string SelectedCustomizations { get; set; } = "";
        public decimal CustomizationPrice { get; set; }

        public decimal FinalUnitPrice => UnitPrice + CustomizationPrice;
        public decimal TotalPrice => FinalUnitPrice * Quantity;
    }
}
