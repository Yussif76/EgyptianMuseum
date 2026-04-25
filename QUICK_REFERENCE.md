# Quick Reference - Setup & Testing Commands

## 🚀 Setup & Deployment

### Step 1: Generate EF Core Migration
```bash
# Option A: Package Manager Console (Visual Studio)
Add-Migration MakeTargetIdNullableAndAddAppFeedbackType -Project EgyptianMuseum.Infrastructure

# Option B: dotnet CLI
dotnet ef migrations add MakeTargetIdNullableAndAddAppFeedbackType --project EgyptianMuseum.Infrastructure
```

### Step 2: Apply Migration to Database
```bash
# Option A: Package Manager Console
Update-Database -Project EgyptianMuseum.Infrastructure

# Option B: dotnet CLI
dotnet ef database update --project EgyptianMuseum.Infrastructure
```

### Step 3: Verify Migration (Optional)
```bash
# Option A: Package Manager Console
Get-Migration -Project EgyptianMuseum.Infrastructure

# Option B: dotnet CLI
dotnet ef migrations list --project EgyptianMuseum.Infrastructure
```

---

## 🧪 Testing with cURL

### Prerequisites
```bash
# Set variables
$BASE_URL = "https://localhost:7000/api/Feedback"
$TOKEN = "YOUR_JWT_TOKEN_HERE"
$HEADERS = @{
    "Authorization" = "Bearer $TOKEN"
    "Content-Type" = "application/json"
}
```

### 1. Create App Feedback
```bash
curl -X POST $BASE_URL `
  -H "Authorization: Bearer $TOKEN" `
  -H "Content-Type: application/json" `
  -d '{
    "targetType": "App",
    "rating": 5,
    "comment": "Excellent experience!"
  }'
```

### 2. Create Artifact Feedback
```bash
curl -X POST $BASE_URL `
  -H "Authorization: Bearer $TOKEN" `
  -H "Content-Type: application/json" `
  -d '{
    "targetType": "Artifact",
    "targetId": 42,
    "rating": 4,
    "comment": "Beautiful artifact"
  }'
```

### 3. Create Chat Feedback
```bash
curl -X POST $BASE_URL `
  -H "Authorization: Bearer $TOKEN" `
  -H "Content-Type: application/json" `
  -d '{
    "targetType": "Chat",
    "targetId": 10,
    "rating": 3,
    "comment": "Good responses"
  }'
```

### 4. Get User's All Feedback
```bash
curl -X GET $BASE_URL `
  -H "Authorization: Bearer $TOKEN"
```

### 5. Get All App Feedback
```bash
curl -X GET "$BASE_URL/target/App" `
  -H "Authorization: Bearer $TOKEN"
```

### 6. Get Artifact Feedback
```bash
curl -X GET "$BASE_URL/target/Artifact?targetId=42" `
  -H "Authorization: Bearer $TOKEN"
```

### 7. Get Chat Feedback
```bash
curl -X GET "$BASE_URL/target/Chat?targetId=10" `
  -H "Authorization: Bearer $TOKEN"
```

### 8. Delete Feedback
```bash
curl -X DELETE "$BASE_URL/1" `
  -H "Authorization: Bearer $TOKEN"
```

---

## 📊 Testing with PowerShell

### Create App Feedback
```powershell
$response = Invoke-RestMethod `
  -Uri "https://localhost:7000/api/Feedback" `
  -Method Post `
  -Headers @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
  } `
  -Body @{
    targetType = "App"
    rating = 5
    comment = "Great app!"
  } | ConvertTo-Json

Write-Output $response | ConvertTo-Json -Depth 10
```

### Get App Feedback
```powershell
$response = Invoke-RestMethod `
  -Uri "https://localhost:7000/api/Feedback/target/App" `
  -Method Get `
  -Headers @{
    "Authorization" = "Bearer $token"
  }

$response.data | ForEach-Object {
    Write-Output "ID: $($_.id) | Rating: $($_.rating) | Comment: $($_.comment)"
}
```

---

## 🔍 Database Queries

### View All Feedback
```sql
SELECT * FROM Feedbacks ORDER BY CreatedAt DESC;
```

### View App Feedback Only
```sql
SELECT * FROM Feedbacks WHERE TargetType = 3 ORDER BY CreatedAt DESC;
```

### View Artifact Feedback
```sql
SELECT * FROM Feedbacks WHERE TargetType = 1 ORDER BY CreatedAt DESC;
```

### View Chat Feedback
```sql
SELECT * FROM Feedbacks WHERE TargetType = 2 ORDER BY CreatedAt DESC;
```

### Feedback Statistics
```sql
SELECT 
    CASE TargetType 
        WHEN 1 THEN 'Artifact'
        WHEN 2 THEN 'Chat'
        WHEN 3 THEN 'App'
    END as TargetType,
    COUNT(*) as TotalFeedback,
    AVG(CAST(Rating AS FLOAT)) as AvgRating,
    MIN(Rating) as MinRating,
    MAX(Rating) as MaxRating
FROM Feedbacks
GROUP BY TargetType;
```

### User Feedback Count
```sql
SELECT 
    UserId,
    COUNT(*) as FeedbackCount,
    AVG(CAST(Rating AS FLOAT)) as AvgRating
FROM Feedbacks
GROUP BY UserId
ORDER BY FeedbackCount DESC;
```

### App Feedback Analytics
```sql
SELECT 
    CAST(Rating AS VARCHAR) as Rating,
    COUNT(*) as Count,
    ROUND(CAST(COUNT(*) AS FLOAT) / (SELECT COUNT(*) FROM Feedbacks WHERE TargetType = 3) * 100, 2) as Percentage
