using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.NotificationsDto
{
    public class CreateNotificationDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
        [Required]
        public List<string> UserIds { get; set; }=  new List<string>();
    }
}
