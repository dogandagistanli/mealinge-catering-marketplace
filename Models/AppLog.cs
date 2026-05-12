namespace Ceng382_25_26_202311031.Models
{
    public class AppLog
    {
        public int Id { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? UserEmail { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}