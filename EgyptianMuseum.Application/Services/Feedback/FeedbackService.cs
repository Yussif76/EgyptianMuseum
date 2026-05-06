using EgyptianMuseum.Application.DTOs.Feedback;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Application.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IPiecesRepository<Pieces> _pieceRepository;
        private readonly IChatConversationRepository _chatConversationRepository;

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            IPiecesRepository<Pieces> pieceRepository,
            IChatConversationRepository chatConversationRepository)
        {
            _feedbackRepository = feedbackRepository;
            _pieceRepository = pieceRepository;
            _chatConversationRepository = chatConversationRepository;
        }

        public async Task<FeedbackDto> CreateAsync(
            string userId,
            CreateFeedbackRequestDto request,
            CancellationToken cancellationToken = default)
        {
            // Validate and parse target type
            if (!Enum.TryParse<FeedbackTargetType>(request.TargetType, true, out var targetType))
            {
                throw new ArgumentException($"Invalid target type: {request.TargetType}");
            }

            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            // Validate comment
            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                throw new ArgumentException("Comment cannot be empty");
            }

            // Validate targetId based on target type
            if (targetType == FeedbackTargetType.App)
            {
                if (request.TargetId.HasValue)
                {
                    throw new ArgumentException("Target ID must be null for App feedback");
                }
            }
            else if (targetType == FeedbackTargetType.Artifact)
            {
                if (!request.TargetId.HasValue)
                {
                    throw new ArgumentException("Target ID is required for Artifact feedback");
                }

                var piece = await _pieceRepository.GetByIdAsync(request.TargetId.Value, cancellationToken);
                if (piece == null)
                {
                    throw new KeyNotFoundException($"Artifact with ID {request.TargetId} not found");
                }
            }
            else if (targetType == FeedbackTargetType.Chat)
            {
                if (!request.TargetId.HasValue)
                {
                    throw new ArgumentException("Target ID is required for Chat feedback");
                }

                var conversation = await _chatConversationRepository.GetByIdAsync(request.TargetId.Value, cancellationToken);
                if (conversation == null)
                {
                    throw new KeyNotFoundException($"Chat conversation with ID {request.TargetId} not found");
                }
            }

            // Create feedback record
            var feedbackEntity = new Domain.Entities.Feedback
            {
                UserId = userId,
                TargetType = targetType,
                TargetId = request.TargetId,
                Rating = request.Rating,
                Comment = request.Comment?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _feedbackRepository.AddAsync(feedbackEntity, cancellationToken);

            return new FeedbackDto
            {
                Id = feedbackEntity.Id,
                TargetType = feedbackEntity.TargetType.ToString(),
                TargetId = feedbackEntity.TargetId,
                Rating = feedbackEntity.Rating,
                Comment = feedbackEntity.Comment,
                CreatedAt = feedbackEntity.CreatedAt
            };
        }

        public async Task<List<FeedbackDto>> GetUserFeedbackAsync(
            string? userId,
            CancellationToken cancellationToken = default)
        {
            var feedbackList = await _feedbackRepository.GetByUserIdAsync(userId, cancellationToken);

            return feedbackList
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    TargetType = f.TargetType.ToString(),
                    TargetId = f.TargetId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToList();
        }

        public async Task<List<FeedbackDto>> GetByTargetAsync(
            string? userId,
            string targetType,
            int? targetId,
            CancellationToken cancellationToken = default)
        {
            // Parse target type
            if (!Enum.TryParse<FeedbackTargetType>(targetType, true, out var parsedTargetType))
            {
                throw new ArgumentException($"Invalid target type: {targetType}");
            }

            // Validate targetId based on target type
            if (parsedTargetType == FeedbackTargetType.App && targetId.HasValue)
            {
                throw new ArgumentException("Target ID must be null for App feedback");
            }

            if (parsedTargetType != FeedbackTargetType.App && !targetId.HasValue)
            {
                throw new ArgumentException($"Target ID is required for {targetType} feedback");
            }

            var feedbackList = await _feedbackRepository.GetByTargetAsync(parsedTargetType, targetId, cancellationToken);

            // If user is authenticated, filter by userId (user's own feedback)
            // If user is not authenticated, return all feedback
            if (!string.IsNullOrEmpty(userId))
            {
                feedbackList = feedbackList
                    .Where(f => f.UserId == userId)
                    .ToList();
            }

            return feedbackList
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    TargetType = f.TargetType.ToString(),
                    TargetId = f.TargetId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToList();
        }

        public async Task DeleteAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id, cancellationToken);
            if (feedback == null)
            {
                throw new KeyNotFoundException("Feedback not found");
            }

            if (feedback.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this feedback");
            }

            await _feedbackRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
