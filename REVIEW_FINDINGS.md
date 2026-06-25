# 🔍 Relationships & Authorization Review - FINDINGS REPORT

## Executive Summary

✅ **Good News:** Most relationships are correctly implemented!
⚠️ **Issues Found:** Minor issues with FeedbackController authorization and missing Room-Pieces relationship

---

## FINDINGS

### 1. ROOM RELATIONSHIPS ✅ PARTIALLY CORRECT

**What's Correct:**
- ✅ Room → Map (belongs to one)
- ✅ Room → TourRooms (has many)
- ✅ Room entity has: Id, Name, Description, MapId, XCoord, YCoord, IsDeleted, CreatedAt, UpdatedAt
- ✅ EF configuration includes Map FK with Cascade delete

**What's Missing:**
- ❌ Room → Pieces (has many) - **NOT DEFINED**
  - Room entity does NOT have `ICollection<Pieces> Pieces`
  - Pieces entity does NOT have `int RoomId` or `Room Room` navigation
  - EF configuration does NOT configure this relationship

**Impact:** Cannot query pieces by room. Room-specific artifact filtering is not possible.

---

### 2. TOUR RELATIONSHIPS ✅ CORRECT

**All Good:**
- ✅ Tour → TourRooms (has many)
- ✅ Tour entity has: Id, Name, Description, DurationMinutes, TourRooms, IsDeleted, CreatedAt, UpdatedAt
- ✅ NO StartTime/EndTime (as required)
- ✅ TourRoom entity has: TourId, RoomId, Order, Tour, Room
- ✅ EF configuration:
  - Composite key (TourId, RoomId) ✅
  - Tour → TourRooms: Cascade delete ✅
  - Room → TourRooms: Restrict delete ✅
  - Index on (TourId, Order) ✅

---

### 3. USERTOUR RELATIONSHIPS ❌ NOT IMPLEMENTED

**Status:** UserTour does NOT exist in the codebase.

**Check:** No UserTour entity file found.
No UserTour DbSet in AppDbContext.
No UserTourRepository or UserTourService.
No UserToursController.

**Recommendation:** DO NOT create UserTour now (as per your instructions - only if referenced).

---

### 4. AUTHORIZATION CONSISTENCY ⚠️ NEEDS FIXES

**Controllers Reviewed:**

| Controller | Status | Issue |
|-----------|--------|-------|
| ScannedArtifactsController | ✅ CORRECT | `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` on class |
| ChatController | ✅ CORRECT | `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` on class |
| FeedbackController | ⚠️ INCONSISTENT | `[Authorize]` on POST/DELETE methods, but NOT on class. GET is public. |
| ToursController | ✅ CORRECT | No `[Authorize]` - public read-only GET endpoints |
| RoomsController | ✅ CORRECT | No `[Authorize]` - public read-only GET endpoints |
| MapsController | ✅ CORRECT | No `[Authorize]` - public read-only GET endpoints |
| PiecesController | ⚠️ NO CLASS ATTRIBUTE | No class-level `[Authorize]`, but individual methods have `[Authorize]` |
| IndoorMapPathsController | ✅ CORRECT | No `[Authorize]` - public read-only GET endpoints |

**Issues Found:**

1. **FeedbackController:**
   - ❌ No `[Authorize]` at class level
   - ❌ POST has `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`
   - ❌ GET endpoints are public (correct behavior, but inconsistent with class-level authorization)
   - ❌ DELETE has `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`
   - **Fix:** Add `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` to class level, keep GET methods public with `[AllowAnonymous]`

2. **PiecesController:**
   - ❌ No class-level `[Authorize]`
   - ❌ Individual methods have `[Authorize]` inconsistently
   - **Fix:** No action needed if this is intentional. If expecting authorized access, add class-level `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`

---

## DETAILED FINDINGS

### Room - Pieces Relationship Missing

