# Tour Recommendation Endpoint - Quick Reference

## Endpoint
```
GET /api/tours/recommend
```

## Query Parameters
- `category` (required): Tour category
- `durationMinutes` (required, > 0): Desired duration
- `numberOfRooms` (required, > 0): Desired room count

## Example Requests

### Perfect Match
```
GET /api/tours/recommend?category=Pharaoh&durationMinutes=30&numberOfRooms=3
```

### Different Duration
```
GET /api/tours/recommend?category=Pharaoh&durationMinutes=60&numberOfRooms=5
```

### Different Category
```
GET /api/tours/recommend?category=Dynasty&durationMinutes=45&numberOfRooms=4
```

## Response Structure
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Tour Name",
      "description": "Tour Description",
      "durationMinutes": 30,
      "category": "Pharaoh",
      "roomsCount": 3,
      "durationDifference": 0,
      "roomDifference": 0,
      "categoryMatched": true
    }
  ],
  "count": 1
}
```

## Sorting Logic
1. **CategoryMatched** (ASC) - True comes first
2. **DurationDifference** (ASC) - Smallest difference first
3. **RoomDifference** (ASC) - Smallest difference first

## Error Responses

### Missing Required Parameter
```json
{
  "success": false,
  "message": "Category is required"
}
```

### Invalid Duration
```json
{
  "success": false,
  "message": "Duration must be greater than 0"
}
```

### Invalid Room Count
```json
{
  "success": false,
  "message": "Number of rooms must be greater than 0"
}
```

## Files Modified
1. ✅ Created: `RecommendedTourResponseDto.cs`
2. ✅ Updated: `ITourRepository.cs` - Added `GetAllWithRoomsAsync`
3. ✅ Updated: `TourRepository.cs` - Implemented `GetAllWithRoomsAsync`
4. ✅ Updated: `ITourService.cs` - Added `RecommendToursAsync`
5. ✅ Updated: `TourService.cs` - Implemented `RecommendToursAsync` + Fixed `MapToDto`
6. ✅ Updated: `ToursController.cs` - Added `RecommendTours` endpoint

## Build Status
✅ **Successful** - No errors or warnings
