# Tours Module - Migration Commands

## Create Migration

Run this command in **Package Manager Console** to create the migration:

```powershell
Add-Migration AddToursModule
```

## Apply Migration

Run this command to update the database:

```powershell
Update-Database
```

## Migration Details

This migration will:

1. Create `Tours` table with columns:
   - `Id` (int, PK)
   - `Name` (nvarchar(255))
   - `Description` (nvarchar(1000))
   - `DurationMinutes` (int)
   - `IsDeleted` (bit)
   - `CreatedAt` (datetime2, nullable)
   - `UpdatedAt` (datetime2, nullable)

2. Create `TourRooms` join table with columns:
   - `TourId` (int, FK)
   - `RoomId` (int, FK)
   - `Order` (int)
   - Composite key on (TourId, RoomId)
   - Index on (TourId, Order)

3. Configure relationships:
   - Tour → TourRooms (cascade delete)
   - Room → TourRooms (restrict delete)

## Rollback Migration

If needed, run this command to revert the migration:

```powershell
Remove-Migration
```

Then rerun `Update-Database` to apply the previous migration.
