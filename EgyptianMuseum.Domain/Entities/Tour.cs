using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{

    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }


        public ICollection<RoomTour> RoomTours { get; set; } = new List<RoomTour>();
        public ICollection<UserTour> UserTours { get; set; } = new List<UserTour>();
    }
}
