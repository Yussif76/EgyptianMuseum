using EgyptianMuseum.Application.DTOs.Map;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IIndoorMapPathService
    {
        Task<List<IndoorMapPathResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IndoorMapPathResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<IndoorMapPathResponseDto>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default);
        Task<IndoorMapPathResponseDto> CreateAsync(CreateIndoorMapPathRequestDto request, CancellationToken cancellationToken = default);
        Task<IndoorMapPathResponseDto> UpdateAsync(int id, UpdateIndoorMapPathRequestDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
