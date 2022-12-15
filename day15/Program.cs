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
            Console.WriteLine("Usage: {0} <file> <n> <mapSize>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
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

    private static int Part1(List<(Point sensor, Point beacon)> data, int n)
    {
        var xs = new List<Interval>();
        foreach (var x in data) {
            var coveredXs = CoveredXs(x, n);
            if (coveredXs != null)
                xs.Add(coveredXs);
        }

        var beacons = data
            .Where(x => x.beacon.Y == n).Select(x => x.beacon.X)
            .Distinct().Count();

        xs = Interval.Merge(xs);
        var sum = 0;
        foreach (var interval in xs) {
            sum += interval.End - interval.Start + 1;
        }
        return sum - beacons;
    }

    private static long Part2(List<(Point sensor, Point beacon)> data, int mapSize)
    {
        for (var n = 0; n <= mapSize; n++) {
            var xs = new List<Interval>();
            foreach (var x in data) {
                var coveredXs = CoveredXs(x, n);
                if (coveredXs != null)
                    xs.Add(coveredXs);
            }
            xs = Interval.Merge(xs);
            // for only one value of n we expect two intervals, otherwise they should be all 1
            // the missing number is at the end of first interval plus one (or second's start minus one)
            if (xs.Count == 2) {
                xs.Sort();
                return 4_000_000L * (xs[0].End + 1) + n;
            }
        }

        return long.MinValue;
        ;
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

public record Interval : IComparable<Interval>
{
    public Interval(int start, int end)
    {
        Start = start;
        End = end;
    }
    public int Start { get; set; }
    public int End { get; set; }

    public int CompareTo(Interval? other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));
        return Start < other.Start ? -1 : Start > other.Start ? 1 : 0;
    }

    public static List<Interval> Merge(List<Interval> intervals)
    {
        if (intervals is null)
            throw new ArgumentNullException(nameof(intervals));
        if (intervals.Count == 0)
            return new List<Interval>();

        var s = new Stack<Interval>();

        intervals.Sort();
        s.Push(intervals[0]);
        for (var i = 1; i < intervals.Count; i++) {
            var top = s.Peek();

            if (top.End < intervals[i].Start) {
                s.Push(intervals[i]);
            } else if (top.End < intervals[i].End) {
                top.End = intervals[i].End;
                s.Pop();
                s.Push(top);
            }
        }

        var l = new List<Interval>();
        while (s.Any()) {
            l.Add(s.Pop());
        }
        return l;
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