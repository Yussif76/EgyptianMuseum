# Modified Files - Smart Scan Implementation

## File 1: IScannedArtifactService.cs
**Path**: `EgyptianMuseum.Application\Interfaces\IScannedArtifactService.cs`

```csharp
using EgyptianMuseum.Application.DTOs.ScannedArtifacts;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IScannedArtifactService
    {
        Task<ScanArtifactResponseDto> ScanArtifactAsync(string userId, ScanArtifactRequestDto request, string lang = "en", CancellationToken cancellationToken = default);
        Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
        Task<ScannedArtifactDto> GetByIdAsync(string userId, int id, CancellationToken cancellationToken = default, string lang = "en");
        Task UpdateFavoriteAsync(string userId, int scannedArtifactId, bool isFavorite, CancellationToken cancellationToken = default);
        Task UpdateFavoriteByPieceIdAsync(string userId, int pieceId, bool isFavorite, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
    }
}
```

**Changes**:
- `GetUserScannedArtifactsAsync`: Added `string lang = "en"` parameter
- `GetByIdAsync`: Added `string lang = "en"` parameter
- `GetUserFavoritesAsync`: Added `string lang = "en"` parameter

---

## File 2: ScannedArtifactService.cs
**Path**: `EgyptianMuseum.Application\Services\ScannedArtifacts\ScannedArtifactService.cs`

