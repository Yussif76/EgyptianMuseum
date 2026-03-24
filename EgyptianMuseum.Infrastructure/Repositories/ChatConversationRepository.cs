using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class ChatConversationRepository : IChatConversationRepository
    {
        private readonly AppDbContext _context;

        public ChatConversationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatConversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ChatConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<ChatConversation>> GetUserConversationsAsync(
            string userId,
            int skip,
            int take,
            CancellationToken cancellationToken = default)
        {
            return await _context.ChatConversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.UpdatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
        {
            await _context.ChatConversations.AddAsync(conversation, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
        {
            _context.ChatConversations.Update(conversation);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int conversationId, CancellationToken cancellationToken = default)
        {
            var conversation = await _context.ChatConversations.FindAsync(
                new object[] { conversationId },
                cancellationToken: cancellationToken);

            if (conversation != null)
            {
                _context.ChatConversations.Remove(conversation);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
