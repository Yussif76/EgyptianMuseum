using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IPieceRepository
    {
        Task<Piece?> GetByLabelTextAsync(string labelText, CancellationToken cancellationToken = default);
        Task<Piece?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
