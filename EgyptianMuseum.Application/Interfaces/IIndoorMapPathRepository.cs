using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IIndoorMapPathRepository
    {
        Task<List<IndoorMapPath>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IndoorMapPath?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<IndoorMapPath>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default);
        Task AddAsync(IndoorMapPath path, CancellationToken cancellationToken = default);
        Task UpdateAsync(IndoorMapPath path, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> MapExistsAsync(int mapId, CancellationToken cancellationToken = default);
    }
}
