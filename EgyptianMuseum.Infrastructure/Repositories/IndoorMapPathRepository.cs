using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class IndoorMapPathRepository : IIndoorMapPathRepository
    {
        private readonly AppDbContext _context;

        public IndoorMapPathRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<IndoorMapPath>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.IndoorMapPaths
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IndoorMapPath?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.IndoorMapPaths
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<IndoorMapPath>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default)
        {
            return await _context.IndoorMapPaths
                .Where(p => p.MapId == mapId && !p.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(IndoorMapPath path, CancellationToken cancellationToken = default)
        {
            path.CreatedAt = DateTime.UtcNow;
            path.IsDeleted = false;
            await _context.IndoorMapPaths.AddAsync(path, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(IndoorMapPath path, CancellationToken cancellationToken = default)
        {
            path.UpdatedAt = DateTime.UtcNow;
            _context.IndoorMapPaths.Update(path);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var path = await GetByIdAsync(id, cancellationToken);
            if (path != null)
            {
                path.IsDeleted = true;
                path.UpdatedAt = DateTime.UtcNow;
                _context.IndoorMapPaths.Update(path);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> MapExistsAsync(int mapId, CancellationToken cancellationToken = default)
        {
            return await _context.Maps
                .AnyAsync(m => m.Id == mapId && !m.IsDeleted, cancellationToken);
        }
    }
}
