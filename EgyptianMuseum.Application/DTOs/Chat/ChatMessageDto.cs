namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string SenderType { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}
