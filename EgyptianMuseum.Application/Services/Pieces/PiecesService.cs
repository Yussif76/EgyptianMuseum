using EgyptianMuseum.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgyptianMuseum.Domain.Entities;
using Microsoft.EntityFrameworkCore;


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
            => repository.GetByIdAsync(id);

        public Task<bool> UpdateAsync(Pieces entity)
            => repository.UpdateAsync(entity);

        public Task<Pieces> GetByCodeAsync(string code)
            => repository.GetFirstOrDefaultAsync(x => x.Code == code);

        public Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize)
            => repository.GetPagedAsync(page, pageSize);

        public Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default)
            => repository.GetByCodeWithTranslationsAsync(code);

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
    }
}

