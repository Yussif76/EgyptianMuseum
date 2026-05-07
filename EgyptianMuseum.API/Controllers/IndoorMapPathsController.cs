using EgyptianMuseum.Application.DTOs.Map;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/indoor-map-paths")]
    public class IndoorMapPathsController : ControllerBase
    {
        private readonly IIndoorMapPathService _pathService;

        public IndoorMapPathsController(IIndoorMapPathService pathService)
        {
            _pathService = pathService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var paths = await _pathService.GetAllAsync(cancellationToken);
                return Ok(new { success = true, data = paths });
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

                var path = await _pathService.GetByIdAsync(id, cancellationToken);
                if (path == null)
                    return NotFound(new { success = false, message = "Path not found" });

                return Ok(new { success = true, data = path });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("map/{mapId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByMapId(int mapId, CancellationToken cancellationToken)
        {
            try
            {
                if (mapId <= 0)
                    return BadRequest(new { success = false, message = "Invalid MapId" });

                var paths = await _pathService.GetByMapIdAsync(mapId, cancellationToken);
                return Ok(new { success = true, data = paths });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateIndoorMapPathRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be empty" });

                var path = await _pathService.CreateAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new { success = true, data = path });
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIndoorMapPathRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be empty" });

                var path = await _pathService.UpdateAsync(id, request, cancellationToken);
                return Ok(new { success = true, data = path });
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

                await _pathService.DeleteAsync(id, cancellationToken);
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
