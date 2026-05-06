# Detailed Code Changes - Line by Line

## File 1: IScannedArtifactService.cs

**Location**: `EgyptianMuseum.Application\Interfaces\IScannedArtifactService.cs`

### Change 1: GetUserScannedArtifactsAsync - Added lang parameter

**Before**:
```csharp
Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(string userId, CancellationToken cancellationToken = default);
```

**After**:
```csharp
Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
```

**Reason**: Allow clients to request translations in their preferred language

---

### Change 2: GetByIdAsync - Added lang parameter

**Before**:
```csharp
Task<ScannedArtifactDto> GetByIdAsync(string userId, int id, CancellationToken cancellationToken = default);
```

**After**:
```csharp
Task<ScannedArtifactDto> GetByIdAsync(string userId, int id, CancellationToken cancellationToken = default, string lang = "en");
```

**Reason**: Support multilingual response for single artifact retrieval

---

### Change 3: GetUserFavoritesAsync - Added lang parameter

**Before**:
```csharp
Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default);
```

**After**:
```csharp
Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
```

**Reason**: Support multilingual response for favorites list

---

## File 2: ScannedArtifactService.cs

**Location**: `EgyptianMuseum.Application\Services\ScannedArtifacts\ScannedArtifactService.cs`

### Change 1: GetUserScannedArtifactsAsync - Implementation update

**Before**:
```csharp
public async Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(
    string userId,
    CancellationToken cancellationToken = default)
{
    var scannedArtifacts = await _scannedArtifactRepository.GetByUserIdAsync(userId, cancellationToken);

    return scannedArtifacts
        .OrderByDescending(s => s.ScannedAt)
        .Select(s => MapToDto(s))
        .ToList();
}
```

**After**:
```csharp
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
```

**Changes**:
1. Added `string lang = "en"` parameter
2. Pass `lang` to `MapToDto(s, lang)` instead of `MapToDto(s)`

**Reason**: Translate piece details based on requested language

---

### Change 2: GetByIdAsync - Implementation update

**Before**:
```csharp
public async Task<ScannedArtifactDto> GetByIdAsync(
    string userId,
    int id,
    CancellationToken cancellationToken = default)
{
    var scannedArtifact = await _scannedArtifactRepository.GetByIdWithPieceAsync(id, cancellationToken);
    if (scannedArtifact == null)
        throw new KeyNotFoundException("Scanned artifact not found");

    if (scannedArtifact.UserId != userId)
        throw new UnauthorizedAccessException("You do not have access to this record");

    return MapToDto(scannedArtifact);
}
```

**After**:
```csharp
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
```

**Changes**:
1. Added `string lang = "en"` parameter
2. Pass `lang` to `MapToDto(scannedArtifact, lang)`

**Reason**: Support multilingual response for single artifact

---

### Change 3: GetUserFavoritesAsync - Implementation update

**Before**:
```csharp
public async Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(
    string userId,
    CancellationToken cancellationToken = default)
{
    var favorites = await _scannedArtifactRepository.GetFavoritesByUserIdAsync(userId, cancellationToken);

    return favorites
        .Select(s => MapToDto(s))
        .ToList();
}
```

**After**:
```csharp
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
```

**Changes**:
1. Added `string lang = "en"` parameter
2. Pass `lang` to `MapToDto(s, lang)`

**Reason**: Translate piece details in favorites based on requested language

---

## File 3: ScannedArtifactsController.cs

**Location**: `EgyptianMuseum.API\Controllers\ScannedArtifactsController.cs`

### Change 1: GetUserScannedArtifacts - Added lang query parameter

**Before**:
```csharp
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> GetUserScannedArtifacts(CancellationToken cancellationToken)
{
    try
    {
        var userId = GetUserId();
        var result = await _scannedArtifactService.GetUserScannedArtifactsAsync(userId, cancellationToken);
        return Ok(new { success = true, message = "Scanned artifacts retrieved successfully", data = result });
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized(new { success = false, message = "User not authenticated" });
    }
}
```

**After**:
```csharp
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
```

**Changes**:
1. Added `[FromQuery] string lang = "en"` parameter
2. Added `CancellationToken cancellationToken = default` for consistency
3. Pass `lang` as third argument to service method
4. Updated call from `GetUserScannedArtifactsAsync(userId, cancellationToken)` to `GetUserScannedArtifactsAsync(userId, cancellationToken, lang)`

**Usage**:
```
GET /api/scannedartifacts?lang=en
GET /api/scannedartifacts?lang=ar
```

---

### Change 2: GetById - Added lang query parameter

