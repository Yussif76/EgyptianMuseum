using EgyptianMuseum.Application.DTOs.Map;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.Maps
{
    public class IndoorMapPathService : IIndoorMapPathService
    {
        private readonly IIndoorMapPathRepository _pathRepository;

        public IndoorMapPathService(IIndoorMapPathRepository pathRepository)
        {
            _pathRepository = pathRepository;
        }

        public async Task<List<IndoorMapPathResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var paths = await _pathRepository.GetAllAsync(cancellationToken);
            return paths.Select(p => PathToResponseDto(p)).ToList();
        }

        public async Task<IndoorMapPathResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var path = await _pathRepository.GetByIdAsync(id, cancellationToken);
            return path != null ? PathToResponseDto(path) : null;
        }

        public async Task<List<IndoorMapPathResponseDto>> GetByMapIdAsync(int mapId, CancellationToken cancellationToken = default)
        {
            var paths = await _pathRepository.GetByMapIdAsync(mapId, cancellationToken);
            return paths.Select(p => PathToResponseDto(p)).ToList();
        }

        public async Task<IndoorMapPathResponseDto> CreateAsync(CreateIndoorMapPathRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidatePathRequest(request);

            var mapExists = await _pathRepository.MapExistsAsync(request.MapId, cancellationToken);
            if (!mapExists)
            {
                throw new KeyNotFoundException($"Map with ID {request.MapId} not found");
            }

            var path = new IndoorMapPath
            {
                MapId = request.MapId,
                FromRoom = request.FromRoom,
                ToRoom = request.ToRoom,
                FromX = request.FromX,
                FromY = request.FromY,
                ToX = request.ToX,
                ToY = request.ToY,
                Distance = request.Distance
            };

            await _pathRepository.AddAsync(path, cancellationToken);
            return PathToResponseDto(path);
        }

        public async Task<IndoorMapPathResponseDto> UpdateAsync(int id, UpdateIndoorMapPathRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidatePathRequest(request);

            var path = await _pathRepository.GetByIdAsync(id, cancellationToken);
            if (path == null)
            {
                throw new KeyNotFoundException($"Path with ID {id} not found");
            }

            path.FromRoom = request.FromRoom;
            path.ToRoom = request.ToRoom;
            path.FromX = request.FromX;
            path.FromY = request.FromY;
            path.ToX = request.ToX;
            path.ToY = request.ToY;
            path.Distance = request.Distance;

            await _pathRepository.UpdateAsync(path, cancellationToken);
            return PathToResponseDto(path);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var path = await _pathRepository.GetByIdAsync(id, cancellationToken);
            if (path == null)
            {
                throw new KeyNotFoundException($"Path with ID {id} not found");
            }

            await _pathRepository.DeleteAsync(id, cancellationToken);
        }

        private void ValidatePathRequest(CreateIndoorMapPathRequestDto request)
        {
            if (request.MapId <= 0)
            {
                throw new ArgumentException("MapId must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(request.FromRoom))
            {
                throw new ArgumentException("FromRoom is required");
            }

            if (string.IsNullOrWhiteSpace(request.ToRoom))
            {
                throw new ArgumentException("ToRoom is required");
            }

            if (request.Distance <= 0)
            {
                throw new ArgumentException("Distance must be greater than 0");
            }

            if (double.IsNaN(request.FromX) || double.IsNaN(request.FromY) ||
                double.IsNaN(request.ToX) || double.IsNaN(request.ToY))
            {
                throw new ArgumentException("Coordinates must be valid numbers");
            }
        }

        private void ValidatePathRequest(UpdateIndoorMapPathRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FromRoom))
            {
                throw new ArgumentException("FromRoom is required");
            }

            if (string.IsNullOrWhiteSpace(request.ToRoom))
            {
                throw new ArgumentException("ToRoom is required");
            }

            if (request.Distance <= 0)
            {
                throw new ArgumentException("Distance must be greater than 0");
            }

            if (double.IsNaN(request.FromX) || double.IsNaN(request.FromY) ||
                double.IsNaN(request.ToX) || double.IsNaN(request.ToY))
            {
                throw new ArgumentException("Coordinates must be valid numbers");
            }
        }

        private IndoorMapPathResponseDto PathToResponseDto(IndoorMapPath path)
        {
            return new IndoorMapPathResponseDto
            {
                Id = path.Id,
                MapId = path.MapId,
                FromRoom = path.FromRoom,
                ToRoom = path.ToRoom,
                FromX = path.FromX,
                FromY = path.FromY,
                ToX = path.ToX,
                ToY = path.ToY,
                Distance = path.Distance
            };
        }
    }
}
