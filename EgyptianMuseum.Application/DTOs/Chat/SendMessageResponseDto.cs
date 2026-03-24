namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class SendMessageResponseDto
    {
        public int ConversationId { get; set; }
        public string UserMessage { get; set; } = null!;
        public string BotReply { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}
