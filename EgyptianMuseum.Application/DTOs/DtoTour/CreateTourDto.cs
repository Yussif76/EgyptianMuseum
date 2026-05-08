using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.DtoTour
{
    public class CreateTourDto
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public int DurationInMinutes { get; set; }
        public List<int> RoomIds { get; set; } = new List<int>();
    }
}