```csharp
using EgyptianMuseum.Application.DTOs.ScannedArtifacts;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.ScannedArtifacts
{
    public class ScannedArtifactService : IScannedArtifactService
    {
        private readonly IScannedArtifactRepository _scannedArtifactRepository;
        private readonly IPiecesRepository<Pieces> _pieceRepository;

        public ScannedArtifactService(
            IScannedArtifactRepository scannedArtifactRepository,
            IPiecesRepository<Pieces> pieceRepository)
        {
            _scannedArtifactRepository = scannedArtifactRepository;
            _pieceRepository = pieceRepository;
        }

        public async Task<ScanArtifactResponseDto> ScanArtifactAsync(
            string userId,
            ScanArtifactRequestDto request,
            string lang = "en",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.LabelText))
                throw new ArgumentException("Label text cannot be empty");

            var labelText = request.LabelText.Trim();

            var piece = await _pieceRepository.GetByCodeWithTranslationsAsync(labelText, cancellationToken);
            if (piece == null)
                throw new KeyNotFoundException($"No artifact found with label '{labelText}'");

            // Check if user already has a scan for this piece
            var existingScannedArtifact = await _scannedArtifactRepository
                .GetByUserIdAndPieceIdAsync(userId, piece.Id, cancellationToken);

            if (existingScannedArtifact != null)
            {
                // Return existing scan with its favorite status preserved
                return MapToScanResponseDto(existingScannedArtifact, lang);
            }

            // Create new scan record
            var scannedArtifact = new ScannedArtifact
            {
                UserId = userId,
                PieceId = piece.Id,
                LabelText = labelText,
                IsFavorite = false,
                ScannedAt = DateTime.UtcNow
            };

            await _scannedArtifactRepository.AddAsync(scannedArtifact, cancellationToken);

            // Reload with piece data for response
            scannedArtifact.Piece = piece;
            return MapToScanResponseDto(scannedArtifact, lang);
        }

        public async Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(
            string userId,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var scannedArtifacts = await _scannedArtifactRepository.GetByUserIdAsync(userId, cancellationToken);

            return scannedArtifacts
                .OrderByDescending(s => s.ScannedAt)
                .Select(s => MapToDto(s, lang))
                .ToList();
        }

        public async Task<ScannedArtifactDto> GetByIdAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdWithPieceAsync(id, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            return MapToDto(scannedArtifact, lang);
        }

        public async Task UpdateFavoriteAsync(
            string userId,
            int scannedArtifactId,
            bool isFavorite,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdAsync(scannedArtifactId, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            scannedArtifact.IsFavorite = isFavorite;
            await _scannedArtifactRepository.UpdateAsync(scannedArtifact, cancellationToken);
        }

        public async Task UpdateFavoriteByPieceIdAsync(
            string userId,
            int pieceId,
            bool isFavorite,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository
                .GetByUserIdAndPieceIdAsync(userId, pieceId, cancellationToken);

            if (scannedArtifact == null)
            {
                var piece = await _pieceRepository.GetByIdAsync(pieceId, cancellationToken);
                if (piece == null)
                    throw new KeyNotFoundException("Piece not found");

                scannedArtifact = new ScannedArtifact
                {
                    UserId = userId,
                    PieceId = pieceId,
                    LabelText = piece.Code,
                    IsFavorite = isFavorite,
                    ScannedAt = DateTime.UtcNow
                };

                await _scannedArtifactRepository.AddAsync(scannedArtifact, cancellationToken);
            }
            else
            {
                scannedArtifact.IsFavorite = isFavorite;
                await _scannedArtifactRepository.UpdateAsync(scannedArtifact, cancellationToken);
            }
        }

        public async Task DeleteAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdAsync(id, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            await _scannedArtifactRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(
            string userId,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var favorites = await _scannedArtifactRepository.GetFavoritesByUserIdAsync(userId, cancellationToken);

            return favorites
                .Select(s => MapToDto(s, lang))
                .ToList();
        }

        private static ScannedArtifactDto MapToDto(ScannedArtifact scannedArtifact, string lang = "en")
        {
            var translation = SelectTranslation(scannedArtifact.Piece, lang);

            return new ScannedArtifactDto
            {
                Id = scannedArtifact.Id,
                PieceId = scannedArtifact.PieceId,
                LabelText = scannedArtifact.LabelText,
                IsFavorite = scannedArtifact.IsFavorite,
                ScannedAt = scannedArtifact.ScannedAt,
                PieceName = translation?.Name ?? scannedArtifact.Piece?.Name,
                PieceDescription = translation?.TextNarration,
                PieceImageUrl = scannedArtifact.Piece?.PhotoPath,
                PiecePeriod = translation?.Period,
                PieceCategory = translation?.Category
            };
        }

        private static ScanArtifactResponseDto MapToScanResponseDto(ScannedArtifact scannedArtifact, string lang = "en")
        {
            var translation = SelectTranslation(scannedArtifact.Piece, lang);

            return new ScanArtifactResponseDto
            {
                ScannedArtifactId = scannedArtifact.Id,
                PieceId = scannedArtifact.PieceId,
                LabelText = scannedArtifact.LabelText,
                IsFavorite = scannedArtifact.IsFavorite,
                ScannedAt = scannedArtifact.ScannedAt,
                PieceName = translation?.Name ?? scannedArtifact.Piece?.Name,
                PieceDescription = translation?.TextNarration,
                PieceImageUrl = scannedArtifact.Piece?.PhotoPath,
                PiecePeriod = translation?.Period,
                PieceCategory = translation?.Category
            };
        }

        private static PieceTranslation? SelectTranslation(Pieces? piece, string lang)
        {
            if (piece?.Translations == null || piece.Translations.Count == 0)
                return null;

            // Priority 1: Requested language
            var translation = piece.Translations.FirstOrDefault(t => t.LanguageCode == lang);
            if (translation != null)
                return translation;

            // Priority 2: English fallback
            translation = piece.Translations.FirstOrDefault(t => t.LanguageCode == "en");
            if (translation != null)
                return translation;

            // Priority 3: First available
            return piece.Translations.FirstOrDefault();
        }
    }
}
```

**Changes**:
- `GetUserScannedArtifactsAsync`: Added `string lang = "en"` parameter, passes to `MapToDto(s, lang)`
- `GetByIdAsync`: Added `string lang = "en"` parameter, passes to `MapToDto(scannedArtifact, lang)`
- `GetUserFavoritesAsync`: Added `string lang = "en"` parameter, passes to `MapToDto(s, lang)`
- Existing mapping methods already support translation selection

