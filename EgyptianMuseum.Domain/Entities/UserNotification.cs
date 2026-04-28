using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class UserNotification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int NotificationId { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }

        public bool IsRead { get; set; } = false;



    }
}
