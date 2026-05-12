namespace Ceng382_25_26_202311031.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = "";
        public string? CatererId { get; set; }
        public string CatererName { get; set; } = "";

        public decimal UnitPrice { get; set; }
        public decimal CustomizationPrice { get; set; }
        public string SelectedCustomizations { get; set; } = "";

        public int Quantity { get; set; }

        public decimal FinalUnitPrice => UnitPrice + CustomizationPrice;
        public decimal Subtotal => FinalUnitPrice * Quantity;
    }
}
