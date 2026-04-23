namespace Ceng382_25_26_202311031.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public string CatererName { get; set; } = "";

        public string? CatererId { get; set; }
        public ApplicationUser? Caterer { get; set; }
    }
}