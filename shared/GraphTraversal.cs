namespace shared;

public class GraphTraversal
{
    public static Dictionary<int, int> Dijkstra(Dictionary<int, List<WeightedNode>> graph, int source)
    {
        var (distances, vertices) = (new Dictionary<int, int>(graph.Count), graph.Keys.ToList());
        for (int i = 0; i < vertices.Count; i++)
            distances[vertices[i]] = Int32.MaxValue;
        distances[source] = 0;

        var pq = new SortedSet<WeightedNode> { new(source, 0) };
        while (pq.Count > 0) {
            var current = pq.First();
            pq.Remove(current);

            foreach (var neighbor in graph[current.Vertex]) {
                var proposal = distances[current.Vertex] + neighbor.Weight;
                if (proposal < distances[neighbor.Vertex]) {
                    distances[neighbor.Vertex] = proposal;
                    pq.Add(new(neighbor.Vertex, distances[neighbor.Vertex]));
                }
            }
        }

        return distances;
    }

    public static (Dictionary<int, int> distances, Dictionary<int, int> parents) DijkstraWithPaths(Dictionary<int, List<WeightedNode>> graph, int source)
    {
        var (distances, vertices) = (new Dictionary<int, int>(graph.Count), graph.Keys.ToList());
        for (int i = 0; i < vertices.Count; i++)
            distances[vertices[i]] = Int32.MaxValue;

        var parents = new Dictionary<int, int>();

        distances[source] = 0;
        parents[source] = -1; //todo

        var pq = new SortedSet<WeightedNode> { new(source, 0) };
        while (pq.Count > 0) {
            var current = pq.First();
            pq.Remove(current);

            foreach (var neighbor in graph[current.Vertex]) {
                var proposal = distances[current.Vertex] + neighbor.Weight;
                if (proposal < distances[neighbor.Vertex]) {
                    distances[neighbor.Vertex] = proposal;
                    parents[neighbor.Vertex] = current.Vertex;
                    pq.Add(new(neighbor.Vertex, distances[neighbor.Vertex]));
                }
            }
        }

        return (distances, parents);
    }

    public static string GetFriendlyPath(int endVertex, Dictionary<int, int> parents)
    {
        var p = endVertex;
        var path = new List<int>();
        do {
            path.Add(p);
            p = parents[p];
        } while (p != -1);
        path.Reverse();
        
        return string.Join(" -> ", path);
    }

    public static void DijkstraDemo()
    {
        var graph = new Dictionary<int, List<WeightedNode>>
        {
            { 0, new() { new (1, 4), new (7, 8) } },
            { 1, new() { new (2, 8), new (7, 11), new (0, 7) } },
            { 2, new() { new (1, 8) , new (3, 7), new (8, 2), new (5, 4) } },
            { 3, new() { new (2, 7), new (4, 9), new (5, 15) } },
            { 4, new() { new (3, 9), new (5, 10) } },
            { 5, new() { new (4, 10), new (6, 2) } },
            { 6, new() { new (5, 2), new (7, 1), new (8, 6) } },
            { 7, new() { new (0, 8), new (1, 11), new (6, 1), new (8, 7) } },
            { 8, new() { new (2, 2), new (6, 6), new (7, 1) , new (10, 10) } },
            {10, new() { new (8,10) } }
        };

        var (distances, parents) = DijkstraWithPaths(graph, 0);
        foreach (var vertex in distances) {
            Console.WriteLine("{0,3}:\t{1,3}\t{2}", vertex.Key, distances[vertex.Key], GetFriendlyPath(vertex.Key, parents));
        }
    }
}