**Before**:
```csharp
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
{
    try
    {
        if (id <= 0)
            return BadRequest(new { success = false, message = "Invalid ID" });

        var userId = GetUserId();
        var result = await _scannedArtifactService.GetByIdAsync(userId, id, cancellationToken);
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
```

**After**:
```csharp
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
```

**Changes**:
1. Added `[FromQuery] string lang = "en"` parameter
2. Added `CancellationToken cancellationToken = default`
3. Pass `lang` as fourth argument to service method
4. Updated call from `GetByIdAsync(userId, id, cancellationToken)` to `GetByIdAsync(userId, id, cancellationToken, lang)`

**Usage**:
```
GET /api/scannedartifacts/1?lang=en
GET /api/scannedartifacts/1?lang=ar
```

---

### Change 3: GetUserFavorites - Added lang query parameter

**Before**:
```csharp
[HttpGet("favorites")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> GetUserFavorites(CancellationToken cancellationToken)
{
    try
    {
        var userId = GetUserId();
        var result = await _scannedArtifactService.GetUserFavoritesAsync(userId, cancellationToken);
        return Ok(new { success = true, message = "Favorite artifacts retrieved successfully", data = result });
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized(new { success = false, message = "User not authenticated" });
    }
}
```

**After**:
```csharp
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
```

**Changes**:
1. Added `[FromQuery] string lang = "en"` parameter
2. Added `CancellationToken cancellationToken = default`
3. Pass `lang` as third argument to service method
4. Updated call from `GetUserFavoritesAsync(userId, cancellationToken)` to `GetUserFavoritesAsync(userId, cancellationToken, lang)`

**Usage**:
```
GET /api/scannedartifacts/favorites?lang=en
GET /api/scannedartifacts/favorites?lang=ar
```

---

## Summary of Changes

### Total Changes: 9
- **Interface changes**: 3 (added lang parameter to method signatures)
- **Service implementation changes**: 3 (added lang parameter and pass to MapToDto)
- **Controller changes**: 3 (added lang query parameter and pass to service)

### Lines of Code Modified: ~30
### Files Modified: 3
### No Files Deleted: 0
### New Files Created: 0

### Database Impact: 
- **NONE** - No schema changes required

### Backward Compatibility:
- ✅ **FULL** - All lang parameters are optional with default value "en"

### Breaking Changes:
- ✅ **NONE** - Existing API consumers work without modification

---

## Testing Points for Each Change

### IScannedArtifactService Changes
- [ ] Interface compiles without errors
- [ ] Implementation matches interface signature
- [ ] All three methods accept lang parameter
- [ ] Default value is "en"

### ScannedArtifactService Changes
- [ ] GetUserScannedArtifactsAsync passes lang to MapToDto
- [ ] GetByIdAsync passes lang to MapToDto
- [ ] GetUserFavoritesAsync passes lang to MapToDto
- [ ] Language-based translation selection works correctly

### ScannedArtifactsController Changes
- [ ] GetUserScannedArtifacts endpoint accepts ?lang=en query parameter
- [ ] GetById endpoint accepts ?lang=ar query parameter
- [ ] GetUserFavorites endpoint accepts ?lang=en query parameter
- [ ] Default lang value is "en" if not provided
- [ ] Language parameter properly passed to service

---

## Performance Impact

- **Zero** additional database queries (includes already existed)
- **Minimal** memory overhead (string parameter)
- **Negligible** CPU impact (LINQ collection filtering)

---

## Security Impact

- ✅ No new security vulnerabilities introduced
- ✅ Authorization checks remain in place
- ✅ User scope enforcement maintained
- ✅ Language parameter cannot bypass security

---

## Verification Commands

```bash
# Build solution
dotnet build

# Run unit tests
dotnet test

# Build release version
dotnet build -c Release

# Clean before rebuild
dotnet clean && dotnet build
```

---

## Rollback Plan (if needed)

If changes need to be reverted, simply:
1. Remove `string lang = "en"` parameters from all methods
2. Update all `MapToDto(s, lang)` calls to `MapToDto(s)`
3. Remove `[FromQuery] string lang = "en"` from controller methods

---

## Complete Change Summary

| File | Type | Changes | Status |
|------|------|---------|--------|
| IScannedArtifactService.cs | Interface | Added lang param to 3 methods | ✅ Complete |
| ScannedArtifactService.cs | Implementation | Updated 3 methods | ✅ Complete |
| ScannedArtifactsController.cs | Controller | Added lang query param to 3 endpoints | ✅ Complete |
| **Total** | **3 files** | **9 changes** | **✅ Complete** |

Build Status: ✅ **SUCCESSFUL**
