using EgyptianMuseum.Application.DTOs.Chat;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatConversationRepository _conversationRepository;
        private readonly IChatMessageRepository _messageRepository;
        private readonly IAiChatService _aiChatService;

        public ChatService(
            IChatConversationRepository conversationRepository,
            IChatMessageRepository messageRepository,
            IAiChatService aiChatService)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _aiChatService = aiChatService;
        }

        public async Task<StartGeneralChatResponseDto> StartGeneralChatAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var conversation = new ChatConversation
            {
                UserId = userId,
                Type = ConversationType.General,
                ArtifactId = null,
                Title = "New Chat",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _conversationRepository.AddAsync(conversation, cancellationToken);

            return new StartGeneralChatResponseDto
            {
                ConversationId = conversation.Id,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt
            };
        }

        public async Task<SendMessageResponseDto> SendMessageAsync(
            string userId,
            int conversationId,
            SendMessageRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                throw new ArgumentException("Message cannot be empty");
            }

            // Verify conversation exists and belongs to user
            var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found");
            }

            if (conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation");
            }

            // Save user message
            var userMessage = new ChatMessage
            {
                ConversationId = conversationId,
                SenderType = SenderType.User,
                Text = request.Message,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(userMessage, cancellationToken);

            // Generate AI reply
            var botReply = await _aiChatService.GenerateReplyAsync(conversation, request.Message, cancellationToken);

            // Save bot message
            var botMessage = new ChatMessage
            {
                ConversationId = conversationId,
                SenderType = SenderType.Bot,
                Text = botReply,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(botMessage, cancellationToken);

            // Update conversation timestamp
            conversation.UpdatedAt = DateTime.UtcNow;
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            return new SendMessageResponseDto
            {
                ConversationId = conversationId,
                UserMessage = request.Message,
                BotReply = botReply,
                SentAt = userMessage.SentAt
            };
        }

        public async Task<List<ChatConversationDto>> GetUserConversationsAsync(
            string userId,
            int skip,
            int take,
            CancellationToken cancellationToken = default)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(
                userId,
                skip,
                take,
                cancellationToken);

            var dtos = new List<ChatConversationDto>();
            foreach (var conv in conversations)
            {
                var lastMessage = await _messageRepository.GetLastMessageAsync(conv.Id, cancellationToken);

                dtos.Add(new ChatConversationDto
                {
                    Id = conv.Id,
                    Title = conv.Title,
                    ArtifactId = conv.ArtifactId,
                    Type = conv.Type.ToString(),
                    CreatedAt = conv.CreatedAt,
                    UpdatedAt = conv.UpdatedAt,
                    LastMessagePreview = lastMessage?.Text.Length > 50
                        ? lastMessage.Text.Substring(0, 50) + "..."
                        : lastMessage?.Text
                });
            }

            return dtos;
        }

        public async Task<List<ChatMessageDto>> GetConversationMessagesAsync(
            string userId,
            int conversationId,
            CancellationToken cancellationToken = default)
        {
            // Verify ownership
            var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found");
            }

            if (conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation");
            }

            var messages = await _messageRepository.GetByConversationIdAsync(conversationId, cancellationToken);

            return messages
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderType = m.SenderType.ToString(),
                    Text = m.Text,
                    SentAt = m.SentAt
                })
                .ToList();
        }

        public async Task DeleteConversationAsync(
            string userId,
            int conversationId,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found");
            }

            if (conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation");
            }

            await _conversationRepository.DeleteAsync(conversationId, cancellationToken);
        }

        public async Task UpdateConversationTitleAsync(
            string userId,
            int conversationId,
            string title,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found");
            }

            if (conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this conversation");
            }

            conversation.Title = title.Trim();
            conversation.UpdatedAt = DateTime.UtcNow;

            await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        }
    }
}
