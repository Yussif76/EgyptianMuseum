using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class PieceTranslation: BaseEntity
    {
        public int PieceId { get; set; }
        public Pieces Piece { get; set; }
        public string LanguageCode { get; set; }
        public string TextNarration { get; set; }
        public string Name { get; set; }
        public string? Period { get; set; }
        public string? Category { get; set; }
    }
}
