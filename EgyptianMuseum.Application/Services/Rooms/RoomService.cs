using EgyptianMuseum.Application.DTOs.Rooms;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.Rooms
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapRepository _mapRepository;

        public RoomService(
            IRoomRepository roomRepository,
            IMapRepository mapRepository)
        {
            _roomRepository = roomRepository;
            _mapRepository = mapRepository;
        }

        public async Task<RoomResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            return MapToDto(room, "en");
        }

        public async Task<RoomResponseDto> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            return MapToDto(room, lang);
        }

        public async Task<List<RoomResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepository.GetAllAsync(cancellationToken);
            return rooms.Select(r => MapToDto(r, "en")).ToList();
        }

        public async Task<List<RoomResponseDto>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepository.GetAllAsync(cancellationToken);
            return rooms.Select(r => MapToDto(r, lang)).ToList();
        }

        public async Task<List<RoomResponseDto>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default)
        {
            if (mapId <= 0)
                throw new ArgumentException("Map ID must be greater than 0");

            var rooms = await _roomRepository.GetByMapIdAsync(mapId, cancellationToken);
            return rooms.Select(r => MapToDto(r, "en")).ToList();
        }

        public async Task<List<RoomResponseDto>> GetByMapIdAsync(int mapId, string lang, CancellationToken cancellationToken = default)
        {
            if (mapId <= 0)
                throw new ArgumentException("Map ID must be greater than 0");

            var rooms = await _roomRepository.GetByMapIdAsync(mapId, cancellationToken);
            return rooms.Select(r => MapToDto(r, lang)).ToList();
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomRequestDto request, CancellationToken cancellationToken = default)
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Room name is required");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Room description is required");

            if (request.MapId <= 0)
                throw new ArgumentException("Valid Map ID is required");

            // Verify Map exists
            var map = await _mapRepository.GetByIdAsync(request.MapId, cancellationToken);
            if (map == null)
                throw new KeyNotFoundException($"Map with ID {request.MapId} not found");

            var room = new Room
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                MapId = request.MapId,
                XCoord = request.XCoord,
                YCoord = request.YCoord
            };

            // Add translations if provided
            if (request.Translations != null && request.Translations.Any())
            {
                room.Translations = request.Translations.Select(t => new RoomTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description
                }).ToList();
            }

            var createdRoom = await _roomRepository.CreateAsync(room, cancellationToken);
            return MapToDto(createdRoom, "en");
        }

        public async Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomRequestDto request, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.Name))
                room.Name = request.Name.Trim();

            if (!string.IsNullOrWhiteSpace(request.Description))
                room.Description = request.Description.Trim();

            if (request.MapId.HasValue && request.MapId.Value > 0)
            {
                // Verify new Map exists
                var mapExists = await _mapRepository.GetByIdAsync(request.MapId.Value, cancellationToken);
                if (mapExists == null)
                    throw new KeyNotFoundException($"Map with ID {request.MapId.Value} not found");

                room.MapId = request.MapId.Value;
            }

            if (request.XCoord.HasValue)
                room.XCoord = request.XCoord.Value;

            if (request.YCoord.HasValue)
                room.YCoord = request.YCoord.Value;

            // Update translations if provided
            if (request.Translations != null && request.Translations.Any())
            {
                // Clear existing translations
                room.Translations.Clear();

                // Add new translations
                room.Translations = request.Translations.Select(t => new RoomTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description
                }).ToList();
            }

            var updated = await _roomRepository.UpdateAsync(room, cancellationToken);
            if (!updated)
                throw new InvalidOperationException("Failed to update room");

            return MapToDto(room, "en");
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Room ID must be greater than 0");

            return await _roomRepository.DeleteAsync(id, cancellationToken);
        }

        private static RoomResponseDto MapToDto(Room room, string lang = "en")
        {
            // Select translation based on language, fallback to first translation if not found
            var translation = room.Translations
                .FirstOrDefault(x => x.LanguageCode == lang)
                ?? room.Translations.FirstOrDefault();

            return new RoomResponseDto
            {
                Id = room.Id,
                Name = translation?.Name ?? room.Name,
                Description = translation?.Description ?? room.Description,
                MapId = room.MapId,
                XCoord = room.XCoord,
                YCoord = room.YCoord
            };
        }
    }
}
