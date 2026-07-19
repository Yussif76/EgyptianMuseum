using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class PieceImage : BaseEntity
    {
        public int PieceId { get; set; }
        public string ImagePath { get; set; }

        // Navigation property
        public Pieces Piece { get; set; }
    }
}
