using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Rooms
                .Include(r => r.Map)
                .Include(r => r.Translations)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
        }

        public async Task<List<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Rooms
                .Include(r => r.Map)
                .Include(r => r.Translations)
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Room>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default)
        {
            return await _context.Rooms
                .Include(r => r.Map)
                .Include(r => r.Translations)
                .Where(r => r.MapId == mapId && !r.IsDeleted)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Room> CreateAsync(Room room, CancellationToken cancellationToken = default)
        {
            await _context.Rooms.AddAsync(room, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return room;
        }

        public async Task<bool> UpdateAsync(Room room, CancellationToken cancellationToken = default)
        {
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == room.Id && !r.IsDeleted, cancellationToken);

            if (existingRoom == null)
                return false;

            existingRoom.Name = room.Name;
            existingRoom.Description = room.Description;
            existingRoom.MapId = room.MapId;
            existingRoom.XCoord = room.XCoord;
            existingRoom.YCoord = room.YCoord;

            _context.Rooms.Update(existingRoom);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

            if (room == null)
                return false;

            room.IsDeleted = true;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RoomExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Rooms
                .AnyAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
        }
    }
}
