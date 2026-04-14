using EgyptianMuseum.Application.DTOs.ScannedArtifacts;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IScannedArtifactService
    {
        Task<ScanArtifactResponseDto> ScanArtifactAsync(string userId, ScanArtifactRequestDto request, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(string userId, CancellationToken cancellationToken = default);
        Task<ScannedArtifactDto> GetByIdAsync(string userId, int id, CancellationToken cancellationToken = default);
        Task UpdateFavoriteAsync(string userId, int scannedArtifactId, bool isFavorite, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default);
    }
}
