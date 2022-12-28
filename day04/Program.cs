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

            var input = File.ReadAllLines(args[0]);
            var part1Count = input
                .Select(line => Parse(line))
                .Where(x => x.interval1.IsFullyContained(x.interval2) || x.interval2.IsFullyContained(x.interval1))
                .Count();

            var part2Count = input
                .Select(x => Parse(x))
                .Count(x => x.interval1.IsOverlap(x.interval2) || x.interval2.IsOverlap(x.interval1));

            Console.WriteLine(part1Count);
            Console.WriteLine(part2Count);
        }

        private static (Interval interval1, Interval interval2) Parse(string s)
        {
            var afterComma = s.IndexOf(',') + 1;
            var (i1s, i2s) = (s.Substring(0, afterComma - 1), s.Substring(afterComma));

            var afterDash1 = i1s.IndexOf('-') + 1;
            var afterDash2 = i2s.IndexOf('-') + 1;

            var i1 = new Interval(int.Parse(i1s[..(afterDash1 - 1)]), int.Parse(i1s[afterDash1..]));
            var i2 = new Interval(int.Parse(i2s[..(afterDash2 - 1)]), int.Parse(i2s[afterDash2..]));
            return (i1, i2);
        }
    }
}