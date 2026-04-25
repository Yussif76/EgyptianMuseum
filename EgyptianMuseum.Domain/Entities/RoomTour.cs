using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class RoomTour
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }  

        [Required]
        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public Tour Tour { get; set; } 
    }
}
