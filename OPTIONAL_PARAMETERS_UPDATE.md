# Tour Recommendation Endpoint - Optional Parameters Update

## Summary

Successfully updated the `GET /api/tours/recommend` endpoint to make all query parameters optional while maintaining intelligent priority-based scoring for provided fields.

---

## Changes Made

### 1. **ToursController.cs**
**File:** `EgyptianMuseum.API\Controllers\ToursController.cs`

**Updates:**
- Changed parameters to nullable:
  ```csharp
  string? category
  int? durationMinutes
  int? numberOfRooms
  ```

- Updated validation to allow optional parameters:
  - Rejects request only if ALL parameters are missing
  - Validates provided values (durationMinutes > 0, numberOfRooms > 0)
  - Allows any combination of parameters

- Enhanced logging to show "not specified" for missing parameters

---

### 2. **ITourService.cs**
**File:** `EgyptianMuseum.Application\Interfaces\ITourService.cs`

**Updated Method Signature:**
```csharp
Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
    string? category, 
    int? durationMinutes, 
    int? numberOfRooms, 
    CancellationToken cancellationToken = default);
```

---

### 3. **TourService.cs**
**File:** `EgyptianMuseum.Application\Services\Tours\TourService.cs`

**Updated Implementation:**

```csharp
public async Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
    string? category, 
    int? durationMinutes, 
    int? numberOfRooms, 
    CancellationToken cancellationToken = default)
{
    // Validate that at least one parameter is provided
    if (string.IsNullOrWhiteSpace(category) && !durationMinutes.HasValue && !numberOfRooms.HasValue)
        throw new ArgumentException("At least one filter parameter is required...");

    // Validate provided values
    if (durationMinutes.HasValue && durationMinutes <= 0)
        throw new ArgumentException("Duration must be greater than 0");

    if (numberOfRooms.HasValue && numberOfRooms <= 0)
        throw new ArgumentException("Number of rooms must be greater than 0");

    var tours = await _tourRepository.GetAllWithRoomsAsync(cancellationToken);

    var recommendedTours = tours
        .Select(tour =>
        {
            // Calculate scores only for provided filters
            var categoryMatched = !string.IsNullOrWhiteSpace(category) && 
                string.Equals(tour.Category, category, StringComparison.OrdinalIgnoreCase);
            
            var durationDifference = durationMinutes.HasValue 
                ? Math.Abs(tour.DurationMinutes - durationMinutes.Value) 
                : int.MaxValue;  // Not compared if not provided
            
            var roomDifference = numberOfRooms.HasValue 
                ? Math.Abs(tour.TourRooms.Count - numberOfRooms.Value) 
                : int.MaxValue;  // Not compared if not provided

            return new RecommendedTourResponseDto
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                DurationMinutes = tour.DurationMinutes,
                Category = tour.Category,
                RoomsCount = tour.TourRooms.Count,
                DurationDifference = durationDifference,
                RoomDifference = roomDifference,
                CategoryMatched = categoryMatched
            };
        })
        .OrderBy(r => !r.CategoryMatched ? 1 : 0)   // Category match first (if provided)
        .ThenBy(r => r.DurationDifference)           // Then closest duration (if provided)
        .ThenBy(r => r.RoomDifference)               // Then closest room count (if provided)
        .ToList();

    return recommendedTours;
}
```

---

## Scoring Logic

### Category Filter
- Calculated **only if** `category` is provided
- Sets `CategoryMatched = true` if tour.Category matches (case-insensitive)
- Sets `DurationDifference = int.MaxValue` if not filtering by duration
- Sorts: Category matches (0) first, non-matches (1) second

### Duration Filter
- Calculated **only if** `durationMinutes` has a value
- `DurationDifference = Math.Abs(tour.DurationMinutes - durationMinutes)`
- Sets `DurationDifference = int.MaxValue` if not filtering by duration
- Sorts: Smallest difference first

### Room Count Filter
- Calculated **only if** `numberOfRooms` has a value
- `RoomDifference = Math.Abs(tour.TourRooms.Count - numberOfRooms)`
- Sets `RoomDifference = int.MaxValue` if not filtering by rooms
- Sorts: Smallest difference first

---

## API Examples

### Example 1: Category Only
```
GET /api/tours/recommend?category=Pharaoh
```

