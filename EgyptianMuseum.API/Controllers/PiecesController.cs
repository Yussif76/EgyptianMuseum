using AutoMapper;
using EgyptianMuseum.Application.DTOs.Pieces;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgyptianMuseum.API.Controllers
{
    //[Route("Pieces")]
    [Route("api/[controller]")]
    [ApiController]
    public class PiecesController : Controller
    {
        private readonly IPiecesServices _service;
        private readonly ILogger<PiecesController> _logger;
        private readonly IMapper _mapper;

        public PiecesController(IPiecesServices service, ILogger<PiecesController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        private string? GetUserIdFromToken()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetPieces(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string lang = "en")
        {
            _logger.LogInformation("Fetching pieces - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var pieces = await _service.GetPagedWithTranslationsAsync(page, pageSize);

            if (!pieces.Any())
            {
                _logger.LogWarning("No pieces found");
                return NotFound("No pieces found.");
            }

            var response = pieces.Select(piece =>
            {
                var translation = piece.Translations
                    .FirstOrDefault(x => x.LanguageCode == lang);

                return new PiecesResponse
                {
                    Id = piece.Id,
                    Code = piece.Code,
                    Name = translation?.Name ?? string.Empty,
                    PhotoPath = piece.PhotoPath,
                    TextNarration = translation?.TextNarration,
                    Period = translation?.Period,
                    Category = translation?.Category
                };
            }).ToList();

            _logger.LogInformation("Returned {Count} pieces", response.Count);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPieceById(
            int id,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching piece with ID: {Id}", id);

            var piece = await _service.GetByIdAsync(id, cancellationToken);

            if (piece == null)
            {
                _logger.LogWarning("Piece not found with ID: {Id}", id);
                return NotFound(new { success = false, message = "Piece not found" });
            }

            var translation = piece.Translations.FirstOrDefault(x => x.LanguageCode == lang);

            var response = new PieceWithScannedStatusDto
            {
                Id = piece.Id,
                Code = piece.Code,
                Name = translation?.Name ?? piece.Name,
                PhotoPath = piece.PhotoPath,
                TextNarration = translation?.TextNarration ?? string.Empty,
                Period = translation?.Period ?? string.Empty,
                Category = translation?.Category ?? string.Empty,
                IsFavorite = false,
                ScannedArtifactId = null,
                ScannedAt = null
            };

            _logger.LogInformation("Piece returned with ID: {Id}", id);
            return Ok(new { success = true, data = response });
        }

        [HttpGet("GetByCode/{code}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCodeWithTranslationsAsync(
            string code,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching piece with Code: {Code}", code);

            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized access attempt to piece code {Code}", code);
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var piece = await _service.GetByCodeWithScannedStatusAsync(code, userId, cancellationToken);

            if (piece == null)
            {
                _logger.LogWarning("Piece not found with Code: {Code}", code);
                return NotFound(new { success = false, message = $"Piece with Code {code} not found." });
            }

            var translation = piece.Translations.FirstOrDefault(x => x.LanguageCode == lang);

            var response = new PieceWithScannedStatusDto
            {
                Id = piece.Id,
                Code = piece.Code,
                Name = translation?.Name ?? piece.Name,
                PhotoPath = piece.PhotoPath,
                TextNarration = translation?.TextNarration ?? string.Empty,
                Period = translation?.Period ?? string.Empty,
                Category = translation?.Category ?? string.Empty,
                IsFavorite = false,
                ScannedArtifactId = null,
                ScannedAt = null
            };

            _logger.LogInformation("Piece returned with Code: {Code}", code);
            return Ok(new { success = true, data = response });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePieces(CreatePiecesRequest piecesRequest)
        {
            if (piecesRequest is null)
            {
                _logger.LogWarning("Created piece failed: request is null");
                return BadRequest("Invalid piece data.");
            }

            if (string.IsNullOrEmpty(piecesRequest.Code))
            {
                _logger.LogWarning("Piece creation failed: Code is empty");
                return BadRequest("Piece code is required.");
            }

            if (string.IsNullOrEmpty(piecesRequest.PhotoPath))
            {
                _logger.LogWarning("Piece creation failed: PhotoPath is empty");
                return BadRequest("PhotoPath is required.");
            }

            if (piecesRequest.Translations == null || !piecesRequest.Translations.Any())
            {
                _logger.LogWarning("Piece creation failed: Text translation is empty");
                return BadRequest("At least one translation is required.");
            }

            var piece = new Pieces
            {
                Code = piecesRequest.Code,
                Name = piecesRequest.Translations.FirstOrDefault()?.Name,
                PhotoPath = piecesRequest.PhotoPath,
                Translations = piecesRequest.Translations.Select(t => new PieceTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    TextNarration = t.TextNarration,
                    Period = t.Period,
                    Category = t.Category
                }).ToList()
            };

            var createdPiece = await _service.CreateAsync(piece);

            _logger.LogInformation("Piece created with code: {Code}", createdPiece.Code);

            var translation = createdPiece.Translations.FirstOrDefault();

            var response = new PiecesResponse
            {
                Id = createdPiece.Id,
                Code = createdPiece.Code,
                Name = translation?.Name ?? createdPiece.Name,
                PhotoPath = createdPiece.PhotoPath,
                TextNarration = translation?.TextNarration,
                Period = translation?.Period,
                Category = translation?.Category
            };

            return Ok(response);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> UpdatePiece(string code, UpdatePiecesRequest piecesRequest)
        {
            _logger.LogInformation("Updating piece with Code: {Code}", code);

            var piece = await _service.GetByCodeWithTranslationsAsync(code);

            if (piece == null)
            {
                _logger.LogWarning("Piece not found for update with Code: {Code}", code);
                return NotFound();
            }

            if (!string.IsNullOrEmpty(piecesRequest.Code) && piecesRequest.Code != code)
            {
                var existing = await _service.GetByCodeAsync(piecesRequest.Code);

                if (existing != null)
                {
                    _logger.LogWarning("Update Piece failed: Code already exists {Code}", piecesRequest.Code);
                    return BadRequest("Piece with this code already exists.");
                }
            }

            _mapper.Map(piecesRequest, piece);

            foreach (var item in piecesRequest.Translations)
            {
                var existing = piece.Translations
                    .FirstOrDefault(x => x.LanguageCode == item.LanguageCode);

                if (existing != null)
                {
                    existing.Name = item.Name;
                    existing.TextNarration = item.TextNarration;
                }
                else
                {
                    piece.Translations.Add(new PieceTranslation
                    {
                        LanguageCode = item.LanguageCode,
                        Name = item.Name,
                        TextNarration = item.TextNarration
                    });
                }
            }

            var updateResult = await _service.UpdateAsync(piece);

            if (!updateResult)
            {
                _logger.LogError("Failed to update piece with Code: {Code}", code);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update piece.");
            }

            _logger.LogInformation("Piece updated with Code: {Code}", piece.Code);

            return Ok(_mapper.Map<PiecesResponse>(piece));
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeletePiece(string code)
        {
            _logger.LogInformation("Deleting piece with Code: {Code}", code);

            var piece = await _service.GetByCodeAsync(code);

            if (piece == null)
            {
                _logger.LogWarning("Piece not found for deletion with Code: {Code}", code);
                return NotFound();
            }

            var deleteResult = await _service.DeleteAsync(piece.Id);

            if (!deleteResult)
            {
                _logger.LogWarning("Failed to delete piece with Code: {Code}", code);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete piece.");
            }

            _logger.LogInformation("Piece deleted with Code: {Code}", code);

            return Ok($"Piece with Code {code} deleted successfully.");
        }
    }
}