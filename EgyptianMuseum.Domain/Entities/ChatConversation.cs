using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Domain.Entities
{
    public class ChatConversation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ConversationType Type { get; set; }
        public int? ArtifactId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
