# Feedback System - Testing Guide

## Postman Collection / cURL Examples

### Setup
- **Base URL:** `https://localhost:7000/api/Feedback`
- **Authentication:** Bearer token in Authorization header
- **Content-Type:** application/json

---

## Test Cases

### 1. App-Wide Feedback - Valid Request ✅

**cURL:**
```bash
curl --request POST \
  --url https://localhost:7000/api/Feedback \
  --header 'Authorization: Bearer YOUR_TOKEN' \
  --header 'Content-Type: application/json' \
  --data '{
    "targetType": "App",
    "rating": 5,
    "comment": "Excellent museum experience!"
  }'
```

**Expected Response (201):**
```json
{
    "success": true,
    "message": "Feedback created successfully",
    "data": {
        "id": 1,
        "targetType": "App",
        "targetId": null,
        "rating": 5,
        "comment": "Excellent museum experience!",
        "createdAt": "2024-01-15T10:30:00Z"
    }
}
```

---

### 2. App Feedback with TargetId - Invalid ❌

**Request:**
```json
{
    "targetType": "App",
    "targetId": 5,
    "rating": 4,
    "comment": "Should fail"
}
```

**Expected Response (400):**
```json
{
    "success": false,
    "message": "Target ID must be null for App feedback"
}
```

---

### 3. Artifact Feedback - Valid ✅

**Request:**
```json
{
    "targetType": "Artifact",
    "targetId": 42,
    "rating": 5,
    "comment": "Beautiful artifact"
}
```

**Expected Response (201):**
```json
{
    "success": true,
    "message": "Feedback created successfully",
    "data": {
        "id": 2,
        "targetType": "Artifact",
        "targetId": 42,
        "rating": 5,
        "comment": "Beautiful artifact",
        "createdAt": "2024-01-15T10:35:00Z"
    }
}
```

---

### 4. Artifact Feedback Without TargetId - Invalid ❌

**Request:**
```json
{
    "targetType": "Artifact",
    "rating": 4,
    "comment": "Missing target ID"
}
```

**Expected Response (400):**
```json
{
    "success": false,
    "message": "Target ID is required for Artifact feedback"
}
```

---

### 5. Chat Feedback - Valid ✅

**Request:**
```json
{
    "targetType": "Chat",
    "targetId": 10,
    "rating": 3,
    "comment": "Good responses"
}
```

**Expected Response (201):**
```json
{
    "success": true,
    "message": "Feedback created successfully",
    "data": {
        "id": 3,
        "targetType": "Chat",
        "targetId": 10,
        "rating": 3,
        "comment": "Good responses",
        "createdAt": "2024-01-15T10:40:00Z"
    }
}
```

---

### 6. Invalid Rating Value - Invalid ❌

**Request:**
```json
{
    "targetType": "App",
    "rating": 10,
    "comment": "Out of range"
}
```

**Expected Response (400):**
```json
{
    "success": false,
    "message": "Rating must be between 1 and 5"
}
```

---

### 7. Invalid Target Type - Invalid ❌

**Request:**
```json
{
    "targetType": "InvalidType",
    "targetId": 1,
    "rating": 5,
    "comment": "Wrong type"
}
```

**Expected Response (400):**
```json
{
    "success": false,
    "message": "Target type must be 'Artifact', 'Chat', or 'App'"
}
```

---

### 8. Non-Existent Artifact - Invalid ❌

**Request:**
```json
{
    "targetType": "Artifact",
    "targetId": 99999,
    "rating": 5,
    "comment": "Should not exist"
}
```

**Expected Response (404):**
```json
{
    "success": false,
    "message": "Artifact with ID 99999 not found"
}
```

---

### 9. Get User's Feedback - Valid ✅

**cURL:**
```bash
curl --request GET \
  --url https://localhost:7000/api/Feedback \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (200):**
```json
{
    "success": true,
    "message": "User feedback retrieved successfully",
    "data": [
        {
            "id": 1,
            "targetType": "App",
            "targetId": null,
            "rating": 5,
            "comment": "Excellent museum experience!",
            "createdAt": "2024-01-15T10:30:00Z"
        },
        {
            "id": 2,
            "targetType": "Artifact",
            "targetId": 42,
            "rating": 5,
            "comment": "Beautiful artifact",
            "createdAt": "2024-01-15T10:35:00Z"
        }
    ]
}
```

---

### 10. Get All App Feedback - Valid ✅

**cURL:**
```bash
curl --request GET \
  --url 'https://localhost:7000/api/Feedback/target/App' \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (200):**
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
            "comment": "Excellent museum experience!",
            "createdAt": "2024-01-15T10:30:00Z"
        }
    ]
}
```

---

### 11. Get Artifact Feedback - Valid ✅

**cURL:**
```bash
curl --request GET \
  --url 'https://localhost:7000/api/Feedback/target/Artifact?targetId=42' \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (200):**
```json
{
    "success": true,
    "message": "Target feedback retrieved successfully",
    "data": [
        {
            "id": 2,
            "targetType": "Artifact",
            "targetId": 42,
            "rating": 5,
            "comment": "Beautiful artifact",
            "createdAt": "2024-01-15T10:35:00Z"
        }
    ]
}
```

---

### 12. Get Artifact Feedback Without ID - Invalid ❌

