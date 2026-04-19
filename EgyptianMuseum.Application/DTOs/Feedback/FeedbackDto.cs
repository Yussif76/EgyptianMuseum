namespace EgyptianMuseum.Application.DTOs.Feedback
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public string TargetType { get; set; } = null!;
        public int TargetId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
