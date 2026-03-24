using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;

        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            await _context.ChatMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ChatMessage>> GetByConversationIdAsync(
            int conversationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ChatMessage?> GetLastMessageAsync(
            int conversationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
