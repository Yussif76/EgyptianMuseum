using EgyptianMuseum.Application.DTOs.Chat;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IChatService
    {
        Task<StartGeneralChatResponseDto> StartGeneralChatAsync(string userId, CancellationToken cancellationToken = default);
        Task<SendMessageResponseDto> SendMessageAsync(string userId, int conversationId, SendMessageRequestDto request, CancellationToken cancellationToken = default);
        Task<List<ChatConversationDto>> GetUserConversationsAsync(string userId, int skip, int take, CancellationToken cancellationToken = default);
        Task<List<ChatMessageDto>> GetConversationMessagesAsync(string userId, int conversationId, CancellationToken cancellationToken = default);
        Task DeleteConversationAsync(string userId, int conversationId, CancellationToken cancellationToken = default);
        Task UpdateConversationTitleAsync(string userId, int conversationId, string title, CancellationToken cancellationToken = default);
    }
}
