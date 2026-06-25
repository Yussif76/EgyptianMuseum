using EgyptianMuseum.Application.DTOs.Tours;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface ITourService
    {
        Task<TourResponseDto> GetByIdAsync(int id, string lang = "en", CancellationToken cancellationToken = default);
        Task<List<TourResponseDto>> GetAllAsync(string lang = "en", CancellationToken cancellationToken = default);
        Task<TourResponseDto> CreateAsync(CreateTourRequestDto request, CancellationToken cancellationToken = default);
        Task<TourResponseDto> UpdateAsync(int id, UpdateTourRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<TourRoomResponseDto> AddRoomToTourAsync(int tourId, AddRoomToTourRequestDto request, CancellationToken cancellationToken = default);
        Task<TourDetailsResponseDto> GetTourDetailsAsync(int tourId, string lang = "en", CancellationToken cancellationToken = default);
        Task<List<TourRoomResponseDto>> GetTourRoomsAsync(int tourId, CancellationToken cancellationToken = default);
        Task<bool> DeleteRoomFromTourAsync(int tourId, int roomId, CancellationToken cancellationToken = default);
        Task<List<TourResponseDto>> GetRecommendedAsync(string lang = "en",CancellationToken cancellationToken = default);
        Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
            string? category, 
            int? durationMinutes, 
            int? numberOfRooms, 
            string lang = "en",
            CancellationToken cancellationToken = default);
    }
}

