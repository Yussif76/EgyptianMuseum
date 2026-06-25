using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly AppDbContext _context;

        public TourRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<Tour?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Include(t => t.Translations)
                .Include(t => t.TourPieces)
                    .ThenInclude(tp => tp.Piece)
                        .ThenInclude(p => p.Translations)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<List<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Tour>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Include(t => t.Translations)
                .Include(t => t.TourPieces)
                    .ThenInclude(tp => tp.Piece)
                        .ThenInclude(p => p.Translations)
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            await _context.Tours.AddAsync(tour, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return tour;
        }

        public async Task<bool> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            var existingTour = await _context.Tours
                .Include(t => t.Translations)
                .Include(t => t.TourPieces)
                .FirstOrDefaultAsync(t => t.Id == tour.Id && !t.IsDeleted, cancellationToken);

            if (existingTour == null)
                return false;

            existingTour.Name = tour.Name;
            existingTour.Description = tour.Description;
            existingTour.DurationMinutes = tour.DurationMinutes;
            existingTour.Category = tour.Category;
            existingTour.Color = tour.Color;
            existingTour.ImageUrl = tour.ImageUrl;
            existingTour.IconPath = tour.IconPath;
            existingTour.PathImageUrl = tour.PathImageUrl;
            existingTour.MarksJson = tour.MarksJson;
            existingTour.IsRecommended = tour.IsRecommended;

            // Update translations
            existingTour.Translations = tour.Translations;

            // Update pieces
            existingTour.TourPieces = tour.TourPieces;

            _context.Tours.Update(existingTour);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);

            if (tour == null)
                return false;

            tour.IsDeleted = true;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> TourExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .AnyAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<Tour?> GetTourWithRoomsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Include(t => t.TourRooms)
                    .ThenInclude(tr => tr.Room)
                .Include(t => t.Translations)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<List<Tour>> GetAllWithRoomsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Include(t => t.TourRooms)
                    .ThenInclude(tr => tr.Room)
                .Include(t => t.Translations)
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TourPiece>> GetTourPiecesAsync(int tourId, CancellationToken cancellationToken = default)
        {
            return await _context.TourPieces
                .Include(tp => tp.Piece)
                    .ThenInclude(p => p.Translations)
                .Where(tp => tp.TourId == tourId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Tour>> GetRecommendedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tours
                .Include(t => t.Translations)
                .Include(t => t.TourPieces)
                    .ThenInclude(tp => tp.Piece)
                        .ThenInclude(p => p.Translations)
                .Where(t => !t.IsDeleted && t.IsRecommended)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
