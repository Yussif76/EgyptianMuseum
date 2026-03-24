using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IChatConversationRepository
    {
        Task<ChatConversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ChatConversation>> GetUserConversationsAsync(string userId, int skip, int take, CancellationToken cancellationToken = default);
        Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken = default);
        Task UpdateAsync(ChatConversation conversation, CancellationToken cancellationToken = default);
        Task DeleteAsync(int conversationId, CancellationToken cancellationToken = default);
    }
}
