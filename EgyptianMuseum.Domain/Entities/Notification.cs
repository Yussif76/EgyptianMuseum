using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Domain.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}

