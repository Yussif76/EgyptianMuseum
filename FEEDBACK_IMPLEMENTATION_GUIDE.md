# Extended Feedback System - App-Wide Feedback Support

## Overview
This implementation extends your existing feedback module to support app-wide feedback ("App" target type). The system now allows users to provide feedback on:
- **Artifact** - feedback for specific museum artifacts/pieces
- **Chat** - feedback for chat conversations
- **App** - feedback for the entire application

## Changes Made

### 1. **Domain Layer**

#### Updated Enum: `FeedbackTargetType.cs`
```csharp
public enum FeedbackTargetType
{
    Artifact = 1,
    Chat = 2,
    App = 3        // NEW: System-wide feedback
}
```

#### Updated Entity: `Feedback.cs`
```csharp
public class Feedback
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public FeedbackTargetType TargetType { get; set; }
    public int? TargetId { get; set; }      // NOW NULLABLE: null for App feedback
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ApplicationUser? User { get; set; }
}
```

### 2. **Application Layer**

#### Updated DTO: `CreateFeedbackRequestDto.cs`
```csharp
public class CreateFeedbackRequestDto
{
    [Required(ErrorMessage = "Target type is required")]
    [RegularExpression("^(Artifact|Chat|App)$", 
        ErrorMessage = "Target type must be 'Artifact', 'Chat', or 'App'")]
    public string TargetType { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Target ID must be greater than 0 when required")]
    public int? TargetId { get; set; }      // NOW OPTIONAL

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(1000, MinimumLength = 1, 
        ErrorMessage = "Comment must be between 1 and 1000 characters")]
    public string? Comment { get; set; }    // NOW OPTIONAL
}
```

#### Updated DTO: `FeedbackDto.cs`
```csharp
public class FeedbackDto
{
    public int Id { get; set; }
    public string TargetType { get; set; } = null!;
    public int? TargetId { get; set; }      // NOW NULLABLE
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
```

#### Updated Service: `FeedbackService.cs`
- **Enhanced Validation Logic:**
  - If `targetType == "App"`: `targetId` MUST be null
  - If `targetType == "Artifact"` or `"Chat"`: `targetId` is REQUIRED
  - Rating must be 1-5
  - Comment is optional (can be empty)
  - For Artifact/Chat: validates that the target exists
  - For App: no target validation needed

#### Updated Interface: `IFeedbackService.cs`
```csharp
Task<List<FeedbackDto>> GetByTargetAsync(
    string userId, 
    string targetType, 
    int? targetId,          // NOW NULLABLE
    CancellationToken cancellationToken = default);
```

### 3. **Infrastructure Layer**

#### Updated Repository: `FeedbackRepository.cs`
```csharp
public async Task<List<Feedback>> GetByTargetAsync(
    FeedbackTargetType targetType, 
    int? targetId,          // NOW NULLABLE
    CancellationToken cancellationToken = default)
{
    var query = _context.Feedbacks.Where(f => f.TargetType == targetType);
    
    if (targetId.HasValue)
    {
        query = query.Where(f => f.TargetId == targetId);
    }
    
    return await query
        .OrderByDescending(f => f.CreatedAt)
        .ToListAsync(cancellationToken);
}
```

#### Updated Interface: `IFeedbackRepository.cs`
Signature updated to accept nullable `targetId`

### 4. **API Layer**

#### Updated Controller: `FeedbackController.cs`

**POST /api/Feedback** - Create Feedback
```
{
    "targetType": "Artifact|Chat|App",
    "targetId": 123,              // Optional for App, Required for others
    "rating": 5,
    "comment": "Great app!"       // Optional
}
```

**GET /api/Feedback** - Get User's All Feedback
```
Returns all feedback for the authenticated user
```

**GET /api/Feedback/target/{targetType}?targetId=123** - Get Target Feedback
```
Filters feedback by target type (with optional targetId for filtering)
- For App: GET /api/Feedback/target/App
- For Artifact: GET /api/Feedback/target/Artifact?targetId=5
- For Chat: GET /api/Feedback/target/Chat?targetId=10
```

**DELETE /api/Feedback/{id}** - Delete Feedback
```
User can only delete their own feedback
```

---

## Validation Rules

| Scenario | TargetType | TargetId | Rating | Comment | Valid? |
|----------|-----------|----------|--------|---------|--------|
| App feedback | App | null | 1-5 | any | ✅ |
| App feedback | App | 5 | 1-5 | any | ❌ (ID must be null) |
| Artifact feedback | Artifact | 1 | 1-5 | any | ✅ |
| Artifact feedback | Artifact | null | 1-5 | any | ❌ (ID required) |
| Chat feedback | Chat | 2 | 1-5 | any | ✅ |
| Invalid rating | Any | - | 6 | any | ❌ (1-5 only) |

