using EgyptianMuseum.Application.DTOs.ScannedArtifacts;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IScannedArtifactService
    {
        Task<ScanArtifactResponseDto> ScanArtifactAsync(string userId, ScanArtifactRequestDto request, string lang = "en", CancellationToken cancellationToken = default);
        Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
        Task<ScannedArtifactDto> GetByIdAsync(string userId, int id, CancellationToken cancellationToken = default, string lang = "en");
        Task UpdateFavoriteAsync(string userId, int scannedArtifactId, bool isFavorite, CancellationToken cancellationToken = default);
        Task UpdateFavoriteByPieceIdAsync(string userId, int pieceId, bool isFavorite, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default);
        Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default, string lang = "en");
    }
}