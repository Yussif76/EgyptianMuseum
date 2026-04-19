using EgyptianMuseum.Domain.Enums;

namespace EgyptianMuseum.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public FeedbackTargetType TargetType { get; set; }
        public int TargetId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}
