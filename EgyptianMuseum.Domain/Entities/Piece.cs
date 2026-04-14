namespace EgyptianMuseum.Domain.Entities
{
    public class Piece
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string LabelText { get; set; } = null!;
        public string? Period { get; set; }
        public string? Category { get; set; }

        public virtual ICollection<ScannedArtifact> ScannedArtifacts { get; set; } = new List<ScannedArtifact>();
    }
}