---

## Example Requests

### 1. App-Wide Feedback (NEW)
```bash
POST /api/Feedback
Authorization: Bearer {token}
Content-Type: application/json

{
    "targetType": "App",
    "rating": 5,
    "comment": "Amazing museum experience!"
}
```

**Response (201 Created):**
```json
{
    "success": true,
    "message": "Feedback created successfully",
    "data": {
        "id": 1,
        "targetType": "App",
        "targetId": null,
        "rating": 5,
        "comment": "Amazing museum experience!",
        "createdAt": "2024-01-15T10:30:00Z"
    }
}
```

### 2. Artifact Feedback
```bash
POST /api/Feedback
Authorization: Bearer {token}
Content-Type: application/json

{
    "targetType": "Artifact",
    "targetId": 42,
    "rating": 4,
    "comment": "Beautiful piece!"
}
```

### 3. Chat Feedback
```bash
POST /api/Feedback
Authorization: Bearer {token}
Content-Type: application/json

{
    "targetType": "Chat",
    "targetId": 5,
    "rating": 3,
    "comment": "Helpful responses but slow"
}
```

### 4. Get All App Feedback
```bash
GET /api/Feedback/target/App
Authorization: Bearer {token}
```

**Response:**
```json
{
    "success": true,
    "message": "Target feedback retrieved successfully",
    "data": [
        {
            "id": 1,
            "targetType": "App",
            "targetId": null,
            "rating": 5,
            "comment": "Amazing!",
            "createdAt": "2024-01-15T10:30:00Z"
        }
    ]
}
```

### 5. Get User's Feedback by Artifact
```bash
GET /api/Feedback/target/Artifact?targetId=42
Authorization: Bearer {token}
```

### 6. Delete Feedback
```bash
DELETE /api/Feedback/1
Authorization: Bearer {token}
```

---

## Database Migration

**Required Migration:**
To make `TargetId` nullable, you need to create an EF Core migration:

```bash
Add-Migration MakeTargetIdNullable -Project EgyptianMuseum.Infrastructure
Update-Database -Project EgyptianMuseum.Infrastructure
```

The migration should include:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterColumn<int>(
        name: "TargetId",
        table: "Feedbacks",
        type: "int",
        nullable: true,     // Change from false to true
        oldClrType: typeof(int),
        oldType: "int",
        oldNullable: false);
}
```

---

## Best Practices Implemented

✅ **Clean Architecture Separation**
- Domain: Entity with enum
- Application: DTOs, Services, Interfaces
- Infrastructure: Repository implementation
- API: Controller handling HTTP

✅ **Validation Strategy**
- Request-level validation with data annotations
- Business logic validation in service layer
- Type-specific rules enforced

✅ **Error Handling**
- ArgumentException for validation failures
- KeyNotFoundException for missing targets
- UnauthorizedAccessException for auth issues
- Proper HTTP status codes (201, 400, 401, 403, 404)

✅ **Async/Await Pattern**
- All database operations are async
- Proper CancellationToken support
- No blocking calls

✅ **Security**
- JWT Bearer authentication required
- User-scoped operations (can only access own feedback)
- Ownership verification before delete

✅ **Production-Ready Code**
- Null coalescing operators
- Proper null checks
- Comprehensive error messages
- Consistent response format

---

## Optional Enhancement: Analytics

To track app feedback trends, you could add:

```csharp
public class FeedbackAnalyticsService
{
    public async Task<FeedbackStatsDto> GetAppFeedbackStatsAsync(CancellationToken cancellationToken)
    {
        var appFeedback = await _feedbackRepository.GetByTargetAsync(
            FeedbackTargetType.App, null, cancellationToken);
        
        return new FeedbackStatsDto
        {
            TotalFeedback = appFeedback.Count,
            AverageRating = appFeedback.Average(f => f.Rating),
            RatingDistribution = appFeedback
                .GroupBy(f => f.Rating)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
```

---

## Summary

✅ All changes follow Clean Architecture principles  
✅ Full validation logic implemented  
✅ Backward compatible with existing feedback types  
✅ Production-ready error handling  
✅ Async/await throughout  
✅ JWT authentication enforced  
✅ Build successful - no compilation errors  

**Ready for database migration and testing!**
