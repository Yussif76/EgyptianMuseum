# Piece-ScannedArtifact Integration Feature

## Overview

This implementation adds a feature to link **Piece (Artifact)** with **ScannedArtifact**, allowing automatic tracking when users view artifacts. The system automatically creates or reuses `ScannedArtifact` records when a user requests a Piece.

## Architecture Changes

### 1. Domain Layer

#### Updated Entity: `ScannedArtifact`
The entity already had the required structure:
```csharp
public class ScannedArtifact
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int PieceId { get; set; }  // NEW RELATION
    public string LabelText { get; set; } = null!;
    public bool IsFavorite { get; set; }
    public DateTime ScannedAt { get; set; }

    public virtual Pieces Piece { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
```

#### Updated Entity: `Pieces`
Already has the navigation property:
```csharp
public ICollection<ScannedArtifact> ScannedArtifacts { get; set; } = new List<ScannedArtifact>();
```

### 2. Infrastructure Layer

#### AppDbContext Configuration
Added **unique constraint** on `(UserId, PieceId)`:
```csharp
entity.HasIndex(e => new { e.UserId, e.PieceId })
    .IsUnique()
    .HasDatabaseName("UK_ScannedArtifacts_UserId_PieceId");
```

#### Repository Enhancement: `ScannedArtifactRepository`
Added two new methods:

1. **`GetByUserIdAndPieceIdAsync`** - Retrieve existing scan record
   ```csharp
   Task<ScannedArtifact?> GetByUserIdAndPieceIdAsync(string userId, int pieceId, CancellationToken cancellationToken)
   ```

2. **`GetFavoritesByUserIdAsync`** - Get all favorite pieces for a user
   ```csharp
   Task<List<ScannedArtifact>> GetFavoritesByUserIdAsync(string userId, CancellationToken cancellationToken)
   ```

#### Migration
**File**: `20260502150532_AddUniqueConstraintScannedArtifacts.cs`
- Creates unique index on `(UserId, PieceId)`
- Ensures no duplicate scans for the same user + piece combination

### 3. Application Layer

#### Interface Updates

**`IPiecesServices`** - New method:
```csharp
Task<Pieces?> GetByIdWithScannedStatusAsync(int id, string userId, CancellationToken cancellationToken)
```

**`IScannedArtifactRepository`** - New methods:
```csharp
Task<ScannedArtifact?> GetByUserIdAndPieceIdAsync(string userId, int pieceId, CancellationToken cancellationToken)
Task<List<ScannedArtifact>> GetFavoritesByUserIdAsync(string userId, CancellationToken cancellationToken)
```

**`IScannedArtifactService`** - New methods:
```csharp
Task UpdateFavoriteByPieceIdAsync(string userId, int pieceId, bool isFavorite, CancellationToken cancellationToken)
Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken)
```

#### Service Implementation

**`PiecesService`** - Updated constructor to inject `IScannedArtifactRepository`:
```csharp
public PiecesService(
    IPiecesRepository<Pieces> repository,
    IScannedArtifactRepository scannedArtifactRepository) : IPiecesServices
```

**Core Logic** - `GetByIdWithScannedStatusAsync`:
```csharp
public async Task<Pieces?> GetByIdWithScannedStatusAsync(int id, string userId, CancellationToken cancellationToken)
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
```

**`ScannedArtifactService`** - New methods:
1. **`UpdateFavoriteByPieceIdAsync`** - Creates or updates favorite status
   - If record exists: updates `IsFavorite`
   - If not exists: creates new record with `IsFavorite` value

2. **`GetUserFavoritesAsync`** - Returns all favorite pieces
   - Filters by `UserId` and `IsFavorite = true`
   - Orders by most recent

### 4. API Layer

#### New DTO: `PieceWithScannedStatusDto`
Extends `PiecesResponse` with scan info:
```csharp
public class PieceWithScannedStatusDto
{
    // Piece data
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PhotoPath { get; set; } = null!;
    public string TextNarration { get; set; } = null!;
    public string Period { get; set; } = null!;
    public string Category { get; set; } = null!;
    
    // Scanned status
    public int? ScannedArtifactId { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime? ScannedAt { get; set; }
}
```

