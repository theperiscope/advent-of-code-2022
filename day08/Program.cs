using shared;

namespace day08;

/// <summary>
/// Treetop Tree House
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
        Console.WriteLine($"Parsing: {parseResults[0].Count}x{parseResults[0][0].Count} in {parseTimings[0]}ms");

        var (results, timings) = Perf.BenchmarkTime(() => Solve(parseResults[0]));

        // (parseResults[0].Count * 2 + ((parseResults[0][0].Count - 2)) * 2) is the exact number of visible trees by definition along the border
        // Solve loop skips those
        Console.WriteLine($"Part 1 : {results[0].n + (parseResults[0].Count * 2 + ((parseResults[0][0].Count - 2)) * 2)} in {timings[0]}ms");
        Console.WriteLine($"Part 2 : {results[0].maxScore}  using same timing");
    }

    private static List<List<char>> Parse(string fileName)
    {
        return File.ReadAllLines(fileName).TrimTrailingEndOfLine().Select(line => line.AsEnumerable().ToList()).ToList();
    }

    private static (int n, int maxScore) Solve(List<List<char>> arr)
    {

        var n = 0;
        var maxScore = 0;
        for (var row = 1; row < arr.Count - 1; row++) {
            for (var col = 1; col < arr[row].Count - 1; col++) {
                var x = arr[row][col];

                var isVisibleTop = true;
                var nT = 0;
                for (var k = row - 1; k >= 0; k--) {
                    var y = arr[k][col];
                    if (y >= x) {
                        isVisibleTop = false;
                        nT = Math.Abs(row - k);
                        break;
                    }
                }
                if (isVisibleTop)
                    nT = row;

                var isVisibleBottom = true;
                var nB = 0;
                for (var k = row + 1; k <= arr.Count - 1; k++) {
                    var y = arr[k][col];
                    if (y >= x) {
                        isVisibleBottom = false;
                        nB = Math.Abs(row - k);
                        break;
                    }
                }
                if (isVisibleBottom)
                    nB = arr.Count - row - 1;

                var isVisibleLeft = true;
                var nL = 0;
                for (var k = col - 1; k >= 0; k--) {
                    var y = arr[row][k];
                    if (y >= x) {
                        isVisibleLeft = false;
                        nL = Math.Abs(col - k);
                        break;
                    }
                }
                if (isVisibleLeft)
                    nL = col;

                var isVisibleRight = true;
                var nR = 0;
                for (var k = col + 1; k <= arr[row].Count - 1; k++) {
                    var y = arr[row][k];
                    if (y >= x) {
                        isVisibleRight = false;
                        nR = Math.Abs(col - k);
                        break;
                    }
                }
                if (isVisibleRight)
                    nR = arr[row].Count - col - 1;

                var isVisible = isVisibleTop || isVisibleBottom || isVisibleLeft || isVisibleRight;
                if (isVisible) {
                    n++;
                }

                var score = nT * nB * nL * nR;
                if (score > maxScore) {
                    maxScore = score;
                }
            }
        }

        return (n, maxScore);
    }
}