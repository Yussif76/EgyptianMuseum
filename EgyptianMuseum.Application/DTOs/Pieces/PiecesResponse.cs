using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.Pieces
{
    public class PiecesResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<string> PhotoPaths { get; set; } = new();
        public string TextNarration { get; set; }
        public string Period { get; set; }
        public string Category { get; set; }
        public int? GalleryNum { get; set; }
        public string Collection { get; set; }
        public List<PieceLocationDto>? PieceLocation { get; set; }

    }
}
