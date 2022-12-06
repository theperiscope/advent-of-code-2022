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

        var data = File.ReadAllLines(args[0]);

        Part1(data);
        Part2(data);
    }

    private const int StartOfPacket = 4;
    private const int StartOfMessage = 14;

    private static void Part1(string[] data)
    {
        foreach (var s in data)
            Console.WriteLine(GetMarkerPosition(s, StartOfPacket));
    }

    private static void Part2(string[] data)
    {
        foreach (var s in data)
            Console.WriteLine(GetMarkerPosition(s, StartOfMessage));
    }

    /// <summary>
    /// Finds marker position based on number of consecutive different characters
    /// </summary>
    private static int? GetMarkerPosition(string s, int n)
    {
        return s
            .Select((ch, i) => new { i, n = s.Substring(i, n).Distinct().Count() })
            .Where(el => el.n == n)
            .Select(el => el.i + n).FirstOrDefault();
    }
}