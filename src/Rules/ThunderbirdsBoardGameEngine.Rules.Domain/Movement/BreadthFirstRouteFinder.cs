using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class BreadthFirstRouteFinder
    {
        public RouteResult? FindShortestRoute(MovementRequest request)
        {
            var edges = request.Topography.Edges.Where(edge => edge.EdgeType == request.Thunderbird.TraversalDomain);

            var queue = new Queue<LocationCode>();
            var visited = new HashSet<LocationCode>();
            var parentMap = new Dictionary<LocationCode, LocationCode>();

            queue.Enqueue(request.Start);
            visited.Add(request.Start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == request.Destination)
                {
                    var route = new List<LocationCode> { request.Destination };
                    var routeCurrent = request.Destination;

                    while (routeCurrent != request.Start)
                    {
                        if (!parentMap.TryGetValue(routeCurrent, out var parent))
                        {
                            return null;
                        }

                        routeCurrent = parent;
                        route.Add(routeCurrent);
                    }

                    route.Reverse();

                    return new RouteResult(
                        route,
                        route.Count - 1);
                }

                var neighbours = edges
                    .Where(edge => edge.Edge1 == current || edge.Edge2 == current)
                    .Select(edge => edge.Edge1 == current ? edge.Edge2 : edge.Edge1);

                foreach (var neighbour in neighbours)
                {
                    if (visited.Contains(neighbour))
                    {
                        continue;
                    }

                    visited.Add(neighbour);
                    parentMap[neighbour] = current;
                    queue.Enqueue(neighbour);
                }
            }

            return null;
        }
    }
}
