# Tours Module - Testing Guide

## Pre-Testing Setup

### 1. Apply Database Migration

Open **Package Manager Console** in Visual Studio:

```powershell
# Set default project to EgyptianMuseum.Infrastructure
Add-Migration AddToursModule
Update-Database
```

Expected output:
```
Build started...
Build succeeded.
Migration started...
Migration completed.
Database updated.
```

### 2. Build Solution

```powershell
dotnet build
# or Ctrl+Shift+B in Visual Studio
```

Expected: Build successful (0 errors)

### 3. Run Application

```powershell
dotnet run
# or F5 in Visual Studio
```

Application should start successfully.

---

## Testing the API

### Option 1: Using Swagger UI (Recommended)

1. Navigate to: `https://localhost:7xxx/swagger`
2. Find "Tours" section
3. Expand and test each endpoint

### Option 2: Using Postman

Import the following requests.

### Option 3: Using curl

Use the curl commands provided below.

---

## Test Cases

### TEST 1: Create a Tour

**Endpoint:** POST `/api/tours`

**Request:**
```json
{
  "name": "Pharaoh's Journey",
  "description": "Explore the life and legacy of ancient Egyptian pharaohs",
  "durationMinutes": 120
}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Pharaoh's Journey",
    "description": "Explore the life and legacy of ancient Egyptian pharaohs",
    "durationMinutes": 120
  }
}
```

**Status Code:** 201 Created

**curl:**
```bash
curl -X POST "https://localhost:7xxx/api/tours" \
  -H "Content-Type: application/json" \
  -d '{"name":"Pharaoh'\''s Journey","description":"Explore the life and legacy of ancient Egyptian pharaohs","durationMinutes":120}'
```

---

### TEST 2: Get All Tours

**Endpoint:** GET `/api/tours`

**Expected Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Pharaoh's Journey",
      "description": "Explore the life and legacy of ancient Egyptian pharaohs",
      "durationMinutes": 120
    }
  ],
  "count": 1
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X GET "https://localhost:7xxx/api/tours"
```

---

### TEST 3: Get Tour by ID

**Endpoint:** GET `/api/tours/1`

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Pharaoh's Journey",
    "description": "Explore the life and legacy of ancient Egyptian pharaohs",
    "durationMinutes": 120
  }
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X GET "https://localhost:7xxx/api/tours/1"
```

---

### TEST 4: Update Tour

**Endpoint:** PUT `/api/tours/1`

**Request:**
```json
{
  "name": "Pharaoh's Journey - Extended",
  "description": "Extended tour exploring the life and legacy of ancient Egyptian pharaohs",
  "durationMinutes": 150
}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Pharaoh's Journey - Extended",
    "description": "Extended tour exploring the life and legacy of ancient Egyptian pharaohs",
    "durationMinutes": 150
  }
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X PUT "https://localhost:7xxx/api/tours/1" \
  -H "Content-Type: application/json" \
  -d '{"name":"Pharaoh'\''s Journey - Extended","description":"Extended tour exploring the life and legacy of ancient Egyptian pharaohs","durationMinutes":150}'
```

---

### TEST 5: Add Room to Tour

**Prerequisites:**
- Tour ID: 1 (from TEST 1)
- Room ID: 1 (must exist in database)

**Endpoint:** POST `/api/tours/1/rooms`

**Request:**
```json
{
  "roomId": 1,
  "order": 1
}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "tourId": 1,
    "roomId": 1,
    "roomName": "Entrance Hall",
    "order": 1
  }
}
```

**Status Code:** 201 Created

**curl:**
```bash
curl -X POST "https://localhost:7xxx/api/tours/1/rooms" \
  -H "Content-Type: application/json" \
  -d '{"roomId":1,"order":1}'
```

---

### TEST 6: Add Second Room to Tour

**Endpoint:** POST `/api/tours/1/rooms`

**Request:**
```json
{
  "roomId": 2,
  "order": 2
}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "tourId": 1,
    "roomId": 2,
    "roomName": "Kings Chamber",
    "order": 2
  }
}
```

**Status Code:** 201 Created

---

### TEST 7: Get Tour Rooms

**Endpoint:** GET `/api/tours/1/rooms`

**Expected Response:**
```json
{
  "success": true,
  "data": [
    {
      "tourId": 1,
      "roomId": 1,
      "roomName": "Entrance Hall",
      "order": 1
    },
    {
      "tourId": 1,
      "roomId": 2,
      "roomName": "Kings Chamber",
      "order": 2
    }
  ],
  "count": 2
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X GET "https://localhost:7xxx/api/tours/1/rooms"
```

---

### TEST 8: Get Tour Details