FROM Feedbacks
WHERE TargetType = 3
GROUP BY Rating
ORDER BY Rating;
```

### Find Feedback with Null TargetId
```sql
SELECT * FROM Feedbacks WHERE TargetId IS NULL;
```

---

## 🧬 Unit Testing (xUnit)

### Run All Tests
```bash
# Package Manager Console
Test-Project EgyptianMuseum.Application.UnitTests

# Or dotnet CLI
dotnet test EgyptianMuseum.Application.UnitTests
```

### Run Specific Test Class
```bash
dotnet test EgyptianMuseum.Application.UnitTests --filter FullyQualifiedName~FeedbackServiceTests
```

### Run Specific Test Method
```bash
dotnet test EgyptianMuseum.Application.UnitTests --filter FullyQualifiedName~FeedbackServiceTests.CreateAsync_AppFeedbackWithValidData_ReturnsSuccess
```

### Test with Coverage
```bash
dotnet test EgyptianMuseum.Application.UnitTests /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

---

## 🔧 Validation Testing

### Valid Requests
```bash
# ✅ Valid: App Feedback (no TargetId needed)
POST /api/Feedback
{ "targetType": "App", "rating": 5 }

# ✅ Valid: Artifact with TargetId
POST /api/Feedback
{ "targetType": "Artifact", "targetId": 1, "rating": 4 }

# ✅ Valid: Chat with TargetId
POST /api/Feedback
{ "targetType": "Chat", "targetId": 2, "rating": 3 }
```

### Invalid Requests
```bash
# ❌ Invalid: App with TargetId
POST /api/Feedback
{ "targetType": "App", "targetId": 1, "rating": 5 }
Response: "Target ID must be null for App feedback"

# ❌ Invalid: Artifact without TargetId
POST /api/Feedback
{ "targetType": "Artifact", "rating": 4 }
Response: "Target ID is required for Artifact feedback"

# ❌ Invalid: Rating out of range
POST /api/Feedback
{ "targetType": "App", "rating": 10 }
Response: "Rating must be between 1 and 5"

# ❌ Invalid: Wrong TargetType
POST /api/Feedback
{ "targetType": "Invalid", "rating": 5 }
Response: "Target type must be 'Artifact', 'Chat', or 'App'"
```

---

## 📈 Performance Testing

### Load Test (using Apache JMeter or similar)
```
Test Plan:
- Concurrent Users: 10
- Ramp-up: 1 second
- Duration: 60 seconds
- Endpoint: GET /api/Feedback/target/App

Expected:
- Avg Response Time: < 200ms
- P95 Response Time: < 500ms
- Success Rate: 100%
```

### Query Performance Check
```sql
-- Check execution time
SET STATISTICS TIME ON
SELECT * FROM Feedbacks WHERE TargetType = 3 ORDER BY CreatedAt DESC
SET STATISTICS TIME OFF

-- Check query plan
SET STATISTICS IO ON
SELECT * FROM Feedbacks WHERE TargetType = 3 ORDER BY CreatedAt DESC
SET STATISTICS IO OFF
```

---

## 🐛 Troubleshooting

### Migration Won't Apply
```bash
# Reset migrations (CAUTION: Data loss)
Update-Database -Migration 0 -Project EgyptianMuseum.Infrastructure
Remove-Migration -Project EgyptianMuseum.Infrastructure

# Re-create
Add-Migration MakeTargetIdNullableAndAddAppFeedbackType -Project EgyptianMuseum.Infrastructure
Update-Database -Project EgyptianMuseum.Infrastructure
```

### Database Out of Sync
```bash
# Check current schema
Get-Migration -Project EgyptianMuseum.Infrastructure

# Rebuild from migrations
Update-Database -Migration 0 -Project EgyptianMuseum.Infrastructure
Update-Database -Project EgyptianMuseum.Infrastructure
```

### Build Errors
```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

---

## 📋 Validation Checklist

After implementing, verify:

- [ ] Build successful (no errors)
- [ ] Migration created successfully
- [ ] Migration applied to database
- [ ] App feedback endpoint works (201 response)
- [ ] Artifact feedback endpoint works (201 response)
- [ ] Chat feedback endpoint works (201 response)
- [ ] TargetId validation enforced
- [ ] Rating validation enforced (1-5)
- [ ] Get endpoints return correct data
- [ ] Delete endpoint works (ownership verified)
- [ ] Error responses return proper HTTP codes
- [ ] JWT authentication enforced
- [ ] User-scoped data access works
- [ ] Database nullable TargetId verified
- [ ] All unit tests pass

---

## 📚 Documentation Reference

- **IMPLEMENTATION_SUMMARY.md** - Overview of all changes
- **FEEDBACK_IMPLEMENTATION_GUIDE.md** - Detailed implementation guide
- **TESTING_GUIDE.md** - Comprehensive testing scenarios
- **ARCHITECTURE_DIAGRAMS.md** - System design and flows
- **QUICK_REFERENCE.md** - This file

---

## 🎯 Success Criteria

✅ Build successful  
✅ Database migration applied  
✅ All endpoints respond correctly  
✅ Validation rules enforced  
✅ Error handling working  
✅ Security implemented  
✅ Tests passing  
✅ Documentation complete  

---

**Last Updated:** January 2025  
**Status:** Ready for Deployment
