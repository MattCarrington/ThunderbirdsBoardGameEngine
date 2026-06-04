using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class BreadthFirstRouteFinder : IRouteFinder
    {
        public RouteResult? FindShortestRoute(MovementInput request)
        {
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

                foreach (var neighbour in request.Topography.GetNeighbours(current, request.Thunderbird.TraversalDomain))
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
