# Tours Module - Quick Reference

## Files Modified/Created

### Created Files (15 new files)

**Domain Layer:**
1. `EgyptianMuseum.Domain\Entities\Tour.cs`
2. `EgyptianMuseum.Domain\Entities\TourRoom.cs`

**Application Layer - Interfaces:**
3. `EgyptianMuseum.Application\Interfaces\ITourRepository.cs`
4. `EgyptianMuseum.Application\Interfaces\ITourRoomRepository.cs`
5. `EgyptianMuseum.Application\Interfaces\ITourService.cs`

**Application Layer - DTOs:**
6. `EgyptianMuseum.Application\DTOs\Tours\CreateTourRequestDto.cs`
7. `EgyptianMuseum.Application\DTOs\Tours\UpdateTourRequestDto.cs`
8. `EgyptianMuseum.Application\DTOs\Tours\TourResponseDto.cs`
9. `EgyptianMuseum.Application\DTOs\Tours\AddRoomToTourRequestDto.cs`
10. `EgyptianMuseum.Application\DTOs\Tours\TourRoomResponseDto.cs`
11. `EgyptianMuseum.Application\DTOs\Tours\TourDetailsResponseDto.cs`

**Application Layer - Services:**
12. `EgyptianMuseum.Application\Services\Tours\TourService.cs`

**Infrastructure Layer:**
13. `EgyptianMuseum.Infrastructure\Repositories\TourRepository.cs`
14. `EgyptianMuseum.Infrastructure\Repositories\TourRoomRepository.cs`

**API Layer:**
15. `EgyptianMuseum.API\Controllers\ToursController.cs`

### Modified Files (2 files)

1. `EgyptianMuseum.Infrastructure\Data\AppDbContext.cs`
   - Added DbSet<Tour>
   - Added DbSet<TourRoom>
   - Configured Tour entity
   - Configured TourRoom entity with composite key

2. `EgyptianMuseum.API\Program.cs`
   - Added using statement for Tours service
   - Registered ITourService, TourService
   - Registered ITourRepository, TourRepository
   - Registered ITourRoomRepository, TourRoomRepository

## How to Test

### 1. Apply Migration

```powershell
# In Package Manager Console
Add-Migration AddToursModule
Update-Database
```

### 2. Test Create Tour

**POST** `/api/tours`
```json
{
  "name": "Ancient Kings Tour",
  "description": "Explore the tombs and artifacts",
  "durationMinutes": 120
}
```

### 3. Test Add Room to Tour

**POST** `/api/tours/1/rooms`
```json
{
  "roomId": 1,
  "order": 1
}
```

### 4. Test Get Tour Details

**GET** `/api/tours/1/details`

Response includes tour info + all rooms ordered by visit order.

### 5. Test Get Tour Rooms

**GET** `/api/tours/1/rooms`

Lists all rooms in tour, ordered by Order property.

## Key Endpoints Summary

| Operation | Method | Path |
|-----------|--------|------|
| List all tours | GET | `/api/tours` |
| Get tour | GET | `/api/tours/{id}` |
| Create tour | POST | `/api/tours` |
| Update tour | PUT | `/api/tours/{id}` |
| Delete tour | DELETE | `/api/tours/{id}` |
| Add room | POST | `/api/tours/{tourId}/rooms` |
| List rooms | GET | `/api/tours/{tourId}/rooms` |
| Get details | GET | `/api/tours/{tourId}/details` |
| Remove room | DELETE | `/api/tours/{tourId}/rooms/{roomId}` |

## Database Schema

### Tours Table
```
Id (int, PK)
Name (nvarchar(255), required)
Description (nvarchar(1000), required)
DurationMinutes (int, required)
IsDeleted (bit, default 0)
CreatedAt (datetime2, nullable)
UpdatedAt (datetime2, nullable)
```

### TourRooms Table (Join)
```
TourId (int, FK→Tours, PK part 1)
RoomId (int, FK→Rooms, PK part 2)
Order (int, required)
Index: (TourId, Order)
```

## Validation Rules

✅ Tour name is required
✅ Tour description is required
✅ DurationMinutes > 0
✅ Room must exist before adding to tour
✅ Tour must exist before adding room
✅ Prevent duplicate rooms in same tour
✅ Order must be greater than 0

## Error Handling

- **400 Bad Request** - Validation errors or invalid IDs
- **404 Not Found** - Tour or room not found
- **409 Conflict** - Room already exists in tour
- **500 Internal Server Error** - Unexpected errors

## Status Check

✅ Build: Successful
✅ Code: Clean Architecture compliant
✅ Async: All operations async/await
✅ Validation: Comprehensive
✅ Logging: Integrated
✅ DTOs: All operations use DTOs

## Ready to Deploy

The Tours module is complete and ready for:
1. Database migration
2. Testing
3. Integration with frontend
4. Future enhancements (UserTours, Navigation)
