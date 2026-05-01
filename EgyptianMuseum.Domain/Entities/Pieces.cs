using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class Pieces:BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public ICollection<PieceTranslation> Translations { get; set; } = new List<PieceTranslation>();
        public ICollection<ScannedArtifact> ScannedArtifacts { get; set; } = new List<ScannedArtifact>();

    }
}
