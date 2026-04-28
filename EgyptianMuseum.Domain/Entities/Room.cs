using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public double XCord { get; set; }
        public double YCord { get; set; }

        public ICollection<RoomTour> RoomTours { get; set; } = new List<RoomTour>();
        
    }
}
