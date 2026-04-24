namespace Ceng382_25_26_202311031.Models
{
    public class CartItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = "";
        public string CatererName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = "";

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}