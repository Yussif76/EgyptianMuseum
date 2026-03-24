namespace EgyptianMuseum.Application.DTOs.Chat
{
    public class ChatConversationDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? ArtifactId { get; set; }
        public string Type { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? LastMessagePreview { get; set; }
    }
}
