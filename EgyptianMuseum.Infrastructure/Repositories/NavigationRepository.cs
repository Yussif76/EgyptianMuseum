using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class NavigationRepository : INavigationRepository
    {
        private readonly AppDbContext _context;

        public NavigationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
        {
            return await _context.Rooms
                .Include(r => r.Translations)
                .Where(r => r.Id == roomId && !r.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(List<Room> Rooms, List<IndoorMapPath> Paths)> GetMapGraphAsync(int mapId, CancellationToken cancellationToken = default)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Translations)
                .Where(r => r.MapId == mapId && !r.IsDeleted)
                .ToListAsync(cancellationToken);

            var paths = await _context.IndoorMapPaths
                .Where(p => p.MapId == mapId && !p.IsDeleted)
                .Include(p => p.FromRoom)
                .ThenInclude(r => r.Translations)
                .Include(p => p.ToRoom)
                .ThenInclude(r => r.Translations)
                .ToListAsync(cancellationToken);

            return (rooms, paths);
        }

        public async Task<bool> RoomsBelongToSameMapAsync(int fromRoomId, int toRoomId, CancellationToken cancellationToken = default)
        {
            var fromRoom = await _context.Rooms
                .Where(r => r.Id == fromRoomId && !r.IsDeleted)
                .Select(r => r.MapId)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromRoom == 0)
                return false;

            var toRoom = await _context.Rooms
                .Where(r => r.Id == toRoomId && !r.IsDeleted)
                .Select(r => r.MapId)
                .FirstOrDefaultAsync(cancellationToken);

            return toRoom != 0 && fromRoom == toRoom;
        }
    }
}
