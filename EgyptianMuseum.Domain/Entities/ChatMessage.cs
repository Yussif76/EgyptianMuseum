using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public SenderType SenderType { get; set; }
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; }
        
        public virtual ChatConversation Conversation { get; set; } = null!;
    }
}
