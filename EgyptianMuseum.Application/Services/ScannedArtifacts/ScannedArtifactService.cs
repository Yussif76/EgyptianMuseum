using EgyptianMuseum.Application.DTOs.ScannedArtifacts;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.ScannedArtifacts
{
    public class ScannedArtifactService : IScannedArtifactService
    {
        private readonly IScannedArtifactRepository _scannedArtifactRepository;
        private readonly IPiecesRepository<Pieces> _pieceRepository;

        public ScannedArtifactService(
            IScannedArtifactRepository scannedArtifactRepository,
            IPiecesRepository<Pieces> pieceRepository)
        {
            _scannedArtifactRepository = scannedArtifactRepository;
            _pieceRepository = pieceRepository;
        }

        public async Task<ScanArtifactResponseDto> ScanArtifactAsync(
            string userId,
            ScanArtifactRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.LabelText))
                throw new ArgumentException("Label text cannot be empty");

            var labelText = request.LabelText.Trim();

            var piece = await _pieceRepository.GetByCodeWithTranslationsAsync(labelText, cancellationToken);
            if (piece == null)
                throw new KeyNotFoundException($"No artifact found with label '{labelText}'");

            var scannedArtifact = new ScannedArtifact
            {
                UserId = userId,
                PieceId = piece.Id,
                LabelText = labelText,
                IsFavorite = false,
                ScannedAt = DateTime.UtcNow
            };

            await _scannedArtifactRepository.AddAsync(scannedArtifact, cancellationToken);
            var translation = piece.Translations.FirstOrDefault(t => t.LanguageCode == "en");
            return new ScanArtifactResponseDto
            {
                ScannedArtifactId = scannedArtifact.Id,
                PieceId = piece.Id,
                LabelText = scannedArtifact.LabelText,
                IsFavorite = scannedArtifact.IsFavorite,
                ScannedAt = scannedArtifact.ScannedAt,
                PieceName = piece.Name,
                PieceDescription =translation.TextNarration ,
                PieceImageUrl = piece.PhotoPath,
                PiecePeriod = translation.Period,
                PieceCategory = translation.Category
            };
        }

        public async Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifacts = await _scannedArtifactRepository.GetByUserIdAsync(userId, cancellationToken);

            return scannedArtifacts
                .OrderByDescending(s => s.ScannedAt)
                .Select(s => MapToDto(s))
                .ToList();
        }

        public async Task<ScannedArtifactDto> GetByIdAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdWithPieceAsync(id, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            return MapToDto(scannedArtifact);
        }

        public async Task UpdateFavoriteAsync(
            string userId,
            int scannedArtifactId,
            bool isFavorite,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdAsync(scannedArtifactId, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            scannedArtifact.IsFavorite = isFavorite;
            await _scannedArtifactRepository.UpdateAsync(scannedArtifact, cancellationToken);
        }

        public async Task DeleteAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdAsync(id, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            await _scannedArtifactRepository.DeleteAsync(id, cancellationToken);
        }

        private static ScannedArtifactDto MapToDto(ScannedArtifact s)
        {
            return new ScannedArtifactDto
            {
                Id = s.Id,
                PieceId = s.PieceId,
                LabelText = s.LabelText,
                IsFavorite = s.IsFavorite,
                ScannedAt = s.ScannedAt,
                PieceName = s.Piece?.Name,
                
            };
        }
    }
}