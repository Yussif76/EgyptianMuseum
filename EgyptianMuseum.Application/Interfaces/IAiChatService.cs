using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IAiChatService
    {
        Task<string> GenerateReplyAsync(ChatConversation conversation, string userMessage, CancellationToken cancellationToken = default);
    }
}
