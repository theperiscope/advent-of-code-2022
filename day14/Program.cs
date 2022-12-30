using shared;

namespace day14;

/// <summary>
/// Regolith Reservoir
/// </summary>
internal class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var lines = File.ReadAllLines(args[0]);
        var map = new byte[1000, 200];
        for (var i = 0; i < map.GetLength(0); i++)
            for (var j = 0; j < map.GetLength(1); j++)
                map[i, j] = (byte)'.';

        var points = new List<Point>();

        foreach (var line in lines) {
            var l = line.Split(" -> ");
            for (var i = 0; i < l.Length - 1; i++) {
                var (l1, l2) = (l[i].Split(','), l[i + 1].Split(','));
                var (start, end) = (new Point(int.Parse(l1[0]), int.Parse(l1[1])), new Point(int.Parse(l2[0]), int.Parse(l2[1])));
                points.AddRange(new[] { start, end });

                if (start.X == end.X) { // vertical line
                    for (var j = Math.Min(start.Y, end.Y); j <= Math.Max(start.Y, end.Y); j++) {
                        map[start.X, j] = (byte)'#';
                    }
                } else if (start.Y == end.Y) { // horizontal
                    for (var j = Math.Min(start.X, end.X); j <= Math.Max(start.X, end.X); j++) {
                        map[j, start.Y] = (byte)'#';
                    }
                } else
                    throw new InvalidOperationException();
            }
        }
        var minX = points.Select(p => p.X).Min();
        var maxX = points.Select(p => p.X).Max();
        var maxY = points.Select(p => p.Y).Max();

        var isAbyssPart1 = (Point p) => (p.X < minX || p.X > maxX) && p.Y > maxY;

        Part1((byte[,])map.Clone(), isAbyssPart1);
        Part2((byte[,])map.Clone(), maxY);
    }

    private static void Part1(byte[,] map, Func<Point, bool> isAbyss)
    {
        var n = 0;
        do {
            var success = DropSand(map, isAbyss, out var p);
            if (!success)
                break;
            n++;
            map[p.X, p.Y] = (byte)'o';
        } while (true);
        Console.WriteLine(n);
    }

    private static void Part2(byte[,] map, int maxY)
    {
        var abyss = (Point p) => map[500, 0] != '.';

        for (var j = 0; j < map.GetLength(0); j++)
            map[j, maxY + 2] = (byte)'#';

        var n = 0;
        do {
            var success = DropSand(map, abyss, out var p);
            if (!success)
                break;
            n++;
            map[p.X, p.Y] = (byte)'o';
        } while (true);
        Console.WriteLine(n);
    }

    private static Point NullPoint = new(int.MinValue, int.MinValue);

    private static bool DropSand(byte[,] map, Func<Point, bool> isAbyss, out Point p)
    {
        p = NullPoint;
        var origin = new Point(500, 0);

        for (; ; ) {
            var newY = origin.Y;
            do {
                newY++;
            } while (!isAbyss(new(origin.X, newY)) && map[origin.X, newY] == '.');

            var f = new Point(origin.X, newY - 1);

            if (isAbyss(f))
                return false;

            var diagLeft = new Point(f.X - 1, f.Y + 1);
            var diagRight = new Point(f.X + 1, f.Y + 1);

            var leftFilled = map[diagLeft.X, diagLeft.Y] != '.';
            var rightFilled = map[diagRight.X, diagRight.Y] != '.';

            if (leftFilled && rightFilled) {
                p = f;
                return true;
            }

            origin = !leftFilled ? diagLeft : diagRight;
        }
    }
}
