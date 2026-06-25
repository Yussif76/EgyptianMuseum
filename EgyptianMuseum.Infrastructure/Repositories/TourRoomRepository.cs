using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class TourRoomRepository : ITourRoomRepository
    {
        private readonly AppDbContext _context;

        public TourRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TourRoom?> GetByIdAsync(int tourId, int roomId, CancellationToken cancellationToken = default)
        {
            return await _context.TourRooms
                .Include(tr => tr.Room)
                .FirstOrDefaultAsync(tr => tr.TourId == tourId && tr.RoomId == roomId, cancellationToken);
        }

        public async Task<List<TourRoom>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
        {
            return await _context.TourRooms
                .Include(tr => tr.Room)
                .Where(tr => tr.TourId == tourId)
                .OrderBy(tr => tr.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> RoomExistsInTourAsync(int tourId, int roomId, CancellationToken cancellationToken = default)
        {
            return await _context.TourRooms
                .AnyAsync(tr => tr.TourId == tourId && tr.RoomId == roomId, cancellationToken);
        }

        public async Task<TourRoom> AddAsync(TourRoom tourRoom, CancellationToken cancellationToken = default)
        {
            await _context.TourRooms.AddAsync(tourRoom, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return tourRoom;
        }

        public async Task<bool> DeleteAsync(int tourId, int roomId, CancellationToken cancellationToken = default)
        {
            var tourRoom = await _context.TourRooms
                .FirstOrDefaultAsync(tr => tr.TourId == tourId && tr.RoomId == roomId, cancellationToken);

            if (tourRoom == null)
                return false;

            _context.TourRooms.Remove(tourRoom);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
