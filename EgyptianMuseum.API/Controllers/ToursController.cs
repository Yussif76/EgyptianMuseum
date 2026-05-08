using EgyptianMuseum.Application.DTOs.DtoTour;
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
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;

        public ToursController(ITourService tourService)
        {
            _tourService = tourService;
        }




        [HttpGet]
        public async Task<IActionResult> GetAllTours(
            [FromQuery] int estimatedTime,
            [FromQuery] int rooms,
            [FromQuery] string category)
        {
            var result = await _tourService.GetAllToursAsync(estimatedTime, rooms,category);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourById(int id)
        {
            var result = await _tourService.GetTourByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Tour not found." });

            return Ok(result);
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartTour(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _tourService.StartTourAsync(id, userId);

            if (!success)
                return NotFound(new { message = "Tour not found." });

            return Ok(new { message = "Tour started successfully." });
        }


        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelTour(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _tourService.CancelTourAsync(id, userId);

            if (!success)
                return NotFound(new { message = "Active tour not found." });

            return Ok(new { message = "Tour cancelled successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTour([FromBody] CreateTourDto dto)
        {
            var id = await _tourService.CreateTourAsync(dto);

            return Ok(new
            {
                message = "Tour created successfully",
                tourId = id
            });
        }
    }
}
