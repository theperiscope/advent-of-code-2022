using shared;

namespace day06;

/// <summary>
/// Tuning Trouble
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var input = File.ReadAllText(args[0]).TrimTrailingEndOfLine();

        var (results, timings) = Perf.BenchmarkTime<int>(() => Part1(input));
        Console.WriteLine($"Part 1: {results[0]} in {timings[0]}ms");

        (results, timings) = Perf.BenchmarkTime<int>(() => Part2(input));
        Console.WriteLine($"Part 2: {results[0]} in {timings[0]}ms");
    }

    private const int StartOfPacket = 4;
    private const int StartOfMessage = 14;

    private static int Part1(string s) => GetMarkerPosition(s, StartOfPacket);
    private static int Part2(string s) => GetMarkerPosition(s, StartOfMessage);

    /// <summary>
    /// Finds marker position based on number of consecutive different characters
    /// </summary>
    private static int GetMarkerPosition(string s, int n)
    {
        return s
            .Select((ch, i) => new { i, n = s.Substring(i, n).Distinct().Count() })
            .Where(el => el.n == n)
            .Select(el => el.i + n).First();
    }
}