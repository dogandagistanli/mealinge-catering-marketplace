namespace Ceng382_25_26_202311031.Models
{
    public class TwoFactorCode
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }

        public string Code { get; set; } = "";

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }
    }
}