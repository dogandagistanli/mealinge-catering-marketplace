namespace Ceng382_25_26_202311031.Models
{
    public class EmailRecord
    {
        public int Id { get; set; }

        public string RecipientEmail { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}