using EgyptianMuseum.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgyptianMuseum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EgyptianMuseum.Application.DTOs.Pieces;
using System.Text.Json;


namespace EgyptianMuseum.Application.Services.Services
{
    public class PiecesService(
        IPiecesRepository<Pieces> repository,
        IScannedArtifactRepository scannedArtifactRepository) : IPiecesServices
    {
        public Task<Pieces> CreateAsync(Pieces entity)
            => repository.CreateAsync(entity);

        public Task<bool> DeleteAsync(int id)
            => repository.DeleteAsync(id);

        public async Task<IEnumerable<Pieces>> GetAllAsync()
            => await repository.GetAllAsync();

        public Task<Pieces> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => repository.GetByIdAsync(id, cancellationToken);

        public Task<bool> UpdateAsync(Pieces entity)
            => repository.UpdateAsync(entity);

        public Task<Pieces> GetByCodeAsync(string code)
            => repository.GetFirstOrDefaultAsync(x => x.Code == code);

        public Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize)
            => repository.GetPagedAsync(page, pageSize);

        public Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default)
            => repository.GetByCodeWithTranslationsAsync(code, cancellationToken);

        public Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize)
            => repository.GetPagedWithTranslationsAsync(page, pageSize);

        //public async Task<Pieces?> GetByIdWithScannedStatusAsync(int id, string userId, CancellationToken cancellationToken = default)
        //{
        //    var piece = await repository.GetByIdAsync(id, cancellationToken);
        //    if (piece == null)
        //        return null;

        //    // Check if ScannedArtifact already exists
        //    var existingScanned = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync(userId, id, cancellationToken);

        //    if (existingScanned == null)
        //    {
        //        // Create new ScannedArtifact
        //        var newScanned = new ScannedArtifact
        //        {
        //            UserId = userId,
        //            PieceId = id,
        //            LabelText = piece.Code,
        //            IsFavorite = false,
        //            ScannedAt = DateTime.UtcNow
        //        };

        //        await scannedArtifactRepository.AddAsync(newScanned, cancellationToken);
        //    }

        //    return piece;
        //}

        public async Task<Pieces?> GetByCodeWithScannedStatusAsync(string code, string userId, CancellationToken cancellationToken = default)
        {
            var piece = await repository.GetByCodeWithTranslationsAsync(code, cancellationToken);
            if (piece == null)
                return null;

            // Check if ScannedArtifact already exists
            var existingScanned = await scannedArtifactRepository.GetByUserIdAndPieceIdAsync(userId, piece.Id, cancellationToken);

            if (existingScanned == null)
            {
                // Create new ScannedArtifact
                var newScanned = new ScannedArtifact
                {
                    UserId = userId,
                    PieceId = piece.Id,
                    LabelText = piece.Name ?? piece.Code,
                    IsFavorite = false,
                    ScannedAt = DateTime.UtcNow
                };

                await scannedArtifactRepository.AddAsync(newScanned, cancellationToken);
            }

            return piece;
        }

        /// <summary>
        /// Serializes PieceLocationDto list to JSON string for storage in PieceLocationJson.
        /// Returns null if the list is null or empty.
        /// </summary>
        private string? SerializePieceLocation(List<PieceLocationDto>? locations)
        {
            if (locations == null || !locations.Any())
                return null;

            try
            {
                return JsonSerializer.Serialize(locations);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to serialize piece locations", ex);
            }
        }

        /// <summary>
        /// Deserializes PieceLocationJson string back to PieceLocationDto list.
        /// Supports both single object format: {"X": 0.3175, "Y": 0.8544}
        /// and array format: [{"X": 0.3175, "Y": 0.8544}]
        /// Returns null if the JSON is null, empty, or represents an empty array.
        /// </summary>
        private List<PieceLocationDto>? DeserializePieceLocation(string? pieceLocationJson)
        {
            if (string.IsNullOrWhiteSpace(pieceLocationJson) || pieceLocationJson == "[]")
                return null;

            try
            {
                var trimmedJson = pieceLocationJson.Trim();

                // Check if JSON is an array format
                if (trimmedJson.StartsWith('['))
                {
                    var locations = JsonSerializer.Deserialize<List<PieceLocationDto>>(trimmedJson);
                    return locations ?? null;
                }

                // Otherwise, deserialize as single object and wrap in a list
                var location = JsonSerializer.Deserialize<PieceLocationDto>(trimmedJson);
                return location != null ? new List<PieceLocationDto> { location } : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize piece locations from storage", ex);
            }
        }

        /// <summary>
        /// Converts a list of image paths into PieceImage entities.
        /// Removes duplicates by checking for existing paths in the current images collection.
        /// </summary>
        public List<PieceImage> ConvertPathsToImages(List<string>? photoPaths, List<PieceImage>? existingImages = null)
        {
            var images = new List<PieceImage>();

            if (photoPaths == null || !photoPaths.Any())
                return images;

            var existingPaths = existingImages?.Select(img => img.ImagePath).ToHashSet() ?? new HashSet<string>();

            foreach (var path in photoPaths)
            {
                if (!string.IsNullOrWhiteSpace(path) && !existingPaths.Contains(path))
                {
                    images.Add(new PieceImage { ImagePath = path });
                    existingPaths.Add(path);
                }
            }

            return images;
        }
    }
}