#### PiecesController Updates

**New Endpoint**: `GET /Pieces/{id}`
- **Requires**: JWT authentication
- **Extracts**: `UserId` from JWT token
- **Action**:
  1. Validates user authentication
  2. Calls `GetByIdWithScannedStatusAsync`
  3. Returns piece with scanned status
- **Response**:
  ```json
  {
    "success": true,
    "data": {
      "id": 1,
      "code": "PH001",
      "name": "Pharaoh Statue",
      "photoPath": "/images/pharaoh.jpg",
      "textNarration": "...",
      "period": "New Kingdom",
      "category": "Statues",
      "scannedArtifactId": 123,
      "isFavorite": false,
      "scannedAt": "2025-05-02T15:05:32.000Z"
    }
  }
  ```

#### ScannedArtifactsController Updates

**New Endpoint 1**: `PUT /api/scanned-artifacts/pieces/{pieceId}/favorite`
- **Requires**: JWT authentication
- **Body**:
  ```json
  {
    "isFavorite": true
  }
  ```
- **Behavior**:
  - If record exists: updates `IsFavorite`
  - If not exists: creates record with `IsFavorite` value
- **Response**: `200 OK`
  ```json
  {
    "success": true,
    "message": "Favorite status updated successfully"
  }
  ```

**New Endpoint 2**: `GET /api/scanned-artifacts/favorites`
- **Requires**: JWT authentication
- **Returns**: List of all favorite pieces for the current user
- **Response**: `200 OK`
  ```json
  {
    "success": true,
    "message": "Favorite artifacts retrieved successfully",
    "data": [
      {
        "id": 123,
        "pieceId": 1,
        "labelText": "PH001",
        "isFavorite": true,
        "scannedAt": "2025-05-02T15:05:32.000Z",
        "pieceName": "Pharaoh Statue",
        "pieceDescription": "...",
        "pieceImageUrl": "/images/pharaoh.jpg",
        "piecePeriod": "New Kingdom",
        "pieceCategory": "Statues"
      }
    ]
  }
  ```

### 5. Dependency Injection

Updated `Program.cs` to inject `IScannedArtifactRepository` into `PiecesService`:
```csharp
builder.Services.AddScoped<IPiecesServices, PiecesService>();
builder.Services.AddScoped<IScannedArtifactRepository, ScannedArtifactRepository>();
```

## Flow Diagrams

### When User Views a Piece

```
GET /Pieces/{id}
    ↓
[PiecesController.GetPieceById]
    ↓
Extract UserId from JWT
    ↓
[PiecesService.GetByIdWithScannedStatusAsync]
    ↓
Get Piece by ID
    ↓
Check if ScannedArtifact exists (UserId + PieceId)
    ├─ YES → Return existing record
    └─ NO → Create new ScannedArtifact
         ↓
    Return piece to controller
    ↓
[Response] Return PieceWithScannedStatusDto
```

### When User Toggles Favorite

```
PUT /api/scanned-artifacts/pieces/{pieceId}/favorite
    ↓
[ScannedArtifactsController.ToggleFavoriteByPieceId]
    ↓
Extract UserId from JWT
    ↓
[ScannedArtifactService.UpdateFavoriteByPieceIdAsync]
    ↓
Check if ScannedArtifact exists (UserId + PieceId)
    ├─ YES → Update IsFavorite
    └─ NO → Create new ScannedArtifact with IsFavorite
         ↓
    Return success
    ↓
[Response] 200 OK
```

### When User Gets Favorites

```
GET /api/scanned-artifacts/favorites
    ↓
[ScannedArtifactsController.GetUserFavorites]
    ↓
Extract UserId from JWT
    ↓
[ScannedArtifactService.GetUserFavoritesAsync]
    ↓
[ScannedArtifactRepository.GetFavoritesByUserIdAsync]
    ↓
Query: SELECT * FROM ScannedArtifacts 
       WHERE UserId = @userId AND IsFavorite = true
       ORDER BY ScannedAt DESC
    ↓
Map to DTOs
    ↓
[Response] 200 OK with list
```

