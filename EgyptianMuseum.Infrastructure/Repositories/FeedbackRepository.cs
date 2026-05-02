using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Domain.Enums;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default)
        {
            await _context.Feedbacks.AddAsync(feedback, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Feedback>> GetByUserIdAsync(string? userId, CancellationToken cancellationToken = default)
        {
            var query = _context.Feedbacks.AsQueryable();

            // If userId provided, filter by that user; otherwise return all
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(f => f.UserId == userId);
            }

            return await query
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Feedback>> GetByTargetAsync(FeedbackTargetType targetType, int? targetId, CancellationToken cancellationToken = default)
        {
            var query = _context.Feedbacks.Where(f => f.TargetType == targetType);

            if (targetId.HasValue)
            {
                query = query.Where(f => f.TargetId == targetId);
            }

            return await query
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var feedback = await _context.Feedbacks.FindAsync(
                new object[] { id },
                cancellationToken: cancellationToken);

            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
