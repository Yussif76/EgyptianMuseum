# Smart Scan API - Usage Examples & Testing Guide

## API Endpoints Overview

All endpoints support multilingual responses via `lang` query parameter.

---

## 1. SCAN ARTIFACT (Smart Scan)

**POST** `/api/scannedartifacts/scan`

### Request

```json
POST /api/scannedartifacts/scan?lang=en
Content-Type: application/json
Authorization: Bearer {token}

{
  "labelText": "GEM300"
}
```

### Success Response (New Scan) - 201 Created

```json
{
  "success": true,
  "data": {
    "scannedArtifactId": 1,
    "pieceId": 14,
    "labelText": "GEM300",
    "isFavorite": false,
    "scannedAt": "2026-05-02T10:30:15.123Z",
    "pieceName": "Tutankhamun Mask",
    "pieceDescription": "The famous funerary mask of Tutankhamun",
    "pieceImageUrl": "images/tutankhamun.jpg",
    "piecePeriod": "New Kingdom",
    "pieceCategory": "Artifacts"
  }
}
```

### Duplicate Scan Response - 201 Created

```json
{
  "success": true,
  "data": {
    "scannedArtifactId": 1,
    "pieceId": 14,
    "labelText": "GEM300",
    "isFavorite": true,  // Preserved from existing record
    "scannedAt": "2026-05-02T10:30:15.123Z",
    "pieceName": "Tutankhamun Mask",
    "pieceDescription": "The famous funerary mask of Tutankhamun",
    "pieceImageUrl": "images/tutankhamun.jpg",
    "piecePeriod": "New Kingdom",
    "pieceCategory": "Artifacts"
  }
}
```

### Piece Not Found - 404 Not Found

```json
{
  "success": false,
  "message": "No artifact found with label 'INVALID'"
}
```

### Invalid Request - 400 Bad Request

```json
{
  "success": false,
  "message": "Label text cannot be empty"
}
```

### Missing Authorization - 401 Unauthorized

```json
{
  "success": false,
  "message": "User not authenticated"
}
```

---

## 2. GET USER'S SCANNED ARTIFACTS

**GET** `/api/scannedartifacts`

### Request - English

```
GET /api/scannedartifacts?lang=en
Authorization: Bearer {token}
```

### Request - Arabic

```
GET /api/scannedartifacts?lang=ar
Authorization: Bearer {token}
```

### Success Response - 200 OK

```json
{
  "success": true,
  "message": "Scanned artifacts retrieved successfully",
  "data": [
    {
      "id": 1,
      "pieceId": 14,
      "labelText": "GEM300",
      "isFavorite": true,
      "scannedAt": "2026-05-02T10:30:15.123Z",
      "pieceName": "Tutankhamun Mask",
      "pieceDescription": "The famous funerary mask of Tutankhamun",
      "pieceImageUrl": "images/tutankhamun.jpg",
      "piecePeriod": "New Kingdom",
      "pieceCategory": "Artifacts"
    },
    {
      "id": 2,
      "pieceId": 20,
      "labelText": "SCARAB001",
      "isFavorite": false,
      "scannedAt": "2026-05-02T09:15:45.456Z",
      "pieceName": "Scarab Amulet",
      "pieceDescription": "Ancient scarab amulet from Old Kingdom",
      "pieceImageUrl": "images/scarab.jpg",
      "piecePeriod": "Old Kingdom",
      "pieceCategory": "Amulets"
    }
  ]
}
```

**Note**: Results are ordered by `ScannedAt` in descending order (newest first).

---

## 3. GET SINGLE SCANNED ARTIFACT

**GET** `/api/scannedartifacts/{id}`

### Request

```
GET /api/scannedartifacts/1?lang=en
Authorization: Bearer {token}
```

### Success Response - 200 OK

```json
{
  "success": true,
  "message": "Scanned artifact retrieved successfully",
  "data": {
    "id": 1,
    "pieceId": 14,
    "labelText": "GEM300",
    "isFavorite": true,
    "scannedAt": "2026-05-02T10:30:15.123Z",
    "pieceName": "Tutankhamun Mask",
    "pieceDescription": "The famous funerary mask of Tutankhamun",
    "pieceImageUrl": "images/tutankhamun.jpg",
    "piecePeriod": "New Kingdom",
    "pieceCategory": "Artifacts"
  }
}
```

### Not Found - 404

```json
{
  "success": false,
  "message": "Scanned artifact not found"
}
```

### Unauthorized (Not Artifact Owner) - 403 Forbidden

```json
{
  "success": false,
  "message": "Error message"
}
```

---

## 4. GET USER'S FAVORITE ARTIFACTS

**GET** `/api/scannedartifacts/favorites`

### Request

```
GET /api/scannedartifacts/favorites?lang=ar
Authorization: Bearer {token}
```

### Success Response - 200 OK

```json
{
  "success": true,
  "message": "Favorite artifacts retrieved successfully",
  "data": [
    {
      "id": 1,
      "pieceId": 14,
      "labelText": "GEM300",
      "isFavorite": true,
      "scannedAt": "2026-05-02T10:30:15.123Z",
      "pieceName": "قناع توت عنخ آمون",
      "pieceDescription": "قناع الجنازة الشهير",
      "pieceImageUrl": "images/tutankhamun.jpg",
      "piecePeriod": "الدولة الحديثة",
      "pieceCategory": "التحف"
    }
  ]
}
```

