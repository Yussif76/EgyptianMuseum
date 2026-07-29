using EgyptianMuseum.Application.DTOs.Pieces;
using EgyptianMuseum.Application.DTOs.Tours;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using System.Text.Json;

namespace EgyptianMuseum.Application.Services.Tours
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITourRoomRepository _tourRoomRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IPiecesRepository<Pieces> _piecesRepository;

        public TourService(
            ITourRepository tourRepository,
            ITourRoomRepository tourRoomRepository,
            IRoomRepository roomRepository,
            IPiecesRepository<Pieces> piecesRepository)
        {
            _tourRepository = tourRepository;
            _tourRoomRepository = tourRoomRepository;
            _roomRepository = roomRepository;
            _piecesRepository = piecesRepository;
        }

        public async Task<TourResponseDto> GetByIdAsync(int id, string lang = "en", CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            var tour = await _tourRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (tour == null)
                throw new KeyNotFoundException($"Tour with ID {id} not found");

            return MapToDtoWithLocalization(tour, lang);
        }

        public async Task<List<TourResponseDto>> GetAllAsync(string lang = "en", CancellationToken cancellationToken = default)
        {
            var tours = await _tourRepository.GetAllWithDetailsAsync(cancellationToken);
            return tours.Select(t => MapToDtoWithLocalization(t, lang)).ToList();
        }
        public async Task<List<TourResponseDto>> GetRecommendedAsync(
    string lang = "en",
    CancellationToken cancellationToken = default)
        {
            var tours = await _tourRepository.GetRecommendedAsync(cancellationToken);

            return tours
                .Select(t => MapToDtoWithLocalization(t, lang))
                .ToList();
        }
        public async Task<TourResponseDto> CreateAsync(CreateTourRequestDto request, CancellationToken cancellationToken = default)
        {
            // Validate request
            ValidateCreateUpdateRequest(request);

            // Validate and load pieces
            var pieces = await ValidateAndLoadPiecesAsync(request.PieceCodes, cancellationToken);

            var tour = new Tour
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Category = request.Category.Trim(),
                DurationMinutes = request.DurationMinutes,
                Color = request.Color.Trim(),
                ImageUrl = request.ImageUrl.Trim(),
                IconPath = request.IconPath.Trim(),
                PathImageUrl = request.PathImageUrl.Trim(),
                IsRecommended = request.IsRecommended
            };

            // Add translations
            if (request.Translations != null && request.Translations.Any())
            {
                tour.Translations = request.Translations.Select(t => new TourTranslation
                {
                    LanguageCode = t.LanguageCode.Trim(),
                    Name = t.Name.Trim(),
                    Description = t.Description.Trim(),
                    Category = t.Category.Trim()
                }).ToList();
            }

            // Add pieces
            if (pieces.Any())
            {
                tour.TourPieces = pieces
                    .Select((p, index) => new TourPiece
                    {
                        PieceId = p.Id,
                        Order = index + 1
                    })
                    .ToList();
            }

            var createdTour = await _tourRepository.CreateAsync(tour, cancellationToken);
            return MapToDtoWithLocalization(createdTour, "en");
        }

        public async Task<TourResponseDto> UpdateAsync(int id, UpdateTourRequestDto request, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            ValidateCreateUpdateRequest(request);

            var existingTour = await _tourRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (existingTour == null)
                throw new KeyNotFoundException($"Tour with ID {id} not found");

            // Validate and load pieces
            var pieces = await ValidateAndLoadPiecesAsync(request.PieceCodes, cancellationToken);

            existingTour.Name = request.Name.Trim();
            existingTour.Description = request.Description.Trim();
            existingTour.Category = request.Category.Trim();
            existingTour.DurationMinutes = request.DurationMinutes;
            existingTour.Color = request.Color.Trim();
            existingTour.ImageUrl = request.ImageUrl.Trim();
            existingTour.IconPath = request.IconPath.Trim();
            existingTour.PathImageUrl = request.PathImageUrl.Trim();
            existingTour.IsRecommended = request.IsRecommended;

            // Update translations
            existingTour.Translations.Clear();
            if (request.Translations != null && request.Translations.Any())
            {
                existingTour.Translations = request.Translations.Select(t => new TourTranslation
                {
                    TourId = id,
                    LanguageCode = t.LanguageCode.Trim(),
                    Name = t.Name.Trim(),
                    Description = t.Description.Trim(),
                    Category = t.Category.Trim()
                }).ToList();
            }

            // Update pieces
            existingTour.TourPieces.Clear();
            if (pieces.Any())
            {
                existingTour.TourPieces = pieces
                    .Select((p, index) => new TourPiece
                    {
                        TourId = id,
                        PieceId = p.Id,
                        Order = index + 1
                    })
                    .ToList();
            }

            await _tourRepository.UpdateAsync(existingTour, cancellationToken);
            return MapToDtoWithLocalization(existingTour, "en");
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            return await _tourRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<TourRoomResponseDto> AddRoomToTourAsync(int tourId, AddRoomToTourRequestDto request, CancellationToken cancellationToken = default)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            if (request.RoomId <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            if (request.Order <= 0)
                throw new ArgumentException("Order must be greater than 0");

            // Verify Tour exists
            var tour = await _tourRepository.TourExistsAsync(tourId, cancellationToken);
            if (!tour)
                throw new KeyNotFoundException($"Tour with ID {tourId} not found");

            // Verify Room exists
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {request.RoomId} not found");

            // Check if Room already exists in Tour
            var exists = await _tourRoomRepository.RoomExistsInTourAsync(tourId, request.RoomId, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"Room with ID {request.RoomId} is already in this tour");

            var tourRoom = new TourRoom
            {
                TourId = tourId,
                RoomId = request.RoomId,
                Order = request.Order
            };

            await _tourRoomRepository.AddAsync(tourRoom, cancellationToken);

            return new TourRoomResponseDto
            {
                TourId = tourRoom.TourId,
                RoomId = tourRoom.RoomId,
                RoomName = room.Name,
                RoomDescription = room.Description,
                MapId = room.MapId,
                XCoord = room.XCoord,
                YCoord = room.YCoord,
                Order = tourRoom.Order
            };
        }

        public async Task<TourDetailsResponseDto> GetTourDetailsAsync(int tourId, string lang = "en", CancellationToken cancellationToken = default)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            var tour = await _tourRepository.GetTourWithRoomsAsync(tourId, cancellationToken);
            if (tour == null)
                throw new KeyNotFoundException($"Tour with ID {tourId} not found");

            var rooms = tour.TourRooms
                .OrderBy(tr => tr.Order)
                .Select(tr => new TourRoomResponseDto
                {
                    TourId = tr.TourId,
                    RoomId = tr.RoomId,
                    RoomName = tr.Room.Name,
                    RoomDescription = tr.Room.Description,
                    MapId = tr.Room.MapId,
                    XCoord = tr.Room.XCoord,
                    YCoord = tr.Room.YCoord,
                    Order = tr.Order
                })
                .ToList();

            // Get translation for this language
            var translation = tour.Translations
                .FirstOrDefault(x => x.LanguageCode == lang)
                ?? tour.Translations.FirstOrDefault();

            return new TourDetailsResponseDto
            {
                Id = tour.Id,
                Name = translation?.Name ?? tour.Name,
                Description = translation?.Description ?? tour.Description,
                DurationMinutes = tour.DurationMinutes,
                Category = translation?.Category ?? tour.Category,
                Rooms = rooms
            };
        }

        public async Task<List<TourRoomResponseDto>> GetTourRoomsAsync(int tourId, CancellationToken cancellationToken = default)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            // Verify Tour exists
            var exists = await _tourRepository.TourExistsAsync(tourId, cancellationToken);
            if (!exists)
                throw new KeyNotFoundException($"Tour with ID {tourId} not found");

            var tourRooms = await _tourRoomRepository.GetByTourIdAsync(tourId, cancellationToken);

            return tourRooms
                .Select(tr => new TourRoomResponseDto
                {
                    TourId = tr.TourId,
                    RoomId = tr.RoomId,
                    RoomName = tr.Room.Name,
                    RoomDescription = tr.Room.Description,
                    MapId = tr.Room.MapId,
                    XCoord = tr.Room.XCoord,
                    YCoord = tr.Room.YCoord,
                    Order = tr.Order
                })
                .ToList();
        }

        public async Task<bool> DeleteRoomFromTourAsync(int tourId, int roomId, CancellationToken cancellationToken = default)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be greater than 0");

            if (roomId <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            return await _tourRoomRepository.DeleteAsync(tourId, roomId, cancellationToken);
        }

        public async Task<List<RecommendedTourResponseDto>> RecommendToursAsync(
            string? category,
            int? durationMinutes,
            int? numberOfRooms,
            string lang = "en",
            CancellationToken cancellationToken = default)
        {
            // Validate that at least one parameter is provided
            if (string.IsNullOrWhiteSpace(category) && !durationMinutes.HasValue && !numberOfRooms.HasValue)
                throw new ArgumentException("At least one filter parameter is required: category, durationMinutes, or numberOfRooms");

            // Validate provided values
            if (durationMinutes.HasValue && durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (numberOfRooms.HasValue && numberOfRooms <= 0)
                throw new ArgumentException("Number of rooms must be greater than 0");

            var tours = await _tourRepository.GetAllWithRoomsAsync(cancellationToken);

            var recommendedTours = tours
                .Select(tour =>
                {
                    // Get translation for this language
                    var translation = tour.Translations
                        .FirstOrDefault(x => x.LanguageCode == lang)
                        ?? tour.Translations.FirstOrDefault();

                    // Calculate scores only for provided filters
                    var tourCategory = translation?.Category ?? tour.Category;
                    var categoryMatched = !string.IsNullOrWhiteSpace(category) &&
                        string.Equals(tourCategory, category, StringComparison.OrdinalIgnoreCase);

                    var durationDifference = durationMinutes.HasValue
                        ? Math.Abs(tour.DurationMinutes - durationMinutes.Value)
                        : int.MaxValue;

                    var roomDifference = numberOfRooms.HasValue
                        ? Math.Abs(tour.TourRooms.Count - numberOfRooms.Value)
                        : int.MaxValue;

                    return new RecommendedTourResponseDto
                    {
                        Id = tour.Id,
                        Name = translation?.Name ?? tour.Name,
                        Description = translation?.Description ?? tour.Description,
                        DurationMinutes = tour.DurationMinutes,
                        Category = tourCategory,
                        RoomsCount = tour.TourRooms.Count,
                        DurationDifference = durationDifference,
                        RoomDifference = roomDifference,
                        CategoryMatched = categoryMatched
                    };
                })
                .OrderBy(r => !r.CategoryMatched ? 1 : 0)  // Category match first (if provided)
                .ThenBy(r => r.DurationDifference)          // Then closest duration (if provided)
                .ThenBy(r => r.RoomDifference)              // Then closest room count (if provided)
                .ToList();

            return recommendedTours;
        }

        /// <summary>
        /// Maps Tour entity to TourResponseDto with localization support.
        /// Includes translated pieces with their translations.
        /// </summary>
        private TourResponseDto MapToDtoWithLocalization(Tour tour, string lang = "en")
        {
            // Get translation for this language, fallback to first translation or base fields
            var translation = tour.Translations
                .FirstOrDefault(x => x.LanguageCode == lang)
                ?? tour.Translations.FirstOrDefault();

            // Map pieces with their translations, ordered by Order to preserve sequence
            var pieces = tour.TourPieces
                .OrderBy(tp => tp.Order)
                .Select(tp => MapPieceToDtoWithTranslation(tp.Piece, lang))
                .ToList();

            return new TourResponseDto
            {
                Id = tour.Id,
                Name = translation?.Name ?? tour.Name,
                Description = translation?.Description ?? tour.Description,
                DurationMinutes = tour.DurationMinutes,
                Category = translation?.Category ?? tour.Category,
                Color = tour.Color,
                ImageUrl = tour.ImageUrl,
                IconPath = tour.IconPath,
                PathImageUrl = tour.PathImageUrl,
                IsRecommended = tour.IsRecommended,
                Pieces = pieces
            };
        }

        /// <summary>
        /// Maps a Piece with translation in the specified language.
        /// Uses the same translation logic as PiecesController.
        /// </summary>
        private TourPieceResponseDto MapPieceToDtoWithTranslation(Pieces piece, string lang = "en")
        {
            var translation = piece.Translations
                .FirstOrDefault(x => x.LanguageCode == lang);

            // Deserialize piece location
            var pieceLocation = DeserializePieceLocation(piece.PieceLocationJson);

            return new TourPieceResponseDto
            {
                Id = piece.Id,
                Code = piece.Code,
                PhotoPaths = piece.Images.Select(img => img.ImagePath).ToList(),
                PieceLocation = pieceLocation,
                Name = translation?.Name ?? piece.Name ?? string.Empty,
                TextNarration = translation?.TextNarration ?? string.Empty,
                Period = translation?.Period ?? string.Empty,
                Category = translation?.Category ?? string.Empty
            };
        }

        /// <summary>
        /// Validates that all piece codes exist and loads the corresponding pieces.
        /// </summary>
        private async Task<List<Pieces>> ValidateAndLoadPiecesAsync(List<string> pieceCodes, CancellationToken cancellationToken)
        {
            if (!pieceCodes.Any())
                return new List<Pieces>();

            var pieces = new List<Pieces>();
            var notFoundCodes = new List<string>();

            foreach (var code in pieceCodes.Distinct())
            {
                var piece = await _piecesRepository.GetByCodeWithTranslationsAsync(code.Trim(), cancellationToken);
                if (piece == null)
                {
                    notFoundCodes.Add(code);
                }
                else
                {
                    pieces.Add(piece);
                }
            }

            if (notFoundCodes.Any())
            {
                throw new KeyNotFoundException(
                    $"The following piece codes do not exist: {string.Join(", ", notFoundCodes)}");
            }

            return pieces;
        }

        /// <summary>
        /// Validates create/update request data.
        /// </summary>
        private void ValidateCreateUpdateRequest(dynamic request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Tour name is required");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Tour description is required");

            if (request.DurationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Tour category is required");

            if (string.IsNullOrWhiteSpace(request.Color))
                throw new ArgumentException("Tour color is required");

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
                throw new ArgumentException("Tour image URL is required");

            if (string.IsNullOrWhiteSpace(request.IconPath))
                throw new ArgumentException("Tour icon path is required");

            if (string.IsNullOrWhiteSpace(request.PathImageUrl))
                throw new ArgumentException("Tour path image URL is required");
        }

        /// <summary>
        /// Deserializes PieceLocationJson string back to PieceLocationDto.
        /// Supports both single object format: {"X": 0.3175, "Y": 0.8544}
        /// and array format: [{"X": 0.3175, "Y": 0.8544}]
        /// Returns null if the JSON is null, empty, or represents an empty array.
        /// </summary>
        private PieceLocationDto? DeserializePieceLocation(string? pieceLocationJson)
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
                    return locations?.FirstOrDefault();
                }

                // Otherwise, deserialize as single object
                var location = JsonSerializer.Deserialize<PieceLocationDto>(trimmedJson);
                return location;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize piece location from storage", ex);
            }
        }
    }
}
