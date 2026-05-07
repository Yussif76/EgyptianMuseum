using EgyptianMuseum.Application.DTOs.Map;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.Maps
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;

        public MapService(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public async Task<List<MapResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var maps = await _mapRepository.GetAllAsync(cancellationToken);
            return maps.Select(m => MapToResponseDto(m)).ToList();
        }

        public async Task<MapResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var map = await _mapRepository.GetByIdAsync(id, cancellationToken);
            return map != null ? MapToResponseDto(map) : null;
        }

        public async Task<List<MapResponseDto>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(zone))
            {
                throw new ArgumentException("Zone cannot be empty");
            }

            var maps = await _mapRepository.GetByZoneAsync(zone, cancellationToken);
            return maps.Select(m => MapToResponseDto(m)).ToList();
        }

        public async Task<MapResponseDto> CreateAsync(CreateMapRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidateMapRequest(request);

            var map = new Map
            {
                Name = request.Name,
                Zone = request.Zone,
                ImageUrl = request.ImageUrl
            };

            await _mapRepository.AddAsync(map, cancellationToken);
            return MapToResponseDto(map);
        }

        public async Task<MapResponseDto> UpdateAsync(int id, UpdateMapRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidateMapRequest(request);

            var map = await _mapRepository.GetByIdAsync(id, cancellationToken);
            if (map == null)
            {
                throw new KeyNotFoundException($"Map with ID {id} not found");
            }

            map.Name = request.Name;
            map.Zone = request.Zone;
            map.ImageUrl = request.ImageUrl;

            await _mapRepository.UpdateAsync(map, cancellationToken);
            return MapToResponseDto(map);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var map = await _mapRepository.GetByIdAsync(id, cancellationToken);
            if (map == null)
            {
                throw new KeyNotFoundException($"Map with ID {id} not found");
            }

            await _mapRepository.DeleteAsync(id, cancellationToken);
        }

        private void ValidateMapRequest(CreateMapRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Zone))
            {
                throw new ArgumentException("Zone is required");
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw new ArgumentException("ImageUrl is required");
            }
        }

        private void ValidateMapRequest(UpdateMapRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Zone))
            {
                throw new ArgumentException("Zone is required");
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw new ArgumentException("ImageUrl is required");
            }
        }

        private MapResponseDto MapToResponseDto(Map map)
        {
            return new MapResponseDto
            {
                Id = map.Id,
                Name = map.Name,
                Zone = map.Zone,
                ImageUrl = map.ImageUrl
            };
        }
    }
}
