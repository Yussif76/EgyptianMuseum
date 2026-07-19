namespace EgyptianMuseum.Application.DTOs.Pieces
{
    public class PieceWithScannedStatusDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> PhotoPaths { get; set; } = new();
        public string TextNarration { get; set; } = null!;
        public string Period { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int? GalleryNum { get; set; }
        public string Collection { get; set; }
        public List<PieceLocationDto>? PieceLocation { get; set; }

        // Scanned status info
        public int? ScannedArtifactId { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime? ScannedAt { get; set; }
    }
}
