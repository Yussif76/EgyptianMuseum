using EgyptianMuseum.Application.DTOs.Rooms;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IRoomService
    {
        Task<RoomResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<RoomResponseDto> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<List<RoomResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<RoomResponseDto>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<List<RoomResponseDto>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default);
        Task<List<RoomResponseDto>> GetByMapIdAsync(int mapId, string lang, CancellationToken cancellationToken = default);
        Task<RoomResponseDto> CreateAsync(CreateRoomRequestDto request, CancellationToken cancellationToken = default);
        Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
