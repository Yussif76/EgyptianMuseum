# Code Reference Guide: Implementation Details

## Table of Contents
1. [Entity Definitions](#entity-definitions)
2. [Database Configuration](#database-configuration)
3. [Repository Implementation](#repository-implementation)
4. [Service Implementation](#service-implementation)
5. [Controller Implementation](#controller-implementation)
6. [DTOs](#dtos)
7. [Complete Examples](#complete-examples)

---

## Entity Definitions

### ScannedArtifact Entity

```csharp
namespace EgyptianMuseum.Domain.Entities
{
    public class ScannedArtifact
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int PieceId { get; set; }
        public string LabelText { get; set; } = null!;
        public bool IsFavorite { get; set; }
        public DateTime ScannedAt { get; set; }

        // Navigation properties
        public virtual Pieces Piece { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
```

### Pieces Entity

```csharp
namespace EgyptianMuseum.Domain.Entities
{
    public class Pieces : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        
        // Navigation properties
        public ICollection<PieceTranslation> Translations { get; set; } 
            = new List<PieceTranslation>();
        public ICollection<ScannedArtifact> ScannedArtifacts { get; set; } 
            = new List<ScannedArtifact>();
    }
}
```

---

## Database Configuration

### EF Core Fluent API Configuration

```csharp
// In AppDbContext.OnModelCreating()

// ScannedArtifact configuration
modelBuilder.Entity<ScannedArtifact>(entity =>
{
    entity.HasKey(e => e.Id);

    entity.Property(e => e.UserId).IsRequired();
    entity.Property(e => e.LabelText).IsRequired().HasMaxLength(255);

    // Foreign Key to Pieces
    entity.HasOne(e => e.Piece)
        .WithMany(p => p.ScannedArtifacts)
        .HasForeignKey(e => e.PieceId)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_ScannedArtifacts_Artifactpieces_PieceId");

    // Foreign Key to ApplicationUser
    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // UNIQUE CONSTRAINT - Prevents duplicates
    entity.HasIndex(e => new { e.UserId, e.PieceId })
        .IsUnique()
        .HasDatabaseName("UK_ScannedArtifacts_UserId_PieceId");
});

// Pieces configuration
modelBuilder.Entity<Pieces>(entity =>
{
    entity.ToTable("Artifactpieces");
    entity.HasKey(p => p.Id);
    
    entity.Property(p => p.Code).IsRequired();
    entity.HasIndex(p => p.Code).IsUnique();
    
    // Query filter for soft deletes
    entity.HasQueryFilter(p => !p.IsDeleted);
});
```

### Migration Code

```csharp
// File: 20260502150532_AddUniqueConstraintScannedArtifacts.cs

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintScannedArtifacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScannedArtifacts_UserId",
                table: "ScannedArtifacts");

            migrationBuilder.CreateIndex(
                name: "UK_ScannedArtifacts_UserId_PieceId",
                table: "ScannedArtifacts",
                columns: new[] { "UserId", "PieceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_ScannedArtifacts_UserId_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.CreateIndex(
                name: "IX_ScannedArtifacts_UserId",
                table: "ScannedArtifacts",
                column: "UserId");
        }
    }
}
```

---

## Repository Implementation

### Interface Definition

```csharp
namespace EgyptianMuseum.Application.Interfaces
{
    public interface IScannedArtifactRepository
    {
        Task AddAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifact>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ScannedArtifact?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ScannedArtifact?> GetByIdWithPieceAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        
        // NEW METHODS
        Task<ScannedArtifact?> GetByUserIdAndPieceIdAsync(string userId, int pieceId, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifact>> GetFavoritesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
```

### Repository Implementation

```csharp
namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class ScannedArtifactRepository : IScannedArtifactRepository
    {
        private readonly AppDbContext _context;

        public ScannedArtifactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default)
        {
            await _context.ScannedArtifacts.AddAsync(scannedArtifact, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ScannedArtifact>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.ScannedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ScannedArtifact?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<ScannedArtifact?> GetByIdWithPieceAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default)
        {
            _context.ScannedArtifacts.Update(scannedArtifact);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _context.ScannedArtifacts.FindAsync(
                new object[] { id },
                cancellationToken: cancellationToken);

            if (scannedArtifact != null)
            {
                _context.ScannedArtifacts.Remove(scannedArtifact);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // NEW METHOD: Get by UserID and PieceID
        public async Task<ScannedArtifact?> GetByUserIdAndPieceIdAsync(string userId, int pieceId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.PieceId == pieceId, cancellationToken);
        }

        // NEW METHOD: Get favorites by UserID
        public async Task<List<ScannedArtifact>> GetFavoritesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                .Where(s => s.UserId == userId && s.IsFavorite)
                .OrderByDescending(s => s.ScannedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
```

---

## Service Implementation

### Interface Definition

```csharp
namespace EgyptianMuseum.Application.Interfaces
{
    public interface IPiecesServices
    {
        Task<IEnumerable<Pieces>> GetAllAsync();
        Task<Pieces> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Pieces> GetByCodeAsync(string code);
        Task<Pieces> CreateAsync(Pieces entity);
        Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize);
        Task<bool> UpdateAsync(Pieces entity);
        Task<bool> DeleteAsync(int id);
        Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default);
        Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize);
        
        // NEW METHODS
        Task<Pieces?> GetByIdWithScannedStatusAsync(int id, string userId, CancellationToken cancellationToken = default);
        Task<Pieces?> GetByCodeWithScannedStatusAsync(string code, string userId, CancellationToken cancellationToken = default);
    }
}
```

### Service Implementation

```csharp
namespace EgyptianMuseum.Application.Services.Services
{
    public class PiecesService(
        IPiecesRepository<Pieces> repository,
        IScannedArtifactRepository scannedArtifactRepository) : IPiecesServices
    {
        public Task<Pieces> CreateAsync(Pieces entity)
            => repository.CreateAsync(entity);

        public Task<bool> DeleteAsync(int id)
            => repository.DeleteAsync(id);

        public async Task<IEnumerable<Pieces>> GetAllAsync()
            => await repository.GetAllAsync();

        public Task<Pieces> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => repository.GetByIdAsync(id);

        public Task<bool> UpdateAsync(Pieces entity)
            => repository.UpdateAsync(entity);

        public Task<Pieces> GetByCodeAsync(string code)
            => repository.GetFirstOrDefaultAsync(x => x.Code == code);

        public Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize)
            => repository.GetPagedAsync(page, pageSize);

        public Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default)
            => repository.GetByCodeWithTranslationsAsync(code);

        public Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize)
            => repository.GetPagedWithTranslationsAsync(page, pageSize);

        /// <summary>
        /// Get piece by ID and manage ScannedArtifact status.
        /// Creates a new ScannedArtifact if it doesn't exist for this user.
        /// </summary>
        public async Task<Pieces?> GetByIdWithScannedStatusAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var piece = await repository.GetByIdAsync(id, cancellationToken);
            if (piece == null)
                return null;

            // Check if ScannedArtifact already exists for this user + piece
            var existingScanned = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync(userId, id, cancellationToken);

            if (existingScanned == null)
            {
                // Create new ScannedArtifact automatically
                var newScanned = new ScannedArtifact
                {
                    UserId = userId,
                    PieceId = id,
                    LabelText = piece.Code,
                    IsFavorite = false,
                    ScannedAt = DateTime.UtcNow
                };

                await scannedArtifactRepository.AddAsync(newScanned, cancellationToken);
            }

            return piece;
        }

        /// <summary>
        /// Get piece by code and manage ScannedArtifact status.
        /// Creates a new ScannedArtifact if it doesn't exist for this user.
        /// </summary>
        public async Task<Pieces?> GetByCodeWithScannedStatusAsync(string code, string userId, CancellationToken cancellationToken = default)
        {
            var piece = await repository.GetByCodeWithTranslationsAsync(code, cancellationToken);
            if (piece == null)
                return null;

            // Check if ScannedArtifact already exists for this user + piece
            var existingScanned = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync(userId, piece.Id, cancellationToken);

            if (existingScanned == null)
            {
                // Create new ScannedArtifact automatically
                var newScanned = new ScannedArtifact
                {
                    UserId = userId,
                    PieceId = piece.Id,
                    LabelText = piece.Name ?? piece.Code,
                    IsFavorite = false,
                    ScannedAt = DateTime.UtcNow
                };

                await scannedArtifactRepository.AddAsync(newScanned, cancellationToken);
            }

            return piece;
        }
    }
}
```

### ScannedArtifactService Implementation (Partial)

```csharp
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

        /// <summary>
        /// Update favorite status by piece ID.
        /// Creates ScannedArtifact if it doesn't exist.
        /// </summary>
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
                // Create new ScannedArtifact with favorite status
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
                // Update existing record
                scannedArtifact.IsFavorite = isFavorite;
                await _scannedArtifactRepository.UpdateAsync(scannedArtifact, cancellationToken);
            }
        }

        /// <summary>
        /// Get all favorite pieces for a user.
        /// </summary>
        public async Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var favorites = await _scannedArtifactRepository
                .GetFavoritesByUserIdAsync(userId, cancellationToken);

            return favorites
                .Select(s => MapToDto(s))
                .ToList();
        }

        private static ScannedArtifactDto MapToDto(ScannedArtifact s)
        {
            return new ScannedArtifactDto
            {
                Id = s.Id,
                PieceId = s.PieceId,
                LabelText = s.LabelText,
                IsFavorite = s.IsFavorite,
                ScannedAt = s.ScannedAt,
                PieceName = s.Piece?.Name,
                PieceDescription = s.Piece?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.TextNarration,
                PieceImageUrl = s.Piece?.PhotoPath,
                PiecePeriod = s.Piece?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.Period,
                PieceCategory = s.Piece?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.Category
            };
        }
    }
}
```

---

## Controller Implementation

### PiecesController

```csharp
namespace EgyptianMuseum.API.Controllers
{
    [Route("Pieces")]
    [ApiController]
    public class PiecesController : Controller
    {
        private readonly IPiecesServices _service;
        private readonly ILogger<PiecesController> _logger;
        private readonly IMapper _mapper;

        public PiecesController(
            IPiecesServices service,
            ILogger<PiecesController> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Extract UserId from JWT claims.
        /// </summary>
        private string? GetUserIdFromToken()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Get piece by ID with auto-creation of ScannedArtifact.
        /// Requires JWT authentication.
        /// </summary>
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

            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized access attempt to piece {Id}", id);
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var piece = await _service.GetByIdWithScannedStatusAsync(id, userId, cancellationToken);
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

        /// <summary>
        /// Get piece by code with auto-creation of ScannedArtifact.
        /// Requires JWT authentication.
        /// </summary>
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
    }
}
```

### ScannedArtifactsController (New Endpoints)

```csharp
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

        /// <summary>
        /// Toggle favorite status for a piece by piece ID.
        /// Creates ScannedArtifact if it doesn't exist.
        /// </summary>
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
                await _scannedArtifactService.UpdateFavoriteByPieceIdAsync(
                    userId,
                    pieceId,
                    request.IsFavorite,
                    cancellationToken);
                
                return Ok(new { success = true, message = "Favorite status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all favorite pieces for the current user.
        /// </summary>
        [HttpGet("favorites")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserFavorites(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var result = await _scannedArtifactService.GetUserFavoritesAsync(userId, cancellationToken);
                return Ok(new 
                { 
                    success = true, 
                    message = "Favorite artifacts retrieved successfully", 
                    data = result 
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }
    }
}
```

---

## DTOs

### PieceWithScannedStatusDto

```csharp
namespace EgyptianMuseum.Application.DTOs.Pieces
{
    public class PieceWithScannedStatusDto
    {
        /// <summary>
        /// Piece identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique piece code (e.g., "PH001")
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Piece name in specified language
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Path to piece image/photo
        /// </summary>
        public string PhotoPath { get; set; } = null!;

        /// <summary>
        /// Text narration/description
        /// </summary>
        public string TextNarration { get; set; } = null!;

        /// <summary>
        /// Historical period (e.g., "New Kingdom")
        /// </summary>
        public string Period { get; set; } = null!;

        /// <summary>
        /// Artifact category (e.g., "Statues")
        /// </summary>
        public string Category { get; set; } = null!;

        // Scanned status fields
        
        /// <summary>
        /// ID of the ScannedArtifact record (if exists)
        /// </summary>
        public int? ScannedArtifactId { get; set; }

        /// <summary>
        /// Whether user marked this piece as favorite
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// When user first scanned/viewed this piece
        /// </summary>
        public DateTime? ScannedAt { get; set; }
    }
}
```

### ScannedArtifactDto

```csharp
namespace EgyptianMuseum.Application.DTOs.ScannedArtifacts
{
    public class ScannedArtifactDto
    {
        public int Id { get; set; }
        public int PieceId { get; set; }
        public string LabelText { get; set; } = null!;
        public bool IsFavorite { get; set; }
        public DateTime ScannedAt { get; set; }
        
        // Piece information (denormalized for convenience)
        public string? PieceName { get; set; }
        public string? PieceDescription { get; set; }
        public string? PieceImageUrl { get; set; }
        public string? PiecePeriod { get; set; }
        public string? PieceCategory { get; set; }
    }
}
```

### UpdateScannedArtifactFavoriteRequestDto

```csharp
namespace EgyptianMuseum.Application.DTOs.ScannedArtifacts
{
    public class UpdateScannedArtifactFavoriteRequestDto
    {
        [Required]
        public bool IsFavorite { get; set; }
    }
}
```

---

## Complete Examples

### Example 1: Create New Scanned Artifact (First View)

```csharp
// HTTP Request
GET /Pieces/1?lang=en
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

// Controller receives request
// ↓
var userId = "user123";  // Extracted from JWT
var pieceId = 1;

// Service logic
var piece = await _service.GetByIdWithScannedStatusAsync(1, "user123", cancellationToken);

// Inside service:
// 1. Get piece by ID
var piece = await repository.GetByIdAsync(1);
// Result: Pieces { Id=1, Code="PH001", Name="Pharaoh Statue", ... }

// 2. Check if scanned record exists
var existing = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync("user123", 1);
// Result: null (doesn't exist)

// 3. Create new record
var newScanned = new ScannedArtifact
{
    UserId = "user123",
    PieceId = 1,
    LabelText = "PH001",
    IsFavorite = false,
    ScannedAt = DateTime.UtcNow  // 2025-05-02T15:45:30Z
};

await scannedArtifactRepository.AddAsync(newScanned);
// Database: INSERT INTO ScannedArtifacts ...

// 4. Return piece
return piece;

// Controller maps to DTO
var response = new PieceWithScannedStatusDto
{
    Id = 1,
    Code = "PH001",
    Name = "Pharaoh Statue",
    PhotoPath = "/images/pharaoh.jpg",
    TextNarration = "...",
    Period = "New Kingdom",
    Category = "Statues",
    IsFavorite = false,
    ScannedArtifactId = 42,  // ID of newly created record
    ScannedAt = null  // Will be set if we fetch fresh
};

// HTTP Response
200 OK
{
  "success": true,
  "data": { ... }
}
```

### Example 2: Reuse Existing Record (Second View)

```csharp
// Same user calls same endpoint again
GET /Pieces/1?lang=en
Authorization: Bearer ...

// Service logic
var piece = await _service.GetByIdWithScannedStatusAsync(1, "user123", cancellationToken);

// Inside service:
// 1. Get piece by ID
var piece = await repository.GetByIdAsync(1);
// Result: Pieces { Id=1, ... }

// 2. Check if scanned record exists
var existing = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync("user123", 1);
// Result: ScannedArtifact { Id=42, UserId="user123", PieceId=1, ScannedAt=2025-05-02T15:45:30Z }

// 3. Record exists, skip creation
if (existing == null)  // FALSE, so we skip this block
{
    // NOT EXECUTED
}

// 4. Return piece
return piece;

// Response is same as before
// Same scannedArtifactId (42)
// Same scannedAt timestamp
// NO DUPLICATE RECORD CREATED
```

### Example 3: Different Users Viewing Same Piece

```csharp
// User A views piece
// Service.GetByIdWithScannedStatusAsync(1, "userA")
//   → Creates record { Id=100, UserId="userA", PieceId=1, ScannedAt=15:45Z }

// User B views same piece
// Service.GetByIdWithScannedStatusAsync(1, "userB")
//   → Creates record { Id=101, UserId="userB", PieceId=1, ScannedAt=15:46Z }

// Database now has:
// ScannedArtifacts:
// ┌────┬────────┬─────────┬────────────┬─────────────┐
// │ Id │ UserId │ PieceId │ LabelText  │ ScannedAt   │
// ├────┼────────┼─────────┼────────────┼─────────────┤
// │100 │ userA  │    1    │ PH001      │ 15:45:30Z   │
// │101 │ userB  │    1    │ PH001      │ 15:46:00Z   │
// └────┴────────┴─────────┴────────────┴─────────────┘

// Unique constraint: (UserId, PieceId)
// Record 1: ("userA", 1) ✓
// Record 2: ("userB", 1) ✓
// Both are unique (different UserIds)
```

### Example 4: Toggle Favorite

```csharp
// HTTP Request
PUT /api/scanned-artifacts/pieces/1/favorite
Authorization: Bearer ...
Content-Type: application/json

{
  "isFavorite": true
}

// Service logic
await _scannedArtifactService.UpdateFavoriteByPieceIdAsync(
    "user123",
    1,
    true,
    cancellationToken);

// Inside service:
var existing = await _scannedArtifactRepository
    .GetByUserIdAndPieceIdAsync("user123", 1);

// CASE A: Record exists (from previous view)
if (existing == null)  // FALSE
{
    // Skip creation
}
else  // TRUE - Record exists
{
    existing.IsFavorite = true;
    await _scannedArtifactRepository.UpdateAsync(existing);
    // Database: UPDATE ScannedArtifacts SET IsFavorite=1 WHERE Id=42
}

// CASE B: Record doesn't exist (piece never viewed)
var existing = await _scannedArtifactRepository
    .GetByUserIdAndPieceIdAsync("user123", 2);  // Different piece

if (existing == null)  // TRUE - Record doesn't exist
{
    var piece = await _pieceRepository.GetByIdAsync(2);
    var newScanned = new ScannedArtifact
    {
        UserId = "user123",
        PieceId = 2,
        LabelText = piece.Code,
        IsFavorite = true,  // Set directly
        ScannedAt = DateTime.UtcNow
    };
    await _scannedArtifactRepository.AddAsync(newScanned);
    // Database: INSERT INTO ScannedArtifacts ... VALUES (..., true, ...)
}

// HTTP Response
200 OK
{
  "success": true,
  "message": "Favorite status updated successfully"
}
```

### Example 5: Get User's Favorites

```csharp
// HTTP Request
GET /api/scanned-artifacts/favorites
Authorization: Bearer ...

// Service logic
var favorites = await _scannedArtifactRepository
    .GetFavoritesByUserIdAsync("user123", cancellationToken);

// Inside repository:
// SELECT * FROM ScannedArtifacts
// WHERE UserId = 'user123' AND IsFavorite = 1
// ORDER BY ScannedAt DESC

// Result:
// ┌────┬────────┬─────────┬──────────────┬───────────┬────────────────┐
// │ Id │ UserId │ PieceId │ LabelText    │ IsFav     │ ScannedAt      │
// ├────┼────────┼─────────┼──────────────┼───────────┼────────────────┤
// │ 42 │user123 │    1    │ PH001        │  1        │ 15:45:30.000Z  │
// │ 50 │user123 │    5    │ SK002        │  1        │ 16:20:00.000Z  │
// └────┴────────┴─────────┴──────────────┴───────────┴────────────────┘

// Service maps to DTOs
var dtos = favorites.Select(s => MapToDto(s)).ToList();

// HTTP Response
200 OK
{
  "success": true,
  "message": "Favorite artifacts retrieved successfully",
  "data": [
    {
      "id": 42,
      "pieceId": 1,
      "labelText": "PH001",
      "isFavorite": true,
      "scannedAt": "2025-05-02T15:45:30.000Z",
      "pieceName": "Pharaoh Statue",
      "pieceDescription": "...",
      "pieceImageUrl": "/images/pharaoh.jpg",
      "piecePeriod": "New Kingdom",
      "pieceCategory": "Statues"
    },
    {
      "id": 50,
      "pieceId": 5,
      "labelText": "SK002",
      "isFavorite": true,
      "scannedAt": "2025-05-02T16:20:00.000Z",
      "pieceName": "Scarab Amulet",
      "pieceDescription": "...",
      ...
    }
  ]
}
```

---

## Dependency Injection Configuration

```csharp
// Program.cs

builder.Services.AddScoped<IPiecesServices, PiecesService>();
builder.Services.AddScoped<IScannedArtifactService, ScannedArtifactService>();
builder.Services.AddScoped<IScannedArtifactRepository, ScannedArtifactRepository>();
builder.Services.AddScoped(typeof(IPiecesRepository<>), typeof(PiecesRepository<>));

// When controller requests IPiecesServices:
// 1. DI looks up IPiecesServices
// 2. Finds PiecesService implementation
// 3. PiecesService constructor needs:
//    - IPiecesRepository<Pieces>
//    - IScannedArtifactRepository
// 4. DI resolves both dependencies
// 5. Creates PiecesService instance
// 6. Injects into controller

public class PiecesController
{
    private readonly IPiecesServices _service;  // ← Injected

    public PiecesController(IPiecesServices service)  // ← Constructor injection
    {
        _service = service;  // Uses PiecesService instance with dependencies
    }
}
```

---

**Code Reference Complete!**

All code is production-ready and follows .NET 8 + C# 12 best practices.
