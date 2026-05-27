using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class BreadthFirstRouteFinder
    {
        public RouteResult? FindShortestRoute(MovementRequest request)
        {
            var edges = request.Topography.Edges
                .Where(edge => edge.EdgeType == request.Thunderbird.TraversalDomain);

            var queue = new Queue<LocationCode>();
            var visited = new HashSet<LocationCode> { request.Start };
            var parentMap = new Dictionary<LocationCode, LocationCode>();

            queue.Enqueue(request.Start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == request.Destination)
                {
                    return BuildRoute(request.Start, request.Destination, parentMap);
                }

                foreach (var neighbour in GetNeighbours(edges, current))
                {
                    if (!visited.Add(neighbour))
                    {
                        continue;
                    }

                    parentMap[neighbour] = current;
                    queue.Enqueue(neighbour);
                }
            }

            return null;
        }

        private static IEnumerable<LocationCode> GetNeighbours(
            IEnumerable<ReferenceMapEdgeDefinition> edges,
            LocationCode current)
        {
            return edges
                .Where(edge => edge.Edge1 == current || edge.Edge2 == current)
                .Select(edge => edge.Edge1 == current ? edge.Edge2 : edge.Edge1);
        }

        private static RouteResult? BuildRoute(
            LocationCode start,
            LocationCode destination,
            Dictionary<LocationCode, LocationCode> parentMap)
        {
            var route = new List<LocationCode> { destination };
            var current = destination;

            while (current != start)
            {
                if (!parentMap.TryGetValue(current, out var parent))
                {
                    return null;
                }

                current = parent;
                route.Add(current);
            }

            route.Reverse();

            return new RouteResult(route, route.Count - 1);
        }
    }
}
