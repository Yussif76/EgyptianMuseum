# Optional Parameters Update - Quick Reference

## Endpoint
```
GET /api/tours/recommend
```

## Parameters (All Optional - At Least One Required)

| Parameter | Type | Required | Valid | Example |
|-----------|------|----------|-------|---------|
| category | string | No | Non-empty | "Pharaoh" |
| durationMinutes | int | No | > 0 | 30 |
| numberOfRooms | int | No | > 0 | 4 |

## Valid Request Examples

### Category Only
```
GET /api/tours/recommend?category=Pharaoh
```

### Duration Only
```
GET /api/tours/recommend?durationMinutes=30
```

### Room Count Only
```
GET /api/tours/recommend?numberOfRooms=4
```

### Category + Duration
```
GET /api/tours/recommend?category=Family&durationMinutes=40
```

### Category + Room Count
```
GET /api/tours/recommend?category=Kids&numberOfRooms=3
```

### Duration + Room Count
```
GET /api/tours/recommend?durationMinutes=45&numberOfRooms=5
```

### All Parameters
```
GET /api/tours/recommend?category=Educational&durationMinutes=60&numberOfRooms=6
```

## Invalid Requests

### Missing All Parameters ❌
```
GET /api/tours/recommend
```
**Error:** "At least one filter parameter is required..."

### Invalid Duration ❌
```
GET /api/tours/recommend?durationMinutes=0
```
**Error:** "Duration must be greater than 0"

### Invalid Room Count ❌
```
GET /api/tours/recommend?numberOfRooms=-5
```
**Error:** "Number of rooms must be greater than 0"

## Sorting Logic

Tours are sorted by these priorities (in order):

1. **Category Match** (if category provided)
   - Matching tours appear first
   - Non-matching tours appear second

2. **Duration Difference** (if durationMinutes provided)
   - Closest duration first
   - Farthest duration last

3. **Room Difference** (if numberOfRooms provided)
   - Closest room count first
   - Farthest room count last

### Examples

**If only category provided:**
- Sort by: Category match

**If only duration provided:**
- Sort by: Closest duration

**If category + duration provided:**
- Sort by: Category match, then duration

**If all provided:**
- Sort by: Category match, then duration, then rooms

## Response Fields

```json
{
  "id": 1,
  "name": "Tour Name",
  "description": "Description",
  "durationMinutes": 30,
  "category": "Category",
  "roomsCount": 3,
  "durationDifference": 0,
  "roomDifference": 0,
  "categoryMatched": true
}
```

### Field Meanings

- **durationDifference**: Difference from requested duration (2147483647 if not filtering by duration)
- **roomDifference**: Difference from requested room count (2147483647 if not filtering by rooms)
- **categoryMatched**: True if category matches request (false if category not requested)

## Success Response Format

```json
{
  "success": true,
  "data": [...],
  "count": 5
}
```

## Error Response Format

```json
{
  "success": false,
  "message": "Error description"
}
```

## Files Updated

1. ✅ `ToursController.cs` - Nullable parameters, updated validation
2. ✅ `ITourService.cs` - Updated method signature
3. ✅ `TourService.cs` - Updated implementation with smart scoring

## Build Status

✅ **Successful** - No errors
