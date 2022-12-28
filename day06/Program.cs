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
        for (var i = 0; i < s.Length - n; i++) {
            var ss = s.Substring(i, n);
            if (IsAllUniqueLowercaseCharacters(ss))
                return i + n;
        }
        return -1;
    }

    private static bool IsAllUniqueLowercaseCharacters(string s)
    {
        var isRepeat = 0; // 32 bits is enough for the 26 letters we have
        for (var i = 0; i < s.Length; i++) {
            var idx = s[i] - 'a';
            if ((isRepeat & (1 << idx)) > 0)
                return false;
            isRepeat |= (1 << idx); // flip bit "idx" to 1
        }
        return true;
    }
}