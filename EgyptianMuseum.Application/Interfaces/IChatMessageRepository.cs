using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IChatMessageRepository
    {
        Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);
        Task<List<ChatMessage>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default);
        Task<ChatMessage?> GetLastMessageAsync(int conversationId, CancellationToken cancellationToken = default);
    }
}
