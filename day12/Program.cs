using shared;

namespace day12;

/// <summary>
/// Hill Climbing Algorithm
/// </summary>
internal class Program
{
    private static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
        var (grid, start, end, allAs) = parseResults[0];
        Console.WriteLine($"Parsing: {grid.GetLength(0)}x{grid.GetLength(1)} grid in {parseTimings[0]:F2}ms");

        var (part1Results, part1Timings) = Perf.BenchmarkTime(() => BFS(grid, start)(end).Count - 1);
        Console.WriteLine($"Part 1 : {part1Results[0]} steps in {part1Timings[0]} ms");

        var (part2Results, part2Timings) = Perf.BenchmarkTime(() => {
            // instead of exploring N paths from all As to the end we do the reverse -
            // explore 1 paths from the end and then scan for minimum A
            var reversedPath = BFSReverse(grid, end);
            var minSteps = allAs.Select(startA => reversedPath(startA).Count - 1).Where(x => x != -1).Min();
            return minSteps;
        });

        Console.WriteLine($"Part 2 : {part2Results[0]} steps in {part2Timings[0]:F2}ms");
        Console.WriteLine();
        Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0] + part2Timings[0]:F2}ms");
    }

    private static (int[,] grid, Point start, Point end, List<Point> allAs) Parse(string fileName) {
        var data = File.ReadAllLines(fileName);
        Point start = new(0, 0), end = new(0, 0);
        var allAs = new List<Point>(); // needed for part 2
        var grid = new int[data.Length + 2, data[0].Length + 2]; // borders (0s) to make getting neighbors more readable
        for (var y = 0; y < data.Length; y++) {
            for (var x = 0; x < data[0].Length; x++) {
                if (data[y][x] == 'S')
                    start = new Point(x, y);
                if (data[y][x] == 'E')
                    end = new Point(x, y);
                if (data[y][x] == 'a')
                    allAs.Add(new Point(x, y));

                grid[y + 1, x + 1] = data[y][x] switch { 'S' => '`', 'E' => '{', _ => data[y][x] }; // re-map start and end to character before/after in ASCII order
            }
        }
        return (grid, start, end, allAs);
    }

    private static readonly List<Point> neighborCandidates = new() { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
    private static readonly Point offset = new(1, 1);

    private static Func<Point, IList<Point>> BFS(int[,] grid, Point start) {
        var parents = new Dictionary<Point, Point>();
        var queue = new Queue<Point>();

        queue.Enqueue(start);
        while (queue.Count > 0) {
            var point = queue.Dequeue();
            var adjustedPoint = point + offset;
            var neighbors = neighborCandidates.Select(x => x + adjustedPoint).Where(x => grid[x.Y, x.X] != 0).ToList();

            foreach (var neighbor in neighbors) {
                if (parents.ContainsKey(neighbor - offset) ||
                    grid[neighbor.Y, neighbor.X] - grid[adjustedPoint.Y, adjustedPoint.X] > 1) // same/ascending order
                    continue;

                parents[neighbor - offset] = point;
                queue.Enqueue(neighbor - offset);
            }
        }

        return end => GetShortestPath(parents, start, end);
    }

    private static Func<Point, IList<Point>> BFSReverse(int[,] grid, Point start) {
        var parents = new Dictionary<Point, Point>();
        var queue = new Queue<Point>();

        queue.Enqueue(start);
        while (queue.Count > 0) {
            var point = queue.Dequeue();
            var adjustedPoint = point + offset;
            var neighbors = neighborCandidates.Select(x => x + adjustedPoint).Where(x => grid[x.Y, x.X] != 0).ToList();

            foreach (var neighbor in neighbors) {
                if (parents.ContainsKey(neighbor - offset)) continue;
                if (!(grid[neighbor.Y, neighbor.X] - grid[adjustedPoint.Y, adjustedPoint.X] >= -1)) // same/descending order
                    continue;

                parents[neighbor - offset] = point;
                queue.Enqueue(neighbor - offset);
            }
        }

        return end => GetShortestPath(parents, start, end);
    }

    private static IList<Point> GetShortestPath(Dictionary<Point, Point> parents, Point start, Point end) {
        var (path, current) = (new List<Point>(), end); // we'll be building path in reverse (from the end)
        while (!current.Equals(start)) {
            path.Add(current);
            if (!parents.ContainsKey(current)) return new List<Point>(); // bad path
            current = parents[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}