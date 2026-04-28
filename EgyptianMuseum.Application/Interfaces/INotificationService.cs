using EgyptianMuseum.Application.DTOs.NotificationsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(CreateNotificationDto dto);
        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task DeleteNotificationAsync(int notificationId, string userId);
        Task<NotificationResponseDto> GetNotificationByIdAsync(int notificationId, string userId);
    }
}