**Endpoint:** GET `/api/tours/1/details`

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Pharaoh's Journey - Extended",
    "description": "Extended tour exploring the life and legacy of ancient Egyptian pharaohs",
    "durationMinutes": 150,
    "rooms": [
      {
        "tourId": 1,
        "roomId": 1,
        "roomName": "Entrance Hall",
        "order": 1
      },
      {
        "tourId": 1,
        "roomId": 2,
        "roomName": "Kings Chamber",
        "order": 2
      }
    ]
  }
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X GET "https://localhost:7xxx/api/tours/1/details"
```

---

### TEST 9: Remove Room from Tour

**Endpoint:** DELETE `/api/tours/1/rooms/1`

**Expected Response:**
```json
{
  "success": true,
  "message": "Room removed from tour successfully"
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X DELETE "https://localhost:7xxx/api/tours/1/rooms/1"
```

---

### TEST 10: Delete Tour

**Endpoint:** DELETE `/api/tours/1`

**Expected Response:**
```json
{
  "success": true,
  "message": "Tour deleted successfully"
}
```

**Status Code:** 200 OK

**curl:**
```bash
curl -X DELETE "https://localhost:7xxx/api/tours/1"
```

---

## Validation Tests

### TEST 11: Missing Required Field

**Endpoint:** POST `/api/tours`

**Request (Invalid - missing description):**
```json
{
  "name": "Invalid Tour",
  "durationMinutes": 120
}
```

**Expected Response:**
```json
{
  "success": false,
  "message": "Tour description is required"
}
```

**Status Code:** 400 Bad Request

---

### TEST 12: Invalid Duration

**Endpoint:** POST `/api/tours`

**Request (Invalid - duration <= 0):**
```json
{
  "name": "Invalid Tour",
  "description": "This tour has invalid duration",
  "durationMinutes": 0
}
```

**Expected Response:**
```json
{
  "success": false,
  "message": "Duration must be greater than 0"
}
```

**Status Code:** 400 Bad Request

---

### TEST 13: Tour Not Found

**Endpoint:** GET `/api/tours/99999`

**Expected Response:**
```json
{
  "success": false,
  "message": "Tour with ID 99999 not found"
}
```

**Status Code:** 404 Not Found

---

### TEST 14: Duplicate Room in Tour

**Prerequisites:**
- Tour ID: 1
- Room ID: 1 already added

**Endpoint:** POST `/api/tours/1/rooms`

**Request:**
```json
{
  "roomId": 1,
  "order": 3
}
```

**Expected Response:**
```json
{
  "success": false,
  "message": "Room with ID 1 is already in this tour"
}
```

**Status Code:** 409 Conflict

---

### TEST 15: Invalid Room ID

**Endpoint:** POST `/api/tours/1/rooms`

**Request:**
```json
{
  "roomId": 99999,
  "order": 1
}
```

**Expected Response:**
```json
{
  "success": false,
  "message": "Room with ID 99999 not found"
}
```

**Status Code:** 404 Not Found

---

## Testing Checklist

### Basic CRUD Operations
- [ ] Create tour (TEST 1)
- [ ] Get all tours (TEST 2)
- [ ] Get tour by ID (TEST 3)
- [ ] Update tour (TEST 4)
- [ ] Delete tour (TEST 10)

### Tour-Room Management
- [ ] Add room to tour (TEST 5)
- [ ] Add multiple rooms (TEST 6)
- [ ] Get tour rooms (TEST 7)
- [ ] Get tour details with rooms (TEST 8)
- [ ] Remove room from tour (TEST 9)

### Validation
- [ ] Missing required fields (TEST 11)
- [ ] Invalid duration (TEST 12)
- [ ] Tour not found (TEST 13)
- [ ] Duplicate room prevention (TEST 14)
- [ ] Invalid room ID (TEST 15)

### Database Verification
- [ ] Verify Tours table created
- [ ] Verify TourRooms table created
- [ ] Verify data persists after delete (soft delete)
- [ ] Verify relationships work correctly
- [ ] Verify indexes exist

### Performance
- [ ] Check query performance
- [ ] Verify indexes used
- [ ] Check N+1 query issues
- [ ] Monitor response times

---

## Debugging Tips

If tests fail:

1. **Check database connection**
   ```powershell
   # Verify connection string in appsettings.json
   ```

2. **Check migration applied**
   ```powershell
   # In Package Manager Console
   Get-Migrations
   ```

3. **Check service registration**
   - Verify Program.cs has all registrations
   - Check dependency injection is working

4. **Check logs**
   - Enable logging in appsettings.json
   - Check Visual Studio Output window
   - Check Application Insights

5. **Verify data exists**
   - Check rooms exist in database
   - Verify tour creation actually saved data
   - Check soft delete flag

---

## Load Testing (Optional)

If performance testing needed:

```csharp
// Example: Create 100 tours
for (int i = 0; i < 100; i++)
{
    POST /api/tours with unique names
}

// Example: Get all tours with pagination needed
GET /api/tours?page=1&pageSize=50
```

---

## Integration Testing

After basic tests pass:

1. Test with frontend application
2. Test with mobile app (if applicable)
3. Test concurrent requests
4. Test with multiple users
5. Test error scenarios

---

## Sign-Off

- [ ] All tests passed
- [ ] No errors in console
- [ ] No warnings in logs
- [ ] Database integrity verified
- [ ] Ready for production

---

**Testing Status:** Ready
**Last Updated:** 2025