---

## File 3: ScannedArtifactsController.cs
**Path**: `EgyptianMuseum.API\Controllers\ScannedArtifactsController.cs`

```csharp
using EgyptianMuseum.Application.DTOs.ScannedArtifacts;
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
    public class ScannedArtifactsController : ControllerBase
    {
        private readonly IScannedArtifactService _scannedArtifactService;

        public ScannedArtifactsController(IScannedArtifactService scannedArtifactService)
        {
            _scannedArtifactService = scannedArtifactService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }

        [HttpPost("scan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ScanArtifact(
            [FromBody] ScanArtifactRequestDto request,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.LabelText))
                    return BadRequest(new { success = false, message = "Label text cannot be empty" });

                var userId = GetUserId();
                var result = await _scannedArtifactService.ScanArtifactAsync(userId, request, lang, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new { success = true, data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserScannedArtifacts(
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUserId();
                var result = await _scannedArtifactService.GetUserScannedArtifactsAsync(userId, cancellationToken, lang);
                return Ok(new { success = true, message = "Scanned artifacts retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            int id,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                var result = await _scannedArtifactService.GetByIdAsync(userId, id, cancellationToken, lang);
                return Ok(new { success = true, message = "Scanned artifact retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpPut("{id}/favorite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFavorite(
            int id,
            [FromBody] UpdateScannedArtifactFavoriteRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                await _scannedArtifactService.UpdateFavoriteAsync(userId, id, request.IsFavorite, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                await _scannedArtifactService.DeleteAsync(userId, id, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpPut("pieces/{pieceId}/favorite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleFavoriteByPieceId(
            int pieceId,
            [FromBody] UpdateScannedArtifactFavoriteRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (pieceId <= 0)
                    return BadRequest(new { success = false, message = "Invalid piece ID" });

                var userId = GetUserId();
                await _scannedArtifactService.UpdateFavoriteByPieceIdAsync(userId, pieceId, request.IsFavorite, cancellationToken);
                return Ok(new { success = true, message = "Favorite status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("favorites")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserFavorites(
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUserId();
                var result = await _scannedArtifactService.GetUserFavoritesAsync(userId, cancellationToken, lang);
                return Ok(new { success = true, message = "Favorite artifacts retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }
    }
}
```

**Changes**:
- `GetUserScannedArtifacts()`: Added `[FromQuery] string lang = "en"` parameter, passed to service
- `GetById()`: Added `[FromQuery] string lang = "en"` parameter, passed to service
- `GetUserFavorites()`: Added `[FromQuery] string lang = "en"` parameter, passed to service
- All endpoints now support multilingual responses

---

## Summary of Changes

### Modified Files: 3
1. **IScannedArtifactService.cs** - Interface updated with language parameter
2. **ScannedArtifactService.cs** - Service implementation updated with language parameter handling
3. **ScannedArtifactsController.cs** - Controller endpoints updated with language query parameter

### No Changes Required:
- **ScannedArtifactRepository.cs** - Already includes Piece and Translations correctly
- **IScannedArtifactRepository.cs** - Already has required methods
- **DTOs** - All DTOs already correct (ScanArtifactRequestDto, ScanArtifactResponseDto, ScannedArtifactDto)
- **Entities** - All entities correctly structured

### Key Features Implemented:
✅ Smart scan with duplicate prevention  
✅ Multilanguage support with fallback logic  
✅ No Piece data duplication in ScannedArtifacts  
✅ Proper DTOs for all responses  
✅ Clean Architecture compliance  
✅ Authorization checks on all endpoints  
✅ Comprehensive error handling  
✅ Async/await throughout

### Build Status:
✅ **BUILD SUCCESSFUL** - All changes compile without errors
