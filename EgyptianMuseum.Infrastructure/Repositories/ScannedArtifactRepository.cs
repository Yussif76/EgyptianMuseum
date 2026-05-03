using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class ScannedArtifactRepository : IScannedArtifactRepository
    {
        private readonly AppDbContext _context;

        public ScannedArtifactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default)
        {
            await _context.ScannedArtifacts.AddAsync(scannedArtifact, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ScannedArtifact>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                    .ThenInclude(p => p.Translations)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.ScannedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ScannedArtifact?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<ScannedArtifact?> GetByIdWithPieceAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                    .ThenInclude(p => p.Translations)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default)
        {
            _context.ScannedArtifacts.Update(scannedArtifact);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _context.ScannedArtifacts.FindAsync(
                new object[] { id },
                cancellationToken: cancellationToken);

            if (scannedArtifact != null)
            {
                _context.ScannedArtifacts.Remove(scannedArtifact);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<ScannedArtifact?> GetByUserIdAndPieceIdAsync(string userId, int pieceId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                    .ThenInclude(p => p.Translations)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.PieceId == pieceId, cancellationToken);
        }

        public async Task<List<ScannedArtifact>> GetFavoritesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ScannedArtifacts
                .Include(s => s.Piece)
                    .ThenInclude(p => p.Translations)
                .Where(s => s.UserId == userId && s.IsFavorite)
                .OrderByDescending(s => s.ScannedAt)
                .ToListAsync(cancellationToken);
        }
    }
}