using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.Pieces
{
    public class UpdatePiecesRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public List<PieceTranslationRequest> Translations { get; set; } = new();

    }
}
