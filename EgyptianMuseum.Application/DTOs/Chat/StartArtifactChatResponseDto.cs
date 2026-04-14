namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class StartArtifactChatResponseDto
    {
        public int ConversationId { get; set; }
        public int ArtifactId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExisting { get; set; }
    }
}