**Current State:**
```csharp
// Room.cs
public class Room : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int MapId { get; set; }
    public Map Map { get; set; }
    public double XCoord { get; set; }
    public double YCoord { get; set; }
    public ICollection<TourRoom> TourRooms { get; set; } // ✅ Has this
    // ❌ MISSING: public ICollection<Pieces> Pieces { get; set; }
}

// Pieces.cs
public class Pieces : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string PhotoPath { get; set; }
    public ICollection<PieceTranslation> Translations { get; set; }
    public ICollection<ScannedArtifact> ScannedArtifacts { get; set; }
    // ❌ MISSING: public int? RoomId { get; set; }
    // ❌ MISSING: public Room Room { get; set; }
}
```

**Required Fix:**
1. Add `ICollection<Pieces> Pieces` to Room entity
2. Add `int? RoomId` and `Room Room` to Pieces entity
3. Configure relationship in AppDbContext

---

### Authorization Issues

**FeedbackController Current:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase  // ❌ No [Authorize] at class level
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Repeated on each method
    public async Task<IActionResult> CreateFeedback(...)

    [HttpGet]  // ✅ Public - correct
    public async Task<IActionResult> GetUserFeedback(...)

    [HttpGet("target/{targetType}")]  // ✅ Public - correct
    public async Task<IActionResult> GetByTarget(...)

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Repeated on method
    public async Task<IActionResult> DeleteFeedback(...)
}
```

**Best Practice Fix:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // ✅ Add here
public class FeedbackController : ControllerBase
{
    [HttpPost]
    // Remove [Authorize] - inherited from class
    public async Task<IActionResult> CreateFeedback(...)

    [HttpGet]
    [AllowAnonymous]  // ✅ Allow public read
    public async Task<IActionResult> GetUserFeedback(...)

    [HttpGet("target/{targetType}")]
    [AllowAnonymous]  // ✅ Allow public read
    public async Task<IActionResult> GetByTarget(...)

    [HttpDelete("{id}")]
    // Remove [Authorize] - inherited from class
    public async Task<IActionResult> DeleteFeedback(...)
}
```

---

## SUMMARY OF REQUIRED FIXES

### 1. Room-Pieces Relationship (DATABASE MIGRATION NEEDED)
**Files to modify:**
- Room.cs - Add Pieces collection
- Pieces.cs - Add RoomId and Room navigation
- AppDbContext.cs - Configure relationship

**Migration:** YES, required

### 2. FeedbackController Authorization (NO MIGRATION NEEDED)
**File to modify:**
- FeedbackController.cs - Add class-level [Authorize], add [AllowAnonymous] to public GET

**Migration:** NO

### 3. Everything Else ✅
- Tour relationships: Correct
- TourRoom relationships: Correct
- Controllers: Mostly correct (except FeedbackController)
- Database configuration: Correct (except Room-Pieces missing)

---

## MIGRATION IMPACT

**Migration Required?** YES

**Reason:** Room-Pieces relationship requires:
1. Adding `RoomId` (nullable int) column to Pieces table
2. Adding foreign key constraint
3. Configuring delete behavior

**Migration Command:**
```powershell
Add-Migration FixRoomTourRelationsAndAuthorization
Update-Database
```

---

## WHAT WILL BE FIXED

1. ✅ Add Room-Pieces relationship (with proper FK configuration)
2. ✅ Fix FeedbackController authorization consistency
3. ✅ Ensure TourRoom.TourRooms navigation is configured
4. ✅ Verify all delete behaviors are safe

---

## WHAT WILL NOT BE CHANGED

- ❌ Will NOT add UserTour (not referenced in code)
- ❌ Will NOT modify working endpoints
- ❌ Will NOT change other modules
- ❌ Will NOT remove existing features
- ❌ Will NOT modify unrelated business logic

---

**Status:** Ready to apply fixes
**Impact:** Low - Adding missing relationship, fixing authorization style
**Breaking Changes:** None
**Database Changes:** Required (RoomId column addition)
