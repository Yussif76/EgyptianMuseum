using EgyptianMuseum.Application.DTOs.Feedback;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> CreateAsync(string userId, CreateFeedbackRequestDto request, CancellationToken cancellationToken = default);
        Task<List<FeedbackDto>> GetUserFeedbackAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<FeedbackDto>> GetByTargetAsync(string userId, string targetType, int targetId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default);
    }
}
