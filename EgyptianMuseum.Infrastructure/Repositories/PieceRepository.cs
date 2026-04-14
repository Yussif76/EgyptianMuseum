using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class PieceRepository : IPieceRepository
    {
        private readonly AppDbContext _context;

        public PieceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Piece?> GetByLabelTextAsync(string labelText, CancellationToken cancellationToken = default)
        {
            return await _context.Pieces
                .FirstOrDefaultAsync(p => p.LabelText == labelText, cancellationToken);
        }

        public async Task<Piece?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Pieces
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
