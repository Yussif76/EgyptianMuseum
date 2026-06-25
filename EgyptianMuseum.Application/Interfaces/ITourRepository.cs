using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface ITourRepository
    {
        Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Tour?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Tour>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Tour>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Tour tour, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> TourExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<Tour?> GetTourWithRoomsAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Tour>> GetAllWithRoomsAsync(CancellationToken cancellationToken = default);
        Task<List<Tour>> GetRecommendedAsync(CancellationToken cancellationToken = default);
        Task<List<TourPiece>> GetTourPiecesAsync(int tourId, CancellationToken cancellationToken = default);
    }
}