---

## 5. UPDATE FAVORITE STATUS BY SCANNED ARTIFACT ID

**PUT** `/api/scannedartifacts/{id}/favorite`

### Request

```json
PUT /api/scannedartifacts/1/favorite
Content-Type: application/json
Authorization: Bearer {token}

{
  "isFavorite": true
}
```

### Success Response - 204 No Content

```
(Empty body)
```

---

## 6. UPDATE FAVORITE STATUS BY PIECE ID

**PUT** `/api/scannedartifacts/pieces/{pieceId}/favorite`

### Request

```json
PUT /api/scannedartifacts/pieces/14/favorite
Content-Type: application/json
Authorization: Bearer {token}

{
  "isFavorite": true
}
```

### Behavior
- If user hasn't scanned this piece yet, a new scan record is created with the favorite status
- If user already scanned this piece, favorite status is updated

### Success Response - 200 OK

```json
{
  "success": true,
  "message": "Favorite status updated successfully"
}
```

### Piece Not Found - 404

```json
{
  "success": false,
  "message": "Piece not found"
}
```

---

## 7. DELETE SCANNED ARTIFACT

**DELETE** `/api/scannedartifacts/{id}`

### Request

```
DELETE /api/scannedartifacts/1
Authorization: Bearer {token}
```

### Success Response - 204 No Content

```
(Empty body)
```

### Not Found - 404

```json
{
  "success": false,
  "message": "Scanned artifact not found"
}
```

---

## Multilingual Translation Fallback Example

### Scenario: Requesting Arabic translation

```
GET /api/scannedartifacts/1?lang=ar
Authorization: Bearer {token}
```

**Fallback Priority**:
1. ✅ If `PieceTranslation.LanguageCode == "ar"` exists → Use Arabic translation
2. 🔄 If Arabic not found, check for `LanguageCode == "en"` → Use English
3. 🔄 If English not found → Use first available translation
4. 🔄 If no translations → Use `Piece.Name` directly

### Response Example

```json
{
  "success": true,
  "data": {
    "pieceName": "قناع توت عنخ آمون",          // Arabic translation (priority 1)
    "pieceDescription": "قناع الجنازة الشهير",
    "piecePeriod": "الدولة الحديثة",
    "pieceCategory": "التحف"
  }
}
```

---

## Smart Scan Logic Flow

```
1. User submits scan request with "GEM300"
   ↓
2. Service fetches Piece by code "GEM300"
   ├─ Not found? → 404 Not Found
   └─ Found? → Continue
   ↓
3. Check if user already has ScannedArtifact for this piece
   ├─ Exists? → Return existing (with preserved IsFavorite)
   └─ Not exists? → Continue
   ↓
4. Create new ScannedArtifact:
   - UserId = authenticated user
   - PieceId = piece.Id
   - LabelText = "GEM300"
   - IsFavorite = false
   - ScannedAt = DateTime.UtcNow
   ↓
5. Save to database
   ↓
6. Return with selected translation (by requested language)
   ↓
7. Response: 201 Created
```

---

## CURL Examples

### Scan New Artifact (English)

```bash
curl -X POST http://localhost:5000/api/scannedartifacts/scan?lang=en \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"labelText":"GEM300"}'
```

### Scan New Artifact (Arabic)

```bash
curl -X POST http://localhost:5000/api/scannedartifacts/scan?lang=ar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"labelText":"GEM300"}'
```

### Get All Scanned Artifacts (English)

```bash
curl -X GET "http://localhost:5000/api/scannedartifacts?lang=en" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Get Favorites (Arabic)

```bash
curl -X GET "http://localhost:5000/api/scannedartifacts/favorites?lang=ar" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Toggle Favorite

```bash
curl -X PUT http://localhost:5000/api/scannedartifacts/1/favorite \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"isFavorite":true}'
```

### Delete Scan

```bash
curl -X DELETE http://localhost:5000/api/scannedartifacts/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Testing Checklist

### Functional Tests
- [ ] First scan creates new record
- [ ] Duplicate scan returns existing record
- [ ] Duplicate scan preserves IsFavorite status
- [ ] Scan with non-existent label returns 404
- [ ] Scan with empty label returns 400
- [ ] Unauthorized user gets 401
- [ ] Wrong user cannot access other user's scans (403)

### Language Tests
- [ ] English translation selected when lang=en
- [ ] Arabic translation selected when lang=ar
- [ ] Fallback to English when requested language not found
- [ ] Fallback to first available when English not found
- [ ] Fallback to Piece.Name when no translations

### List/Filter Tests
- [ ] GetUserScannedArtifacts returns all user's scans
- [ ] Results ordered by ScannedAt (newest first)
- [ ] GetUserFavorites returns only favorite scans
- [ ] Language parameter works on GET endpoints

### Favorite Tests
- [ ] Toggle favorite by ID works
- [ ] Toggle favorite by Piece ID creates new scan if needed
- [ ] Favorite status persists across requests

### Edge Cases
- [ ] Update favorite on already-deleted scan
- [ ] Concurrent scans of same piece by different users
- [ ] Language code case sensitivity
- [ ] Special characters in LabelText
