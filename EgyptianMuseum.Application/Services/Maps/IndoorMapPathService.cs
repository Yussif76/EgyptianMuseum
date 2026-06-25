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
                FromRoomId = request.FromRoomId,
                ToRoomId = request.ToRoomId
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

            path.FromRoomId = request.FromRoomId;
            path.ToRoomId = request.ToRoomId;

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

            if (request.FromRoomId <= 0)
            {
                throw new ArgumentException("FromRoomId must be greater than 0");
            }

            if (request.ToRoomId <= 0)
            {
                throw new ArgumentException("ToRoomId must be greater than 0");
            }

            if (request.FromRoomId == request.ToRoomId)
            {
                throw new ArgumentException("FromRoomId and ToRoomId must be different");
            }
        }

        private void ValidatePathRequest(UpdateIndoorMapPathRequestDto request)
        {
            if (request.FromRoomId <= 0)
            {
                throw new ArgumentException("FromRoomId must be greater than 0");
            }

            if (request.ToRoomId <= 0)
            {
                throw new ArgumentException("ToRoomId must be greater than 0");
            }

            if (request.FromRoomId == request.ToRoomId)
            {
                throw new ArgumentException("FromRoomId and ToRoomId must be different");
            }
        }

        private IndoorMapPathResponseDto PathToResponseDto(IndoorMapPath path)
        {
            return new IndoorMapPathResponseDto
            {
                Id = path.Id,
                MapId = path.MapId,
                FromRoomId = path.FromRoomId,
                ToRoomId = path.ToRoomId
            };
        }
    }
}
