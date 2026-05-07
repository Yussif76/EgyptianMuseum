using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class MapRepository : IMapRepository
    {
        private readonly AppDbContext _context;

        public MapRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Map>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Maps
                .Where(m => !m.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<Map?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Maps
                .Where(m => m.Id == id && !m.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Map>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default)
        {
            return await _context.Maps
                .Where(m => m.Zone == zone && !m.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Map map, CancellationToken cancellationToken = default)
        {
            map.CreatedAt = DateTime.UtcNow;
            map.IsDeleted = false;
            await _context.Maps.AddAsync(map, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Map map, CancellationToken cancellationToken = default)
        {
            map.UpdatedAt = DateTime.UtcNow;
            _context.Maps.Update(map);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var map = await GetByIdAsync(id, cancellationToken);
            if (map != null)
            {
                map.IsDeleted = true;
                map.UpdatedAt = DateTime.UtcNow;
                _context.Maps.Update(map);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
