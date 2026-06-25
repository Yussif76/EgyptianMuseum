using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Room>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Room>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default);
        Task<Room> CreateAsync(Room room, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Room room, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> RoomExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