**cURL:**
```bash
curl --request GET \
  --url 'https://localhost:7000/api/Feedback/target/Artifact' \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (400):**
```json
{
    "success": false,
    "message": "Target ID must be provided and greater than 0 for Artifact and Chat feedback"
}
```

---

### 13. Delete Own Feedback - Valid ✅

**cURL:**
```bash
curl --request DELETE \
  --url 'https://localhost:7000/api/Feedback/1' \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (204 No Content):**
```
(No body)
```

---

### 14. Delete Non-Existent Feedback - Invalid ❌

**cURL:**
```bash
curl --request DELETE \
  --url 'https://localhost:7000/api/Feedback/99999' \
  --header 'Authorization: Bearer YOUR_TOKEN'
```

**Expected Response (404):**
```json
{
    "success": false,
    "message": "Feedback not found"
}
```

---

### 15. Unauthorized Access - No Token ❌

**cURL (without Authorization header):**
```bash
curl --request GET \
  --url 'https://localhost:7000/api/Feedback'
```

**Expected Response (401):**
```json
{
    "success": false,
    "message": "User not authenticated"
}
```

---

## Edge Cases to Test

### 1. Empty Comment (Should be allowed)
```json
{
    "targetType": "App",
    "rating": 5,
    "comment": ""
}
```
✅ Should succeed

### 2. Very Long Comment
```json
{
    "targetType": "App",
    "rating": 5,
    "comment": "... [1001 characters] ..."
}
```
❌ Should fail with "Comment must be between 1 and 1000 characters"

### 3. Whitespace-Only Comment
```json
{
    "targetType": "App",
    "rating": 5,
    "comment": "   "
}
```
❌ Should fail with "Comment cannot be empty"

### 4. Zero TargetId
```json
{
    "targetType": "Artifact",
    "targetId": 0,
    "rating": 5,
    "comment": "Invalid"
}
```
❌ Should fail with "Target ID must be greater than 0"

### 5. Negative Rating
```json
{
    "targetType": "App",
    "rating": -1,
    "comment": "Negative"
}
```
❌ Should fail with "Rating must be between 1 and 5"

---

## Unit Test Examples (xUnit)

```csharp
public class FeedbackServiceTests
{
    private readonly FeedbackService _service;
    private readonly Mock<IFeedbackRepository> _mockRepository;
    private readonly Mock<IPieceRepository> _mockPieceRepository;
    private readonly Mock<IChatConversationRepository> _mockChatRepository;

    public FeedbackServiceTests()
    {
        _mockRepository = new Mock<IFeedbackRepository>();
        _mockPieceRepository = new Mock<IPieceRepository>();
        _mockChatRepository = new Mock<IChatConversationRepository>();
        
        _service = new FeedbackService(
            _mockRepository.Object,
            _mockPieceRepository.Object,
            _mockChatRepository.Object
        );
    }

    [Fact]
    public async Task CreateAsync_AppFeedbackWithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateFeedbackRequestDto
        {
            TargetType = "App",
            TargetId = null,
            Rating = 5,
            Comment = "Great app!"
        };

        // Act
        var result = await _service.CreateAsync("user123", request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("App", result.TargetType);
        Assert.Null(result.TargetId);
        Assert.Equal(5, result.Rating);
    }

    [Fact]
    public async Task CreateAsync_AppFeedbackWithTargetId_ThrowsException()
    {
        // Arrange
        var request = new CreateFeedbackRequestDto
        {
            TargetType = "App",
            TargetId = 5,
            Rating = 5,
            Comment = "Invalid"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync("user123", request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAsync_ArtifactFeedbackWithoutTargetId_ThrowsException()
    {
        // Arrange
        var request = new CreateFeedbackRequestDto
        {
            TargetType = "Artifact",
            TargetId = null,
            Rating = 4,
            Comment = "Missing ID"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync("user123", request, CancellationToken.None)
        );
    }
}
```

---

## Database Verification Queries

```sql
-- View all feedback
SELECT * FROM Feedbacks ORDER BY CreatedAt DESC;

-- View app feedback only
SELECT * FROM Feedbacks WHERE TargetType = 3 ORDER BY CreatedAt DESC;

-- View feedback stats
SELECT 
    TargetType,
    COUNT(*) as TotalFeedback,
    AVG(CAST(Rating AS FLOAT)) as AvgRating,
    MIN(Rating) as MinRating,
    MAX(Rating) as MaxRating
FROM Feedbacks
GROUP BY TargetType;

-- View user's feedback
SELECT * FROM Feedbacks WHERE UserId = 'user123' ORDER BY CreatedAt DESC;

-- Check nullable TargetId
SELECT 
    Id,
    TargetType,
    TargetId,
    CASE WHEN TargetId IS NULL THEN 'NULL' ELSE CAST(TargetId AS VARCHAR) END as TargetIdValue
FROM Feedbacks;
```

---

## Checklist Before Deployment

- [ ] Database migration applied (`Update-Database`)
- [ ] All unit tests passing
- [ ] Integration tests passing
- [ ] JWT authentication verified
- [ ] Error responses are consistent
- [ ] No compilation warnings
- [ ] Postman collection tested all endpoints
- [ ] Edge cases handled properly
- [ ] Documentation updated
- [ ] Code review completed
