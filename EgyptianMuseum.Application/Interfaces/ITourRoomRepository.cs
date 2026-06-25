using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface ITourRoomRepository
    {
        Task<TourRoom?> GetByIdAsync(int tourId, int roomId, CancellationToken cancellationToken = default);
        Task<List<TourRoom>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default);
        Task<bool> RoomExistsInTourAsync(int tourId, int roomId, CancellationToken cancellationToken = default);
        Task<TourRoom> AddAsync(TourRoom tourRoom, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int tourId, int roomId, CancellationToken cancellationToken = default);
    }
}
