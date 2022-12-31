using shared;
using System.Diagnostics;

namespace day23;

/// <summary>
/// Unstable Diffusion
/// </summary>
internal class Program
{
    private static readonly object dictLock = new object();

    static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var sw = new Stopwatch(); sw.Start();
        var input = File.ReadLines(args[0]).ToList();
        var lineLength = input[0].Length;

        var elves = new HashSet<Point>();
        for (var y = 0; y < input.Count; y++) {
            for (var x = 0; x < lineLength; x++) {
                if (input[y][x] == '#')
                    elves.Add(new(x, y));
            }
        }

        var (start, moves) = (0, 0);
        var answer2 = 0;

        var proposals = new Dictionary<Point, List<Point>>();

        while (true) {

            if (moves == 10) {
                var (minX, maxX) = (elves.Select(a => a.X).Min(), elves.Select(a => a.X).Max());
                var (minY, maxY) = (elves.Select(a => a.Y).Min(), elves.Select(a => a.Y).Max());
                var answer1 = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count(); // area = a*b, minus the elves
                Console.WriteLine(answer1);
            }

            proposals.Clear();

            // first half of round: make proposals
            Parallel.ForEach(elves, (elf, state) => {
                var elvesAround = AllCandidates.Select(x => x + elf).Any(x => elves.Contains(x));
                if (!elvesAround)
                    return; // continue
                var nn = NorthCandidates.Select(x => x + elf).Any(x => elves.Contains(x));
                var ss = SouthCandidates.Select(x => x + elf).Any(x => elves.Contains(x));
                var ww = WestCandidates.Select(x => x + elf).Any(x => elves.Contains(x));
                var ee = EastCandidates.Select(x => x + elf).Any(x => elves.Contains(x));

                var done = false;
                // shuffling of order of 4 conditions using a mod counter
                for (var p = start; p < start + 4; p++) {
                    if (done) break;
                    Point? target = null;
                    if (!done && p % 4 == 0 && target == null) {
                        if (!nn) target = new Point(elf.X, elf.Y - 1);
                    } else if (!done && p % 4 == 1 && target == null) {
                        if (!ss) target = new Point(elf.X, elf.Y + 1);
                    } else if (!done && p % 4 == 2 && target == null) {
                        if (!ww) target = new Point(elf.X - 1, elf.Y);
                    } else if (!done && p % 4 == 3 && target == null) {
                        if (!ee) target = new Point(elf.X + 1, elf.Y);
                    }

                    if (target != null) { // to reduce repeating code in conditions above
                        lock (dictLock) {
                            if (!proposals.ContainsKey(target)) {
                                proposals.Add(target, new List<Point> { elf });
                            } else {
                                if (proposals[target].Count <= 1) // we don't need any extras
                                    proposals[target].Add(elf);
                            }
                        }
                        done = true;
                        return; // continue
                    }
                }
            });

            if (proposals.Count == 0) break;

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
        sw.Stop(); Console.WriteLine(sw.Elapsed.TotalMilliseconds);
    }

    private static readonly List<Point> NorthCandidates = new() { new(-1, -1), new(0, -1), new(1, -1), };
    private static readonly List<Point> SouthCandidates = new() { new(-1, 1), new(0, 1), new(1, 1), };
    private static readonly List<Point> WestCandidates = new() { new(-1, -1), new(-1, 0), new(-1, 1), };
    private static readonly List<Point> EastCandidates = new() { new(1, -1), new(1, 0), new(1, 1), };
    private static readonly List<Point> AllCandidates = NorthCandidates.Union(SouthCandidates).Union(WestCandidates).Union(EastCandidates).ToList();
}