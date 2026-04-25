using EgyptianMuseum.Application.DTOs.NotificationsDto;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgyptianMuseum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
            if (notification == null)
                return NotFound(new { message = "Notification not found." });
            return Ok(notification);
        }


        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { message = "Notification marked as read" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationService.DeleteNotificationAsync(id, userId);
            return Ok(new { message = "Notification deleted successfully" });
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] CreateNotificationDto dto)
        {
            await _notificationService.SendNotificationAsync(dto);
            return Ok(new { message = "Notification sent successfully" });
        }
    }
}
