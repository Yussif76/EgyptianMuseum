using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.Chat
{
    public class MockAiChatService : IAiChatService
    {
        public Task<string> GenerateReplyAsync(
            ChatConversation conversation,
            string userMessage,
            CancellationToken cancellationToken = default)
        {
            var reply = $"This is a mock AI reply for: {userMessage}";
            return Task.FromResult(reply);
        }
    }
}
