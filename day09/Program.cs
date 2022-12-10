namespace day09;

/// <summary>
/// Rope Bridge
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

        var s = new Snake(2);
        var v = new HashSet<(int X, int Y)> { (0, 0) };
        foreach (var cmdInput in data) {
            v = v.Union(s.Move(cmdInput[0], int.Parse(cmdInput[2..]))).ToHashSet();
        }

        Console.WriteLine(v.Count);

        var s10 = new Snake(10);
        var v10 = new HashSet<(int X, int Y)> { (0, 0) };
        foreach (var cmdInput in data) {
            v10 = v10.Union(s10.Move(cmdInput[0], int.Parse(cmdInput[2..]))).ToHashSet();
        }

        Console.WriteLine(v10.Count);

        //Visualize(v);
    }

    private static void Visualize(HashSet<(int X, int Y)> v)
    {
        // calculate what to add to get to 0,0-based coordates
        var minX = v.Min(x => x.X);
        var minY = v.Min(x => x.Y);

        var vv = v.Select(point => (point.X + (-minX), point.Y + (-minY))).ToHashSet();
        for (var y = 0; y <= vv.Max(x => x.Item2); y++) {
            for (var x = 0; x <= vv.Max(x => x.Item1); x++) {
                Console.Write(vv.Contains((x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }
}

internal class Snake
{
    public Snake(int n)
    {
        var tmp = new Part() { // start with tail
            Name = (n - 1).ToString(),
            X = 0,
            Y = 0,
            Previous = null
        };
        var tail = tmp;

        for (var i = n - 2; i >= 0; i--) {
            var x = new Part() {
                Name = i == 0 ? "H" : i.ToString(),
                X = 0,
                Y = 0,
                Previous = tmp
            };
            tmp = x;
        }

        Head = tmp;
        Tail = tail;
    }

    public Part Head { get; set; }
    public Part Tail { get; set; }

    public HashSet<(int X, int Y)> Move(char cmd, int steps)
    {
        var visited = new HashSet<(int X, int Y)>();

        for (var i = 1; i <= steps; i++) {
            var p = Head;
            switch (cmd) {
                case 'R':
                    p.X++;
                    break;
                case 'L':
                    p.X--;
                    break;
                case 'U':
                    p.Y++;
                    break;
                case 'D':
                    p.Y--;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            while (p.Previous != null) {
                if (Math.Abs(p.X - p.Previous.X) >= 2 ||
                    Math.Abs(p.Y - p.Previous.Y) >= 2) {
                    p.Previous.X +=
                        ((p.X != p.Previous.X) ? 1 : 0)
                        * Math.Sign(p.X - p.Previous.X);
                    p.Previous.Y +=
                        ((p.Y != p.Previous.Y) ? 1 : 0)
                        * Math.Sign(p.Y - p.Previous.Y);
                }
                p = p.Previous;
            }
            visited.Add((Tail.X, Tail.Y));
        }

        return visited;
    }

    internal class Part
    {
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Part? Previous { get; set; }
    }
}