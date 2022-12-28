using shared;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace day15;

/// <summary>
/// Beacon Exclusion Zone
/// </summary>
internal class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 3) {
            // day15 input-short.txt 10 20
            // day15 input-short.txt 2000000 4000000
            Console.WriteLine("Usage: {0} <file> <rowNumber> <mapSize>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var lines = File.ReadAllLines(args[0]);
        var regex = new Regex(@"x=(-*\d+),\sy=(-*\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        List<(Point sensor, Point beacon)> data = lines.Select(s => regex.Matches(s))
            .Select(m =>
                (
                    new Point(int.Parse(m[0].Groups[1].Value), int.Parse(m[0].Groups[2].Value)),
                    new Point(int.Parse(m[1].Groups[1].Value), int.Parse(m[1].Groups[2].Value))
                )).ToList();

        Console.WriteLine(Part1(data, int.Parse(args[1])));
        Console.WriteLine(Part2(data, int.Parse(args[2])));
    }

    private static int Part1(List<(Point sensor, Point beacon)> data, int row)
    {
        var xs = new List<Interval>();
        foreach (var sensorBeacon in data) {
            var coveredXs = CoveredXs(sensorBeacon, row);
            if (coveredXs != null)
                xs.Add(coveredXs);
        }

        var beacons = data
            .Where(x => x.beacon.Y == row).Select(x => x.beacon.X)
            .Distinct().Count(); // distinct as there are overlapping beacons

        return Interval.Merge(xs).Sum(i => i.End - i.Start + 1) - beacons;
    }

    private static long Part2(List<(Point sensor, Point beacon)> data, int mapSize)
    {
        var (list, q) = (Enumerable.Range(0, mapSize + 1).ToList(), new ConcurrentQueue<long>());

        Parallel.ForEach(list, (n, state) => {
            var xs = new List<Interval>();
            foreach (var x in data) {
                var coveredXs = CoveredXs(x, n);
                if (coveredXs != null)
                    xs.Add(coveredXs);
            }
            xs = Interval.Merge(xs);
            // for only one value of row we expect two intervals, otherwise they should be all 1
            // the missing number is at the end of first interval plus one (or second's start minus one)
            if (xs.Count == 2) {
                xs.Sort();
                q.Enqueue(4_000_000L * (xs[0].End + 1) + n);
                state.Stop();
            }
        });

        q.TryDequeue(out var result);
        return result;
    }

    private static Interval? CoveredXs((Point sensor, Point beacon) s, int n)
    {
        var coverageAtRow = Math.Max(0, (s.sensor.ManhattanDistanceTo(s.beacon) + 1) * 2 - 1 - (Math.Abs(s.sensor.Y - n) * 2));
        if (coverageAtRow <= 0)
            return null;

        var startX = s.sensor.X - ((coverageAtRow - 1) / 2);
        var interval = new Interval(startX, startX + coverageAtRow - 1);

        return interval;
    }
}

public record Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X;
    public int Y;

    public int ManhattanDistanceTo(Point p) => Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
}