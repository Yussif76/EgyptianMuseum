namespace EgyptianMuseum.Domain.Entities
{
    public class ScannedArtifact
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int PieceId { get; set; }
        public string LabelText { get; set; } = null!;
        public bool IsFavorite { get; set; }
        public DateTime ScannedAt { get; set; }

        public virtual Pieces Piece { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}