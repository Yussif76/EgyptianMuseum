using EgyptianMuseum.Application.DTOs.Chat;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }

        [HttpPost("general")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> StartGeneralChat(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var result = await _chatService.StartGeneralChatAsync(userId, cancellationToken);
                return CreatedAtAction(nameof(GetConversationMessages), new { conversationId = result.ConversationId }, result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }

        [HttpPost("{conversationId}/messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendMessage(
            int conversationId,
            [FromBody] SendMessageRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (conversationId <= 0)
                    return BadRequest(new { success = false, message = "Invalid conversation ID" });

                if (string.IsNullOrWhiteSpace(request?.Message))
                    return BadRequest(new { success = false, message = "Message cannot be empty" });

                var userId = GetUserId();
                var result = await _chatService.SendMessageAsync(userId, conversationId, request, cancellationToken);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{conversationId}/messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetConversationMessages(
            int conversationId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (conversationId <= 0)
                    return BadRequest(new { success = false, message = "Invalid conversation ID" });

                var userId = GetUserId();
                var result = await _chatService.GetConversationMessagesAsync(userId, conversationId, cancellationToken);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }
        }

        [HttpGet("conversations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetConversations(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (skip < 0) skip = 0;
                if (take <= 0) take = 20;
                if (take > 100) take = 100;

                var userId = GetUserId();
                var result = await _chatService.GetUserConversationsAsync(userId, skip, take, cancellationToken);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }

        [HttpPut("{conversationId}/title")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTitle(
            int conversationId,
            [FromBody] UpdateChatTitleRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (conversationId <= 0)
                    return BadRequest(new { success = false, message = "Invalid conversation ID" });

                if (string.IsNullOrWhiteSpace(request?.Title))
                    return BadRequest(new { success = false, message = "Title cannot be empty" });

                var userId = GetUserId();
                await _chatService.UpdateConversationTitleAsync(userId, conversationId, request.Title, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{conversationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteConversation(
            int conversationId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (conversationId <= 0)
                    return BadRequest(new { success = false, message = "Invalid conversation ID" });

                var userId = GetUserId();
                await _chatService.DeleteConversationAsync(userId, conversationId, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Conversation not found" });
            }
        }
    }
}