## Database Schema

### ScannedArtifacts Table

| Column | Type | Constraints |
|--------|------|-------------|
| Id | INT | PRIMARY KEY |
| UserId | NVARCHAR(450) | FOREIGN KEY (ApplicationUser), NOT NULL |
| PieceId | INT | FOREIGN KEY (Artifactpieces), NOT NULL |
| LabelText | NVARCHAR(255) | NOT NULL |
| IsFavorite | BIT | NOT NULL, DEFAULT = 0 |
| ScannedAt | DATETIME2 | NOT NULL |
| **UNIQUE INDEX** | | **(UserId, PieceId)** |

## Error Handling

### Validation
- ✅ User authentication check (JWT)
- ✅ Piece existence validation
- ✅ User authorization (owns scanned record)
- ✅ Null reference checks

### Exception Handling
- `UnauthorizedAccessException` → 401 Unauthorized
- `KeyNotFoundException` → 404 Not Found
- `ArgumentException` → 400 Bad Request
- General `Exception` → Logged & handled gracefully

## Concurrency & Uniqueness

**Unique Constraint Benefits**:
1. Prevents duplicate records at database level
2. Handles race conditions automatically
3. Database enforces consistency

**Handling in Code**:
- Check before creation with `GetByUserIdAndPieceIdAsync`
- Gracefully handle if unique constraint violation occurs
- Always returns consistent state

## Testing Endpoints

### 1. View a Piece (Auto-creates ScannedArtifact)
```bash
GET /Pieces/1
Authorization: Bearer {your_jwt_token}
```

### 2. Toggle Favorite (Create or Update)
```bash
PUT /api/scanned-artifacts/pieces/1/favorite
Authorization: Bearer {your_jwt_token}
Content-Type: application/json

{
  "isFavorite": true
}
```

### 3. Get All Favorites
```bash
GET /api/scanned-artifacts/favorites
Authorization: Bearer {your_jwt_token}
```

### 4. Get All Scanned Artifacts
```bash
GET /api/scanned-artifacts
Authorization: Bearer {your_jwt_token}
```

## Performance Considerations

1. **Indexes**: Unique index on (UserId, PieceId) enables fast lookups
2. **Query Optimization**: Includes navigation properties when needed
3. **Async/Await**: All operations are fully asynchronous
4. **Database Constraints**: Delegate uniqueness enforcement to DB

## Security

1. **Authentication Required**: All endpoints require JWT
2. **Authorization**: Users can only access their own records
3. **SQL Injection**: Protected via EF Core parameterization
4. **Claim Validation**: Extract UserId from authenticated claims

## Migration

Run migrations to apply the unique constraint:
```bash
dotnet ef database update -p EgyptianMuseum.Infrastructure -s EgyptianMuseum.API
```

Migration file: `20260502150532_AddUniqueConstraintScannedArtifacts.cs`

## Summary of Changes

| Component | Type | Change |
|-----------|------|--------|
| AppDbContext | Config | Added unique constraint |
| ScannedArtifact | Entity | Already had PieceId |
| Pieces | Entity | Already had collection |
| IScannedArtifactRepository | Interface | +2 methods |
| ScannedArtifactRepository | Implementation | +2 methods |
| IPiecesServices | Interface | +1 method |
| PiecesService | Implementation | +1 method, updated constructor |
| IScannedArtifactService | Interface | +2 methods |
| ScannedArtifactService | Implementation | +2 methods |
| PiecesController | API | +1 endpoint (GET /{id}) |
| ScannedArtifactsController | API | +2 endpoints (PUT, GET /favorites) |
| DTOs | Domain | +1 new DTO |
| Migration | Infrastructure | +1 migration file |

## Bonus Feature Completed ✅

✅ **GET /api/scanned-artifacts/favorites** - Returns all favorite pieces for the current user

## Notes

- All code follows Clean Architecture principles
- DTOs are only used in the API layer
- Business logic is in services
- Repository pattern is strictly followed
- Async/await is used throughout
- Dependency injection is properly configured
- Error handling is comprehensive
