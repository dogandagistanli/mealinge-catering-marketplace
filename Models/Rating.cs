namespace Ceng382_25_26_202311031.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } = "";
        public string CatererName { get; set; } = "";

        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }

        public int MenuItemScore { get; set; }
        public int CatererScore { get; set; }

        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}