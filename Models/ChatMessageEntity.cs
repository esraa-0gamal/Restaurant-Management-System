namespace RestaurantSystem.Models
{
    public class ChatMessageEntity
    {
        public int Id { get; set; }
        public string Role { get; set; } = ""; // "user", "assistant", "system"
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
