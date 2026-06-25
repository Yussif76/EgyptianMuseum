# Tour Recommendation Endpoint Implementation

## Summary

Successfully added a new GET endpoint `GET /api/tours/recommend` to the Tours module that recommends tours based on user preferences using priority-based scoring.

---

## Files Modified

### 1. **New DTO Created**
**File:** `EgyptianMuseum.Application\DTOs\Tours\RecommendedTourResponseDto.cs`

```csharp
public class RecommendedTourResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public string Category { get; set; } = null!;
    public int RoomsCount { get; set; }
    public int DurationDifference { get; set; }
    public int RoomDifference { get; set; }
    public bool CategoryMatched { get; set; }
}
```

---

### 2. **ITourRepository Interface**
**File:** `EgyptianMuseum.Application\Interfaces\ITourRepository.cs`

**Added Method:**
```csharp
Task<List<Tour>> GetAllWithRoomsAsync(CancellationToken cancellationToken = default);
```

---

### 3. **TourRepository Implementation**
**File:** `EgyptianMuseum.Infrastructure\Repositories\TourRepository.cs`

**Added Method:**
```csharp
public async Task<List<Tour>> GetAllWithRoomsAsync(CancellationToken cancellationToken = default)
{
    return await _context.Tours
        .Include(t => t.TourRooms)
        .Where(t => !t.IsDeleted)
        .OrderBy(t => t.Name)
        .ToListAsync(cancellationToken);
}
```

---

### 4. **ITourService Interface**
**File:** `EgyptianMuseum.Application\Interfaces\ITourService.cs`

**Added Method:**
```csharp
Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
    string category, 
    int durationMinutes, 
    int numberOfRooms, 
    CancellationToken cancellationToken = default);
```

---

### 5. **TourService Implementation**
**File:** `EgyptianMuseum.Application\Services\Tours\TourService.cs`

**Added Method:**
```csharp
public async Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
    string category, 
    int durationMinutes, 
    int numberOfRooms, 
    CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(category))
        throw new ArgumentException("Category is required");

    if (durationMinutes <= 0)
        throw new ArgumentException("Duration must be greater than 0");

    if (numberOfRooms <= 0)
        throw new ArgumentException("Number of rooms must be greater than 0");

    var tours = await _tourRepository.GetAllWithRoomsAsync(cancellationToken);

    var recommendedTours = tours
        .Select(tour =>
        {
            var categoryScore = string.Equals(tour.Category, category, StringComparison.OrdinalIgnoreCase) ? 0 : 1;
            var durationDifference = Math.Abs(tour.DurationMinutes - durationMinutes);
            var roomDifference = Math.Abs(tour.TourRooms.Count - numberOfRooms);

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
                CategoryMatched = categoryScore == 0
            };
        })
        .OrderBy(r => r.CategoryMatched ? 0 : 1)
        .ThenBy(r => r.DurationDifference)
        .ThenBy(r => r.RoomDifference)
        .ToList();

    return recommendedTours;
}
```

**Also Fixed:**
- Updated `MapToDto` method to include Category field

---

### 6. **ToursController**
**File:** `EgyptianMuseum.API\Controllers\ToursController.cs`

**Added Endpoint:**
```csharp
[HttpGet("recommend")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> RecommendTours(
    [FromQuery] string category,
    [FromQuery] int durationMinutes,
    [FromQuery] int numberOfRooms,
    CancellationToken cancellationToken)
{
    try
    {
        if (string.IsNullOrWhiteSpace(category))
            return BadRequest(new { success = false, message = "Category is required" });

        if (durationMinutes <= 0)
            return BadRequest(new { success = false, message = "Duration must be greater than 0" });

        if (numberOfRooms <= 0)
            return BadRequest(new { success = false, message = "Number of rooms must be greater than 0" });

        _logger.LogInformation("Recommending tours for category: {Category}, duration: {Duration}, rooms: {Rooms}",
            category, durationMinutes, numberOfRooms);

        var recommendedTours = await _tourService.RecommendToursAsync(category, durationMinutes, numberOfRooms, cancellationToken);
        return Ok(new { success = true, data = recommendedTours, count = recommendedTours.Count });
    }
    catch (ArgumentException ex)
    {
        _logger.LogWarning("Invalid argument: {Message}", ex.Message);
        return BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error recommending tours");
        return StatusCode(StatusCodes.Status500InternalServerError,
            new { success = false, message = "Error recommending tours" });
    }
}
```

