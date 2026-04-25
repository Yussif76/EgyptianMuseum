using System.ComponentModel.DataAnnotations;

namespace EgyptianMuseum.Application.DTOs.Feedback
{
    public class CreateFeedbackRequestDto
    {
        [Required(ErrorMessage = "Target type is required")]
        [RegularExpression("^(Artifact|Chat|App)$", ErrorMessage = "Target type must be 'Artifact', 'Chat', or 'App'")]
        public string TargetType { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Target ID must be greater than 0 when required")]
        public int? TargetId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        public string? Comment { get; set; }
    }
}
