using EgyptianMuseum.Application.DTOs.DtoRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.DtoTour
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<RoomDto> Rooms { get; set; } = new List<RoomDto>();
    }
}
