namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class StartGeneralChatResponseDto
    {
        public int ConversationId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
