# Tours Module - Quick Command Reference

## 🚀 DEPLOYMENT COMMANDS

### Step 1: Create Migration
```powershell
# In Package Manager Console (PMC)
Add-Migration AddToursModule
```

**Expected Output:**
```
Build started...
Build succeeded.
To undo this action, use Remove-Migration.
```

### Step 2: Update Database
```powershell
Update-Database
```

**Expected Output:**
```
Applying migration '20250507_AddToursModule'.
Done.
```

### Step 3: Build Solution
```powershell
# Option 1: Using dotnet CLI
dotnet build

# Option 2: Visual Studio (Ctrl+Shift+B)
```

**Expected Output:**
```
Build succeeded.
```

### Step 4: Run Application
```powershell
# Using dotnet CLI
dotnet run

# Or press F5 in Visual Studio
```

**Expected Output:**
```
Application started. Press Ctrl+C to shut down.
```

---

## 🧪 TESTING COMMANDS

### Using cURL

#### Get All Tours
```bash
curl -X GET "https://localhost:7xxx/api/tours"
```

#### Create Tour
```bash
curl -X POST "https://localhost:7xxx/api/tours" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Test Tour\",\"description\":\"Test\",\"durationMinutes\":120}"
```

#### Get Tour by ID
```bash
curl -X GET "https://localhost:7xxx/api/tours/1"
```

#### Update Tour
```bash
curl -X PUT "https://localhost:7xxx/api/tours/1" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Updated Tour\",\"description\":\"Updated\",\"durationMinutes\":150}"
```

#### Add Room to Tour
```bash
curl -X POST "https://localhost:7xxx/api/tours/1/rooms" \
  -H "Content-Type: application/json" \
  -d "{\"roomId\":1,\"order\":1}"
```

#### Get Tour Rooms
```bash
curl -X GET "https://localhost:7xxx/api/tours/1/rooms"
```

#### Get Tour Details
```bash
curl -X GET "https://localhost:7xxx/api/tours/1/details"
```

#### Delete Room from Tour
```bash
curl -X DELETE "https://localhost:7xxx/api/tours/1/rooms/1"
```

#### Delete Tour
```bash
curl -X DELETE "https://localhost:7xxx/api/tours/1"
```

---

## 📊 DATABASE COMMANDS

### SQL Server Management Studio (SSMS)

#### Check Tours Table
```sql
SELECT * FROM Tours;
```

#### Check TourRooms Table
```sql
SELECT * FROM TourRooms ORDER BY TourId, [Order];
```

#### Count Tours
```sql
SELECT COUNT(*) as TourCount FROM Tours WHERE IsDeleted = 0;
```

#### Count Tour Rooms
```sql
SELECT COUNT(*) as RoomCount FROM TourRooms;
```

#### View Tour with Rooms
```sql
SELECT 
    t.Id,
    t.Name,
    t.Description,
    t.DurationMinutes,
    tr.RoomId,
    r.Name as RoomName,
    tr.[Order]
FROM Tours t
LEFT JOIN TourRooms tr ON t.Id = tr.TourId
LEFT JOIN Rooms r ON tr.RoomId = r.Id
ORDER BY t.Id, tr.[Order];
```

---

## 🔧 TROUBLESHOOTING COMMANDS

### Reset Database (Danger - Deletes all data)
```powershell
# Remove last migration
Remove-Migration

# Update database to previous state
Update-Database -Migration NameOfPreviousMigration

# Alternative: Drop and recreate
Drop-Database
Update-Database
```

### Check Entity Framework Migrations
```powershell
# List all migrations
Get-Migrations

# List pending migrations
Get-Migrations -MigrationsPath EgyptianMuseum.Infrastructure/Migrations
```

### Clean Build
```powershell
# Clean solution
dotnet clean

# Build
dotnet build
```

### View Application Logs
```powershell
# If using file logging
Get-Content ".\logs\*.log" -Tail 100
```

---

## 📍 KEY URLS

### Local Development
```
API Base:        https://localhost:7xxx/api
Swagger Docs:    https://localhost:7xxx/swagger
Tours Endpoint:  https://localhost:7xxx/api/tours
```

### Connection String (appsettings.json)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EgyptianMuseumDb;Trusted_Connection=true;"
}
```

---

## 📦 PROJECT STRUCTURE

```
EgyptianMuseum.Domain/
  └─ Entities/Tour.cs, TourRoom.cs

EgyptianMuseum.Application/
  ├─ Interfaces/ITourService.cs, ITourRepository.cs, ITourRoomRepository.cs
  ├─ DTOs/Tours/*.cs (6 files)
  └─ Services/Tours/TourService.cs

EgyptianMuseum.Infrastructure/
  ├─ Data/AppDbContext.cs (Updated)
  └─ Repositories/TourRepository.cs, TourRoomRepository.cs

EgyptianMuseum.API/
  ├─ Controllers/ToursController.cs
  └─ Program.cs (Updated)
```

---

## ✅ VERIFICATION CHECKLIST

### After Migration
- [ ] Tour table exists
- [ ] TourRooms table exists
- [ ] Foreign keys created
- [ ] Indexes created
- [ ] No errors in SQL

### After Build
- [ ] 0 compilation errors
- [ ] 0 warnings
- [ ] All projects build
- [ ] No dependency issues

### After Running
- [ ] Application starts
- [ ] Swagger loads
- [ ] Endpoints accessible
- [ ] No runtime errors

### After Testing
- [ ] Create tour works
- [ ] Get tours works
- [ ] Add room works
- [ ] Validation works
- [ ] Error handling works

---

## 🎯 COMMON ISSUES & FIXES

### Issue: Migration Not Found
```powershell
# Solution: Ensure you're in correct directory
cd EgyptianMuseum.Infrastructure
Add-Migration AddToursModule
```

### Issue: Database Not Updated
```powershell
# Solution: Check pending migrations
Get-Migrations

# Apply all pending
Update-Database
```

### Issue: Service Not Registered
```csharp
// Solution: Check Program.cs has this line
builder.Services.AddScoped<ITourService, TourService>();
```

### Issue: 404 Not Found on Endpoint
```powershell
# Solution: Check:
# 1. Application is running
# 2. Correct port number
# 3. Correct route in controller
# 4. Controller is added to MapControllers()
```

---

## 📞 USEFUL LINKS

### Documentation Files
- TOURS_MODULE_TESTING_GUIDE.md - 15 test cases
- TOURS_MODULE_IMPLEMENTATION.md - Technical details
- TOURS_MODULE_QUICK_REFERENCE.md - API reference

### Migration Info
```powershell
# Get help on specific command
Get-Help Add-Migration -Full
Get-Help Update-Database -Full
```

---

## ⚡ QUICK START (Summary)

```powershell
# 1. Apply migration
Add-Migration AddToursModule
Update-Database

# 2. Build
dotnet build

# 3. Run
dotnet run

# 4. Open Swagger
# Navigate to: https://localhost:7xxx/swagger

# 5. Test endpoints
# Use Swagger UI to test all 9 endpoints
```

---

## ✨ ALL ENDPOINTS AT A GLANCE

```
GET    /api/tours                          - List all
GET    /api/tours/{id}                     - Get one
POST   /api/tours                          - Create
PUT    /api/tours/{id}                     - Update
DELETE /api/tours/{id}                     - Delete

POST   /api/tours/{tourId}/rooms           - Add room
GET    /api/tours/{tourId}/rooms           - List rooms
GET    /api/tours/{tourId}/details         - Get with rooms
DELETE /api/tours/{tourId}/rooms/{roomId}  - Remove room
```

---

**Ready to deploy? Run the commands above!** 🚀
