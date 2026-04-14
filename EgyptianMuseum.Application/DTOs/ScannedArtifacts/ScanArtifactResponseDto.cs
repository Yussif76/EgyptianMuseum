namespace EgyptianMuseum.Application.DTOs.ScannedArtifacts
{
    public class ScanArtifactResponseDto
    {
        public int ScannedArtifactId { get; set; }
        public int PieceId { get; set; }
        public string LabelText { get; set; } = null!;
        public bool IsFavorite { get; set; }
        public DateTime ScannedAt { get; set; }
        public string? PieceName { get; set; }
        public string? PieceDescription { get; set; }
        public string? PieceImageUrl { get; set; }
        public string? PiecePeriod { get; set; }
        public string? PieceCategory { get; set; }
    }
}
