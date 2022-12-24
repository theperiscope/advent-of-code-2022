namespace day23;

/// <summary>
/// Unstable Diffusion
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var input = File.ReadLines(args[0]).ToList();
        var lineLength = input[0].Length;

        var elves = new HashSet<Point>();
        for (var y = 0; y < input.Count; y++) {
            var line = input[y];
            for (var x = 0; x < lineLength; x++) {
                if (line[x] == '#')
                    elves.Add(new(x, y));
            }
        }

        var start = 0;
        var moves = 0;
        var answer2 = 0;

        while (true) {

            if (moves == 10) {
                var minX = elves.Select(a => a.X).Min();
                var maxX = elves.Select(a => a.X).Max();
                var minY = elves.Select(a => a.Y).Min();
                var maxY = elves.Select(a => a.Y).Max();

                var answer1 = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count();
                Console.WriteLine(answer1);
            }

            var proposals = new Dictionary<Point, List<Point>>();

            // first half of round: make proposals
            foreach (var elf in elves) {
                var elvesAround = AllCandidates.Select(x => x + elf).Count(x => elves.Contains(x));
                if (elvesAround == 0)
                    continue;

                var done = false;
                // shuffling of order of 4 conditions using a mod counter
                for (var p = start; p < start + 4; p++) {

                    if (!done && p % 4 == 0) {
                        var n = NorthCandidates.Select(x => x + elf).Count(x => elves.Contains(x));
                        if (n == 0) {
                            var target = new Point(elf.X, elf.Y - 1);
                            if (!proposals.ContainsKey(target))
                                proposals.Add(target, new List<Point>());
                            proposals[target].Add(elf);
                            done = true;
                            continue;
                        }
                    }

                    if (!done && p % 4 == 1) {
                        var s = SouthCandidates.Select(x => x + elf).Count(x => elves.Contains(x));
                        if (s == 0) {
                            var target = new Point(elf.X, elf.Y + 1);
                            if (!proposals.ContainsKey(target))
                                proposals.Add(target, new List<Point>());
                            proposals[target].Add(elf);
                            done = true;
                            continue;
                        }
                    }

                    if (!done && p % 4 == 2) {
                        var w = WestCandidates.Select(x => x + elf).Count(x => elves.Contains(x));
                        if (w == 0) {
                            var target = new Point(elf.X - 1, elf.Y);
                            if (!proposals.ContainsKey(target))
                                proposals.Add(target, new List<Point>());
                            proposals[target].Add(elf);
                            done = true;
                            continue;
                        }
                    }

                    if (!done && p % 4 == 3) {
                        var e = EastCandidates.Select(x => x + elf).Count(x => elves.Contains(x));
                        if (e == 0) {
                            var target = new Point(elf.X + 1, elf.Y);
                            if (!proposals.ContainsKey(target))
                                proposals.Add(target, new List<Point>());
                            proposals[target].Add(elf);
                            done = true;
                            continue;
                        }
                    }
                }

            }

            if (proposals.Count == 0) {
                break;
            }

            // second half of round: evaluate proposals and move elf to new location (the key)
            var validProposals = proposals.Where(x => x.Value.Count == 1);
            foreach (var proposal in validProposals) {
                elves.Remove(proposal.Value.First());
                elves.Add(proposal.Key);
            }

            start = (start + 1) % 4;
            moves++;
        }

        answer2 = moves + 1;
        Console.WriteLine(answer2);
    }

    private static readonly List<Point> NorthCandidates = new() { new(-1, -1), new(0, -1), new(1, -1), };
    private static readonly List<Point> SouthCandidates = new() { new(-1, 1), new(0, 1), new(1, 1), };
    private static readonly List<Point> WestCandidates = new() { new(-1, -1), new(-1, 0), new(-1, 1), };
    private static readonly List<Point> EastCandidates = new() { new(1, -1), new(1, 0), new(1, 1), };
    private static readonly List<Point> AllCandidates = NorthCandidates.Union(SouthCandidates).Union(WestCandidates).Union(EastCandidates).ToList();

}

internal record Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X;
    public int Y;

    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
}