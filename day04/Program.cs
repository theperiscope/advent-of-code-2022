using shared;

namespace day04
{
    /// <summary>
    /// Camp Cleanup
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var part1Count = Utils.ReadAllLinesFrom(args[0])
                .Select(x => ParseInputLine(x))
                .Where(x => IsFullyContained(x.interval1, x.interval2))
                .Count();

            var part2Count = Utils.ReadAllLinesFrom(args[0])
                .Select(x => ParseInputLine(x))
                .Where(x => IsOverlap(x.interval1, x.interval2) != null)
                .Count();

            Console.WriteLine(part1Count);
            Console.WriteLine(part2Count);
        }

        private static ((int start, int end) interval1, (int start, int end) interval2) ParseInputLine(string s)
        {
            // TODO: add validity checks and asserts, currently assumes valid input/formats
            var afterComma = s.IndexOf(',') + 1;
            var (e1, e2) = (s.Substring(0, afterComma - 1), s.Substring(afterComma));

            var afterDash1 = e1.IndexOf('-') + 1;
            var afterDash2 = e2.IndexOf('-') + 1;

            var (e1Start, e1End) = (int.Parse(e1.Substring(0, afterDash1 - 1)), int.Parse(e1.Substring(afterDash1)));
            var (e2Start, e2End) = (int.Parse(e2.Substring(0, afterDash2 - 1)), int.Parse(e2.Substring(afterDash2)));

            return ((e1Start, e1End), (e2Start, e2End));
        }

        private static bool IsFullyContained((int start, int end) interval1, (int start, int end) interval2)
        {
            return
                (interval1.start >= interval2.start && interval1.end <= interval2.end) ||
                (interval2.start >= interval1.start && interval2.end <= interval1.end);
        }

        private static (int overlapStart, int overlapEnd)? IsOverlap((int start, int end) interval1, (int start, int end) interval2)
        {
            return interval2.start > interval1.end || interval1.start > interval2.end
                ? null
                : (Math.Max(interval1.start, interval2.start), Math.Min(interval1.end, interval2.end));
        }
    }
}