---

## API Usage

### Endpoint
```
GET /api/tours/recommend
```

### Query Parameters
- `category` (string, required): Category to match (e.g., "Pharaoh")
- `durationMinutes` (int, required): Desired tour duration in minutes (must be > 0)
- `numberOfRooms` (int, required): Desired number of rooms (must be > 0)

### Example Request
```
GET /api/tours/recommend?category=Pharaoh&durationMinutes=30&numberOfRooms=3
```

### Example Response
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Quick Pharaoh Tour",
      "description": "Short tour for pharaoh artifacts",
      "durationMinutes": 30,
      "category": "Pharaoh",
      "roomsCount": 3,
      "durationDifference": 0,
      "roomDifference": 0,
      "categoryMatched": true
    },
    {
      "id": 2,
      "name": "Extended Pharaoh Tour",
      "description": "Extended tour for pharaoh artifacts",
      "durationMinutes": 45,
      "category": "Pharaoh",
      "roomsCount": 4,
      "durationDifference": 15,
      "roomDifference": 1,
      "categoryMatched": true
    },
    {
      "id": 3,
      "name": "Dynasty Tour",
      "description": "Tour featuring various dynasties",
      "durationMinutes": 30,
      "category": "Dynasty",
      "roomsCount": 3,
      "durationDifference": 0,
      "roomDifference": 0,
      "categoryMatched": false
    }
  ],
  "count": 3
}
```

---

## Scoring Algorithm

Tours are ranked by the following priority:

1. **Category Match** (highest priority)
   - Score 0 if category matches (case-insensitive)
   - Score 1 if category doesn't match

2. **Duration Difference** (secondary priority)
   - `Math.Abs(tour.DurationMinutes - requestedDurationMinutes)`
   - Closest match wins

3. **Room Count Difference** (tertiary priority)
   - `Math.Abs(tour.TourRooms.Count - requestedNumberOfRooms)`
   - Closest match wins

### Example Sorting
For request: `category=Pharaoh, duration=30, rooms=3`

- Tour A: Category match (0), Duration diff 0, Rooms diff 0 → **Rank 1**
- Tour B: Category match (0), Duration diff 5, Rooms diff 0 → **Rank 2**
- Tour C: Category mismatch (1), Duration diff 0, Rooms diff 0 → **Rank 3**

---

## Validation

**Request Validation:**
- `category`: Required, non-empty string
- `durationMinutes`: Required, must be > 0
- `numberOfRooms`: Required, must be > 0

**Error Responses:**
- 400 Bad Request: Invalid query parameters
- 500 Internal Server Error: Server error during processing

---

## Key Features

✅ **Non-Strict Matching**
- Tours with similar durations and room counts are returned
- No exact matching required

✅ **Priority-Based Ranking**
- Category match is highest priority
- Then closest duration
- Then closest room count

✅ **Clean Architecture**
- Follows existing patterns
- DTO-based responses
- Service layer abstraction
- Repository pattern for data access

✅ **Async/Await**
- Full async implementation
- Proper CancellationToken support

✅ **Logging**
- Request logging at Information level
- Error logging with full context

✅ **No Breaking Changes**
- All existing endpoints remain unchanged
- No migrations required (Category already exists)

---

## Testing

### Success Case
```
GET /api/tours/recommend?category=Pharaoh&durationMinutes=30&numberOfRooms=3
Response: 200 OK with sorted recommendations
```

### Validation Cases
```
GET /api/tours/recommend?category=&durationMinutes=30&numberOfRooms=3
Response: 400 Bad Request - "Category is required"

GET /api/tours/recommend?category=Pharaoh&durationMinutes=0&numberOfRooms=3
Response: 400 Bad Request - "Duration must be greater than 0"

GET /api/tours/recommend?category=Pharaoh&durationMinutes=30&numberOfRooms=0
Response: 400 Bad Request - "Number of rooms must be greater than 0"
```

---

## Build Status

✅ **Build Successful** - All code compiles without errors or warnings
