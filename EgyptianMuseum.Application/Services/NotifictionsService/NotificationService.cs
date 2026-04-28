using EgyptianMuseum.Application.DTOs.NotificationsDto;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Services.NotifictionsService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task SendNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Title = dto.Title,
                Body = dto.Body,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddNotificationAsync(notification);


            foreach (var userId in dto.UserIds)
            {
                var userNotification = new UserNotification
                {
                    UserId = userId,
                    Notification = notification,
                    IsRead = false
                };
                await _notificationRepository.AddUserNotificationAsync(userNotification);
            }

            await _notificationRepository.SaveChangesAsync();
        }

        public async Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string userId)
        {
            var userNotifications = await _notificationRepository.GetUserNotificationsAsync(userId);

            return userNotifications.Select(un => new NotificationResponseDto
            {
                Id = un.Notification.Id,
                Title = un.Notification.Title,
                Body = un.Notification.Body,
                IsRead = un.IsRead,
                CreatedAt = un.Notification.CreatedAt
            }).ToList();
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var userNotification = await _notificationRepository.GetUserNotificationAsync(notificationId, userId);

            if (userNotification == null)
                throw new InvalidOperationException("Notification not found");

            userNotification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId, string userId)
        {
            var userNotification = await _notificationRepository.GetUserNotificationAsync(notificationId, userId);

            if (userNotification == null)
                throw new InvalidOperationException("Notification not found");

            await _notificationRepository.DeleteUserNotificationAsync(userNotification);
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task<NotificationResponseDto> GetNotificationByIdAsync(int notificationId, string userId)
        {
            var userNotification = await _notificationRepository.GetUserNotificationAsync(notificationId, userId);
            if (userNotification == null)
                throw new InvalidOperationException("Notification not found");
            return new NotificationResponseDto
            {
                Id = userNotification.Notification.Id,
                Title = userNotification.Notification.Title,
                Body = userNotification.Notification.Body,
                IsRead = userNotification.IsRead,
                CreatedAt = userNotification.Notification.CreatedAt
            };

        }
    }
}
