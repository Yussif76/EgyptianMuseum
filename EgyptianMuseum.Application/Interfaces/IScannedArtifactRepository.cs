using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IScannedArtifactRepository
    {
        Task AddAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifact>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ScannedArtifact?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ScannedArtifact?> GetByIdWithPieceAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(ScannedArtifact scannedArtifact, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}