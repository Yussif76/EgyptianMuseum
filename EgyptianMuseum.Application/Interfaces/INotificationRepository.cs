using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task AddUserNotificationAsync(UserNotification userNotification);
        Task<List<UserNotification>> GetUserNotificationsAsync(string userId);
        Task<UserNotification?> GetUserNotificationAsync(int notificationId, string userId);
        Task DeleteUserNotificationAsync(UserNotification userNotification);
        Task SaveChangesAsync();
    }
}
