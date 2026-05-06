# Quick Reference: GetByCode Enhancement

## Endpoint Summary

### GET /Pieces/GetByCode/{code}

```http
GET /Pieces/GetByCode/PH001?lang=en
Authorization: Bearer {jwt_token}
```

**What it does**:
- Retrieves a piece by its code
- **Automatically creates** a `ScannedArtifact` record (first time only)
- Returns piece with translations and scanned status

**Response**:
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
    "scannedArtifactId": 42,
    "isFavorite": false,
    "scannedAt": "2025-05-02T15:45:30.000Z"
  }
}
```

---

## Key Features

| Feature | Status | Notes |
|---------|--------|-------|
| **JWT Required** | ✅ Required | Use Authorization header |
| **Auto-Create Record** | ✅ Yes | Only on first view |
| **Prevent Duplicates** | ✅ Yes | Unique constraint (UserId, PieceId) |
| **Language Support** | ✅ Yes | Use `?lang=en` query parameter |
| **Logging** | ✅ Yes | All operations logged |
| **Error Handling** | ✅ Yes | Proper HTTP status codes |

---

## Implementation Details

### Code Path
```
PiecesController.GetByCodeWithTranslationsAsync()
    ↓
PiecesService.GetByCodeWithScannedStatusAsync()
    ↓
ScannedArtifactRepository.GetByUserIdAndPieceIdAsync()
    ↓
ScannedArtifactRepository.AddAsync() [if not exists]
```

### Database Schema
```sql
-- Unique constraint prevents duplicates
CREATE UNIQUE INDEX UK_ScannedArtifacts_UserId_PieceId
ON ScannedArtifacts(UserId, PieceId);
```

---

## Usage Examples

### cURL
```bash
curl -X GET "https://api.example.com/Pieces/GetByCode/PH001?lang=en" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### C# HttpClient
```csharp
var client = new HttpClient();
var token = "YOUR_JWT_TOKEN";
var code = "PH001";

client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync(
    $"https://api.example.com/Pieces/GetByCode/{code}?lang=en");

var json = await response.Content.ReadAsStringAsync();
```

### JavaScript/Fetch
```javascript
const token = "YOUR_JWT_TOKEN";
const code = "PH001";

const response = await fetch(
  `/Pieces/GetByCode/${code}?lang=en`,
  {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  }
);

const data = await response.json();
console.log(data);
```

---

## Error Handling

### 401 Unauthorized
```json
{
  "success": false,
  "message": "User not authenticated"
}
```
**Fix**: Include valid JWT token in Authorization header

### 404 Not Found
```json
{
  "success": false,
  "message": "Piece with Code INVALID not found."
}
```
**Fix**: Use a valid piece code (e.g., PH001)

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred"
}
```
**Fix**: Check server logs, database connection

---

## Query Parameters

| Parameter | Type | Required | Default | Example |
|-----------|------|----------|---------|---------|
| `code` | string | ✅ Yes | N/A | `PH001` |
| `lang` | string | ❌ No | `en` | `en`, `ar`, `fr` |

---

## Related Endpoints

### Get Piece by ID (Also auto-creates)
```http
GET /Pieces/{id}?lang=en
Authorization: Bearer {token}
```

### Toggle Favorite
```http
PUT /api/scanned-artifacts/pieces/{pieceId}/favorite
Authorization: Bearer {token}
Content-Type: application/json

{
  "isFavorite": true
}
```

### Get All Favorites
```http
GET /api/scanned-artifacts/favorites
Authorization: Bearer {token}
```

---

## Testing

### Test 1: First Call (Creates Record)
```
Call 1: GET /Pieces/GetByCode/PH001
Response: scannedArtifactId = 42, scannedAt = "2025-05-02T15:45:30.000Z"

Database: NEW record created
```

### Test 2: Second Call (Reuses Record)
```
Call 2: GET /Pieces/GetByCode/PH001 (same user, same code)
Response: scannedArtifactId = 42, scannedAt = "2025-05-02T15:45:30.000Z"

