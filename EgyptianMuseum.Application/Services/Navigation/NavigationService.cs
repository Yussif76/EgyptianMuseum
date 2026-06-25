using EgyptianMuseum.Application.DTOs.Navigation;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly INavigationRepository _navigationRepository;

        public NavigationService(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }

        public async Task<ShortestPathResponseDto> GetShortestPathAsync(ShortestPathRequestDto request, CancellationToken cancellationToken = default)
        {
            return await GetShortestPathAsync(request, "en", cancellationToken);
        }

        public async Task<ShortestPathResponseDto> GetShortestPathAsync(ShortestPathRequestDto request, string lang, CancellationToken cancellationToken = default)
        {
            // Validation: Check if fromRoomId and toRoomId are different
            if (request.FromRoomId == request.ToRoomId)
            {
                return new ShortestPathResponseDto
                {
                    Success = false,
                    Message = "Source and destination rooms must be different.",
                    Path = new(),
                    TotalDistance = 0
                };
            }

            // Get the source room to determine the map
            var sourceRoom = await _navigationRepository.GetRoomByIdAsync(request.FromRoomId, cancellationToken);
            if (sourceRoom == null)
            {
                return new ShortestPathResponseDto
                {
                    Success = false,
                    Message = $"Source room with ID {request.FromRoomId} not found.",
                    Path = new(),
                    TotalDistance = 0
                };
            }

            // Get the destination room
            var destinationRoom = await _navigationRepository.GetRoomByIdAsync(request.ToRoomId, cancellationToken);
            if (destinationRoom == null)
            {
                return new ShortestPathResponseDto
                {
                    Success = false,
                    Message = $"Destination room with ID {request.ToRoomId} not found.",
                    Path = new(),
                    TotalDistance = 0
                };
            }

            // Validation: Check if both rooms belong to the same map
            if (sourceRoom.MapId != destinationRoom.MapId)
            {
                return new ShortestPathResponseDto
                {
                    Success = false,
                    Message = "Source and destination rooms must belong to the same map.",
                    Path = new(),
                    TotalDistance = 0
                };
            }

            // Get all rooms and paths for the map
            var (rooms, paths) = await _navigationRepository.GetMapGraphAsync(sourceRoom.MapId, cancellationToken);

            // Build the graph and run Dijkstra's algorithm
            var result = CalculateShortestPath(sourceRoom, destinationRoom, rooms, paths, lang);

            return result;
        }

        /// <summary>
        /// Calculates the shortest path using Dijkstra's Algorithm.
        /// </summary>
        private ShortestPathResponseDto CalculateShortestPath(Room source, Room destination, List<Room> allRooms, List<IndoorMapPath> allPaths, string lang = "en")
        {
            // Build adjacency list (bidirectional graph)
            var graph = BuildGraph(allRooms, allPaths);

            // Run Dijkstra's algorithm
            var (distances, previous) = RunDijkstra(source, graph, allRooms);

            // Check if destination is reachable
            if (!distances.ContainsKey(destination.Id) || double.IsInfinity(distances[destination.Id]))
            {
                return new ShortestPathResponseDto
                {
                    Success = false,
                    Message = $"No path exists between room {source.Name} and room {destination.Name}.",
                    Path = new(),
                    TotalDistance = 0
                };
            }

            // Reconstruct the path
            var path = ReconstructPath(source.Id, destination.Id, previous, allRooms);
            var totalDistance = distances[destination.Id];

            // Convert to DTOs with translation support
            var pathDtos = path.Select(room => new NavigationRoomStepDto
            {
                RoomId = room.Id,
                Name = GetRoomDisplayName(room, lang),
                X = room.XCoord,
                Y = room.YCoord
            }).ToList();

            return new ShortestPathResponseDto
            {
                Success = true,
                Path = pathDtos,
                TotalDistance = totalDistance
            };
        }

        /// <summary>
        /// Builds an adjacency list representation of the graph.
        /// Each edge stores the destination room and the calculated distance.
        /// </summary>
        private Dictionary<int, List<(int DestinationRoomId, double Distance, Room DestinationRoom)>> BuildGraph(List<Room> rooms, List<IndoorMapPath> paths)
        {
            var graph = new Dictionary<int, List<(int, double, Room)>>();

            // Initialize graph with all rooms
            foreach (var room in rooms)
            {
                graph[room.Id] = new List<(int, double, Room)>();
            }

            // Add edges (bidirectional)
            foreach (var path in paths)
            {
                var distance = CalculateDistance(path.FromRoom, path.ToRoom);

                // Add forward edge
                if (graph.ContainsKey(path.FromRoomId))
                {
                    graph[path.FromRoomId].Add((path.ToRoomId, distance, path.ToRoom));
                }

                // Add reverse edge (bidirectional)
                if (graph.ContainsKey(path.ToRoomId))
                {
                    graph[path.ToRoomId].Add((path.FromRoomId, distance, path.FromRoom));
                }
            }

            return graph;
        }

        /// <summary>
        /// Runs Dijkstra's Algorithm to find shortest distances from source to all other nodes.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - Dictionary of distances from source to each node
        /// - Dictionary of previous nodes for path reconstruction
        /// </returns>
        private (Dictionary<int, double> Distances, Dictionary<int, int?> Previous) RunDijkstra(Room source, Dictionary<int, List<(int, double, Room)>> graph, List<Room> allRooms)
        {
            var distances = new Dictionary<int, double>();
            var previous = new Dictionary<int, int?>();
            var unvisited = new PriorityQueue<(int RoomId, double Distance), double>();

            // Initialize distances
            foreach (var room in allRooms)
            {
                distances[room.Id] = double.MaxValue;
                previous[room.Id] = null;
            }

            distances[source.Id] = 0;
            unvisited.Enqueue((source.Id, 0), 0);

            while (unvisited.Count > 0)
            {
                var (currentRoomId, _) = unvisited.Dequeue();
                var currentDistance = distances[currentRoomId];

                // Skip if this node was already processed with a shorter distance
                if (currentDistance == double.MaxValue)
                    continue;

                // Check all neighbors
                if (graph.ContainsKey(currentRoomId))
                {
                    foreach (var (neighborId, edgeDistance, _) in graph[currentRoomId])
                    {
                        var newDistance = currentDistance + edgeDistance;

                        if (newDistance < distances[neighborId])
                        {
                            distances[neighborId] = newDistance;
                            previous[neighborId] = currentRoomId;
                            unvisited.Enqueue((neighborId, newDistance), newDistance);
                        }
                    }
                }
            }

            return (distances, previous);
        }

        /// <summary>
        /// Reconstructs the path from source to destination using the previous dictionary.
        /// </summary>
        private List<Room> ReconstructPath(int sourceId, int destinationId, Dictionary<int, int?> previous, List<Room> allRooms)
        {
            var path = new List<int>();
            int? current = destinationId;

            while (current != null)
            {
                path.Add(current.Value);
                current = previous[current.Value];
            }

            path.Reverse();

            // Convert room IDs back to Room objects
            var roomMap = allRooms.ToDictionary(r => r.Id);
            return path.Where(id => roomMap.ContainsKey(id)).Select(id => roomMap[id]).ToList();
        }

        /// <summary>
        /// Calculates the Euclidean distance between two rooms based on their coordinates.
        /// Distance is NOT stored in the database; it's calculated dynamically.
        /// </summary>
        private double CalculateDistance(Room fromRoom, Room toRoom)
        {
            var deltaX = toRoom.XCoord - fromRoom.XCoord;
            var deltaY = toRoom.YCoord - fromRoom.YCoord;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Gets the display name for a room based on language and translations.
        /// Fallback to room.Name if translation is not found.
        /// </summary>
        private string GetRoomDisplayName(Room room, string lang = "en")
        {
            var translation = room.Translations
                .FirstOrDefault(x => x.LanguageCode == lang)
                ?? room.Translations.FirstOrDefault();

            return translation?.Name ?? room.Name;
        }
    }
}