**Response:** Tours sorted by category match only
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Pharaoh Tour",
      "category": "Pharaoh",
      "categoryMatched": true,
      "durationDifference": 2147483647,
      "roomDifference": 2147483647
    }
  ]
}
```

### Example 2: Duration Only
```
GET /api/tours/recommend?durationMinutes=30
```

**Response:** Tours sorted by closest duration only
```json
{
  "success": true,
  "data": [
    {
      "id": 2,
      "name": "30-Minute Tour",
      "durationMinutes": 30,
      "durationDifference": 0,
      "categoryMatched": false,
      "roomDifference": 2147483647
    }
  ]
}
```

### Example 3: Room Count Only
```
GET /api/tours/recommend?numberOfRooms=4
```

**Response:** Tours sorted by closest room count only
```json
{
  "success": true,
  "data": [
    {
      "id": 3,
      "name": "4-Room Tour",
      "roomsCount": 4,
      "roomDifference": 0,
      "categoryMatched": false,
      "durationDifference": 2147483647
    }
  ]
}
```

### Example 4: Category + Duration
```
GET /api/tours/recommend?category=Family&durationMinutes=40
```

**Response:** Tours sorted by category first, then duration
```json
{
  "success": true,
  "data": [
    {
      "id": 4,
      "name": "Family Tour",
      "category": "Family",
      "durationMinutes": 40,
      "categoryMatched": true,
      "durationDifference": 0,
      "roomDifference": 2147483647
    }
  ]
}
```

### Example 5: All Parameters
```
GET /api/tours/recommend?category=Kids&durationMinutes=20&numberOfRooms=2
```

**Response:** Full priority sorting applied
```json
{
  "success": true,
  "data": [
    {
      "id": 5,
      "name": "Kids Tour",
      "category": "Kids",
      "durationMinutes": 20,
      "roomsCount": 2,
      "categoryMatched": true,
      "durationDifference": 0,
      "roomDifference": 0
    }
  ]
}
```

---

## Error Cases

### Missing All Parameters
```
GET /api/tours/recommend
```

**Response:** 400 Bad Request
```json
{
  "success": false,
  "message": "At least one filter parameter is required: category, durationMinutes, or numberOfRooms"
}
```

### Invalid Duration
```
GET /api/tours/recommend?durationMinutes=0
```

**Response:** 400 Bad Request
```json
{
  "success": false,
  "message": "Duration must be greater than 0"
}
```

### Invalid Room Count
```
GET /api/tours/recommend?numberOfRooms=-5
```

**Response:** 400 Bad Request
```json
{
  "success": false,
  "message": "Number of rooms must be greater than 0"
}
```

---

## Sorting Priority (Always Maintained)

1. **Category Match** (highest priority if provided)
   - Tours with matching category appear first
   - Set to `int.MaxValue` if category not provided (doesn't affect sorting)

2. **Duration Difference** (secondary priority if provided)
   - Tours with closest duration appear next
   - Set to `int.MaxValue` if duration not provided (doesn't affect sorting)

3. **Room Count Difference** (tertiary priority if provided)
   - Tours with closest room count appear last
   - Set to `int.MaxValue` if room count not provided (doesn't affect sorting)

---

## Validation Rules

| Parameter | Required | Valid Values | Notes |
|-----------|----------|--------------|-------|
| category | Optional | Non-empty string | Case-insensitive matching |
| durationMinutes | Optional | Integer > 0 | Rejects 0 or negative |
| numberOfRooms | Optional | Integer > 0 | Rejects 0 or negative |
| At least one | Required | One or more | Rejects empty request |

---

## Key Improvements

✅ **Flexible Filtering** - Users can search by any combination of parameters  
✅ **Smart Scoring** - Only considers provided filters  
✅ **Backward Compatible** - Existing logic preserved, just more flexible  
✅ **Intelligent Sorting** - Priority maintained based on provided filters  
✅ **Consistent Validation** - Only rejects if ALL parameters missing or values invalid  
✅ **Clean Code** - Uses nullable types and HasValue checks  
✅ **Better Logging** - Shows which parameters are actually used  

---

## Build Status

✅ **Build Successful** - No errors or warnings

---

## Files Modified

1. ✅ `EgyptianMuseum.API\Controllers\ToursController.cs` - Updated RecommendTours endpoint
2. ✅ `EgyptianMuseum.Application\Interfaces\ITourService.cs` - Updated method signature
3. ✅ `EgyptianMuseum.Application\Services\Tours\TourService.cs` - Updated implementation

---

## No Breaking Changes

- All existing Tour CRUD endpoints remain unchanged
- No database schema modifications
- No migrations required
- Clean Architecture principles maintained