Database: SAME record (no new record created)
```

### Test 3: Different User (Different Record)
```
Call 1 (User A): GET /Pieces/GetByCode/PH001
Response: scannedArtifactId = 42

Call 2 (User B): GET /Pieces/GetByCode/PH001
Response: scannedArtifactId = 43  # DIFFERENT ID

Database: TWO records exist (one per user)
```

---

## Performance

| Metric | Value | Note |
|--------|-------|------|
| Time Complexity | O(2) | Check + Create |
| Space Complexity | O(1) | Single record |
| DB Roundtrips | 2 | Check existing + Create (batched) |
| Index Lookup | O(log n) | Unique index on (UserId, PieceId) |

---

## Security Notes

✅ **Secure**:
- JWT token required
- User ID extracted from authenticated claims
- SQL injection protected (EF Core)
- User data isolated (UserId check)

❌ **Not Secure**:
- Calling without JWT
- Using modified JWT tokens
- Exposing JWTs in logs

---

## Changelog

### Version 2.0 (May 2, 2025)
- ✅ Added `GetByCodeWithScannedStatusAsync` method
- ✅ Made endpoint JWT-required
- ✅ Auto-creates ScannedArtifact on first view
- ✅ Returns PieceWithScannedStatusDto
- ✅ Added proper error handling

### Version 1.0 (Previous)
- Basic GetByCode functionality
- No JWT requirement
- No ScannedArtifact tracking

---

## Troubleshooting

### Issue: 401 Unauthorized
```
Symptom: Always getting "User not authenticated"
Cause: Missing or invalid JWT token
Solution: 
  1. Get fresh JWT from login endpoint
  2. Include in Authorization header
  3. Verify token hasn't expired
```

### Issue: 404 Not Found for Valid Code
```
Symptom: Getting 404 even though piece exists
Cause: Code might be case-sensitive or have typo
Solution:
  1. Check exact piece code in database
  2. Verify query parameter formatting
  3. Try with different language parameter
```

### Issue: Duplicate ScannedArtifacts (shouldn't happen)
```
Symptom: Multiple records for same (UserId, PieceId)
Cause: Unique constraint not applied
Solution:
  1. Run migration: dotnet ef database update
  2. Verify constraint exists in database
  3. Check database logs for violations
```

---

## Migration Notes

If you need to update the endpoint, remember:

**DO**:
- ✅ Keep backward compatibility in service layer
- ✅ Use DTOs for responses
- ✅ Validate JWT before using
- ✅ Log all operations
- ✅ Test with multiple users

**DON'T**:
- ❌ Change method signature without updating interface
- ❌ Remove authentication requirement
- ❌ Expose entities directly
- ❌ Skip validation
- ❌ Hardcode user IDs

---

## Code Snippets

### Extract JWT in Controller
```csharp
private string? GetUserIdFromToken()
{
    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
```

### Auto-Create Logic
```csharp
var existingScanned = await scannedArtifactRepository
    .GetByUserIdAndPieceIdAsync(userId, piece.Id, cancellationToken);

if (existingScanned == null)
{
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
```

---

## Useful Commands

### List All Pieces
```http
GET /Pieces
```

### Get Specific Piece by ID
```http
GET /Pieces/{id}
Authorization: Bearer {token}
```

### Create New Piece (Admin)
```http
POST /Pieces
Content-Type: application/json

{
  "code": "PH001",
  "name": "Pharaoh Statue",
  "photoPath": "/images/pharaoh.jpg",
  "translations": [...]
}
```

---

## API Documentation

**API Endpoint**: `/Pieces/GetByCode/{code}`  
**Method**: `GET`  
**Authentication**: Required (JWT Bearer)  
**Response Format**: JSON  
**Status Codes**:
- `200`: Success
- `401`: Unauthorized (no JWT)
- `404`: Piece not found
- `500`: Server error

---

**Last Updated**: May 2, 2025  
**Maintained By**: Backend Team  
**Status**: Production Ready ✅
