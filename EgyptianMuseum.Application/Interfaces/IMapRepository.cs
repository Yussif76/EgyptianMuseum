using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IMapRepository
    {
        Task<List<Map>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Map?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Map>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default);
        Task AddAsync(Map map, CancellationToken cancellationToken = default);
        Task UpdateAsync(Map map, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
