namespace EgyptianMuseum.Domain.Entities
{
    public class TourPiece
    {
        public int TourId { get; set; }

        public Tour Tour { get; set; } = null!;

        public int PieceId { get; set; }

        public Pieces Piece { get; set; } = null!;
    }
}
