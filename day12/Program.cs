namespace day12;

/// <summary>
/// Hill Climbing Algorithm
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var data = File.ReadAllLines(args[0]);

        Point start = new(0, 0), end = new(0, 0);
        var allAs = new List<Point>(); // needed for part 2
        var grid = new int[data.Length + 2, data[0].Length + 2]; // borders (0s) to make getting neighbors more readable
        for (var i = 0; i < data.Length; i++) {
            for (var j = 0; j < data[0].Length; j++) {
                if (data[i][j] == 'S')
                    start = new Point(i, j);
                if (data[i][j] == 'E')
                    end = new Point(i, j);
                if (data[i][j] == 'a')
                    allAs.Add(new Point(i, j));

                grid[i + 1, j + 1] = data[i][j] switch { 'S' => '`', 'E' => '{', _ => data[i][j] };
            }
        }

        Console.WriteLine($"Part 1: {BFS(grid, start, end)(end).Count - 1} steps");

        var minSteps = allAs.Select(startA => BFS(grid, startA, end)(end).Count - 1).Where(x => x != -1).Min();
        Console.WriteLine($"Part 2: {minSteps} steps");
    }

    private static readonly List<Point> neighborCandidates = new() { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
    private static readonly Point offset = new(1, 1);

    private static Func<Point, IList<Point>> BFS(int[,] grid, Point start, Point end)
    {
        var previous = new Dictionary<Point, Point>();
        var queue = new Queue<Point>();

        queue.Enqueue(start);
        while (queue.Count > 0) {
            var point = queue.Dequeue();
            var adjustedPoint = point + offset;
            var neighbors = neighborCandidates.Select(x => x + adjustedPoint).Where(x => grid[x.Row, x.Col] != 0).ToList();

            foreach (var neighbor in neighbors) {
                if (previous.ContainsKey(neighbor - offset) ||
                    grid[neighbor.Row, neighbor.Col] - grid[adjustedPoint.Row, adjustedPoint.Col] > 1)
                    continue;

                previous[neighbor - offset] = point;
                queue.Enqueue(neighbor - offset);
            }
        }

        IList<Point> shortestPath(Point end)
        {
            var path = new List<Point>();
            var current = end;

            while (!current.Equals(start)) {
                path.Add(current);

                if (!previous.ContainsKey(current)) // bad path
                    return new List<Point>();

                current = previous[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        return shortestPath;
    }
}

internal record Point
{
    public Point(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public int Row;
    public int Col;

    public static Point operator +(Point a, Point b) => new(a.Row + b.Row, a.Col + b.Col);
    public static Point operator -(Point a, Point b) => new(a.Row - b.Row, a.Col - b.Col);
}