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
            string lang = "en",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.LabelText))
                throw new ArgumentException("Label text cannot be empty");

            var labelText = request.LabelText.Trim();

            var piece = await _pieceRepository.GetByCodeWithTranslationsAsync(labelText, cancellationToken);
            if (piece == null)
                throw new KeyNotFoundException($"No artifact found with label '{labelText}'");

            // Check if user already has a scan for this piece
            var existingScannedArtifact = await _scannedArtifactRepository
                .GetByUserIdAndPieceIdAsync(userId, piece.Id, cancellationToken);

            if (existingScannedArtifact != null)
            {
                // Return existing scan with its favorite status preserved
                return MapToScanResponseDto(existingScannedArtifact, lang);
            }

            // Create new scan record
            var scannedArtifact = new ScannedArtifact
            {
                UserId = userId,
                PieceId = piece.Id,
                LabelText = labelText,
                IsFavorite = false,
                ScannedAt = DateTime.UtcNow
            };

            await _scannedArtifactRepository.AddAsync(scannedArtifact, cancellationToken);

            // Reload with piece data for response
            scannedArtifact.Piece = piece;
            return MapToScanResponseDto(scannedArtifact, lang);
        }

        public async Task<List<ScannedArtifactDto>> GetUserScannedArtifactsAsync(
            string userId,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var scannedArtifacts = await _scannedArtifactRepository.GetByUserIdAsync(userId, cancellationToken);

            return scannedArtifacts
                .OrderByDescending(s => s.ScannedAt)
                .Select(s => MapToDto(s, lang))
                .ToList();
        }

        public async Task<ScannedArtifactDto> GetByIdAsync(
            string userId,
            int id,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var scannedArtifact = await _scannedArtifactRepository.GetByIdWithPieceAsync(id, cancellationToken);
            if (scannedArtifact == null)
                throw new KeyNotFoundException("Scanned artifact not found");

            if (scannedArtifact.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this record");

            return MapToDto(scannedArtifact, lang);
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

        public async Task UpdateFavoriteByPieceIdAsync(
            string userId,
            int pieceId,
            bool isFavorite,
            CancellationToken cancellationToken = default)
        {
            var scannedArtifact = await _scannedArtifactRepository
                .GetByUserIdAndPieceIdAsync(userId, pieceId, cancellationToken);

            if (scannedArtifact == null)
            {
                var piece = await _pieceRepository.GetByIdAsync(pieceId, cancellationToken);
                if (piece == null)
                    throw new KeyNotFoundException("Piece not found");

                scannedArtifact = new ScannedArtifact
                {
                    UserId = userId,
                    PieceId = pieceId,
                    LabelText = piece.Code,
                    IsFavorite = isFavorite,
                    ScannedAt = DateTime.UtcNow
                };

                await _scannedArtifactRepository.AddAsync(scannedArtifact, cancellationToken);
            }
            else
            {
                scannedArtifact.IsFavorite = isFavorite;
                await _scannedArtifactRepository.UpdateAsync(scannedArtifact, cancellationToken);
            }
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

        public async Task<List<ScannedArtifactDto>> GetUserFavoritesAsync(
            string userId,
            CancellationToken cancellationToken = default,
            string lang = "en")
        {
            var favorites = await _scannedArtifactRepository.GetFavoritesByUserIdAsync(userId, cancellationToken);

            return favorites
                .Select(s => MapToDto(s, lang))
                .ToList();
        }

        private static ScannedArtifactDto MapToDto(ScannedArtifact scannedArtifact, string lang = "en")
        {
            var translation = SelectTranslation(scannedArtifact.Piece, lang);

            return new ScannedArtifactDto
            {
                Id = scannedArtifact.Id,
                PieceId = scannedArtifact.PieceId,
                LabelText = scannedArtifact.LabelText,
                IsFavorite = scannedArtifact.IsFavorite,
                ScannedAt = scannedArtifact.ScannedAt,
                PieceName = translation?.Name ?? scannedArtifact.Piece?.Name,
                PieceDescription = translation?.TextNarration,
                PieceImageUrl = scannedArtifact.Piece?.PhotoPath,
                PiecePeriod = translation?.Period,
                PieceCategory = translation?.Category
            };
        }

        private static ScanArtifactResponseDto MapToScanResponseDto(ScannedArtifact scannedArtifact, string lang = "en")
        {
            var translation = SelectTranslation(scannedArtifact.Piece, lang);

            return new ScanArtifactResponseDto
            {
                ScannedArtifactId = scannedArtifact.Id,
                PieceId = scannedArtifact.PieceId,
                LabelText = scannedArtifact.LabelText,
                IsFavorite = scannedArtifact.IsFavorite,
                ScannedAt = scannedArtifact.ScannedAt,
                PieceName = translation?.Name ?? scannedArtifact.Piece?.Name,
                PieceDescription = translation?.TextNarration,
                PieceImageUrl = scannedArtifact.Piece?.PhotoPath,
                PiecePeriod = translation?.Period,
                PieceCategory = translation?.Category
            };
        }

        private static PieceTranslation? SelectTranslation(Pieces? piece, string lang)
        {
            if (piece?.Translations == null || piece.Translations.Count == 0)
                return null;

            // Priority 1: Requested language
            var translation = piece.Translations.FirstOrDefault(t => t.LanguageCode == lang);
            if (translation != null)
                return translation;

            // Priority 2: English fallback
            translation = piece.Translations.FirstOrDefault(t => t.LanguageCode == "en");
            if (translation != null)
                return translation;

            // Priority 3: First available
            return piece.Translations.FirstOrDefault();
        }
    }
}