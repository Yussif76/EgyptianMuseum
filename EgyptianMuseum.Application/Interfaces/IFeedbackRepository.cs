using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IFeedbackRepository
    {
        Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default);
        Task<List<Feedback>> GetByUserIdAsync(string? userId, CancellationToken cancellationToken = default);
        Task<List<Feedback>> GetByTargetAsync(FeedbackTargetType targetType, int? targetId, CancellationToken cancellationToken = default);
        Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
