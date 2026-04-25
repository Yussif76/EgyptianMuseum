using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            
        }

        public async Task AddUserNotificationAsync(UserNotification userNotification)
        {
            await _context.UserNotifications.AddAsync(userNotification);
            
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.UserNotifications
                 .Where(un => un.UserId == userId)
                 .Include(un => un.Notification)
                 .OrderByDescending(un => un.Notification.CreatedAt)
                 .ToListAsync();
        }

        public async Task<UserNotification?> GetUserNotificationAsync(int notificationId, string userId)
        {
            return await _context.UserNotifications
                .FirstOrDefaultAsync(un => un.NotificationId == notificationId && un.UserId == userId);
        }

        public async Task DeleteUserNotificationAsync(UserNotification userNotification)
        {
            _context.UserNotifications.Remove(userNotification);
            
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
