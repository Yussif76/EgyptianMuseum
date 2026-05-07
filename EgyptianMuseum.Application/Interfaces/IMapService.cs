using EgyptianMuseum.Application.DTOs.Map;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IMapService
    {
        Task<List<MapResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<MapResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<MapResponseDto>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default);
        Task<MapResponseDto> CreateAsync(CreateMapRequestDto request, CancellationToken cancellationToken = default);
        Task<MapResponseDto> UpdateAsync(int id, UpdateMapRequestDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
