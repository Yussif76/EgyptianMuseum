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
        public string PhotoPath { get; set; }
        public string TextNarration { get; set; }
        public string Period { get; set; }
        public string Category { get; set; }

    }
}
