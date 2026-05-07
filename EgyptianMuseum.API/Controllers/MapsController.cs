using EgyptianMuseum.Application.DTOs.Map;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapService _mapService;

        public MapsController(IMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var maps = await _mapService.GetAllAsync(cancellationToken);
                return Ok(new { success = true, data = maps });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var map = await _mapService.GetByIdAsync(id, cancellationToken);
                if (map == null)
                    return NotFound(new { success = false, message = "Map not found" });

                return Ok(new { success = true, data = map });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("zone/{zone}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByZone(string zone, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(zone))
                    return BadRequest(new { success = false, message = "Zone cannot be empty" });

                var maps = await _mapService.GetByZoneAsync(zone, cancellationToken);
                return Ok(new { success = true, data = maps });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMapRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be empty" });

                var map = await _mapService.CreateAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new { success = true, data = map });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMapRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be empty" });

                var map = await _mapService.UpdateAsync(id, request, cancellationToken);
                return Ok(new { success = true, data = map });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                await _mapService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
