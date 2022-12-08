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

        var data = System.IO.File.ReadAllLines(args[0]);

        var arr = new List<List<char>>();
        for (var i = 0; i < data.Length; i++) {
            var row = data[i];
            arr.Add(new List<char>(row.AsEnumerable()));
        }
        Part1(arr);
        Part2(arr);
    }

    private static void Part1(List<List<char>> data)
    {
        var n = 2 * (data.Count + data[0].Count - 2);
        for (var row = 1; row < data.Count - 1; row++) {
            for (var col = 1; col < data[row].Count - 1; col++) {
                var x = data[row][col] - '0';

                var isVisibleTop = true;
                for (var k = row - 1; k >= 0; k--) {
                    var y = data[k][col] - '0';
                    if (y >= x) {
                        isVisibleTop = false;
                        break;
                    }
                }

                var isVisibleBottom = true;
                for (var k = row + 1; k <= data.Count - 1; k++) {
                    var y = data[k][col] - '0';
                    if (y >= x) {
                        isVisibleBottom = false;
                        break;
                    }
                }

                var isVisibleLeft = true;
                for (var k = col - 1; k >= 0; k--) {
                    var y = data[row][k] - '0';
                    if (y >= x) {
                        isVisibleLeft = false;
                        break;
                    }
                }

                var isVisibleRight = true;
                for (var k = col + 1; k <= data[row].Count - 1; k++) {
                    var y = data[row][k] - '0';
                    if (y >= x) {
                        isVisibleRight = false;
                        break;
                    }
                }

                var isVisible = isVisibleTop || isVisibleBottom || isVisibleLeft || isVisibleRight;
                if (isVisible) {
                    n++;
                }
            }
        }

        Console.WriteLine(n);
    }

    private static void Part2(List<List<char>> data)
    {
        var maxScore = 0;
        for (var row = 0; row < data.Count; row++) {
            for (var col = 0; col < data[row].Count; col++) {
                var x = data[row][col] - '0';

                var isVisibleTop = true;
                var nT = 0;
                for (var k = row - 1; k >= 0; k--) {
                    var y = data[k][col] - '0';
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
                for (var k = row + 1; k <= data.Count - 1; k++) {
                    var y = data[k][col] - '0';
                    if (y >= x) {
                        isVisibleBottom = false;
                        nB = Math.Abs(row - k);
                        break;
                    }
                }
                if (isVisibleBottom)
                    nB = data.Count - row - 1;

                var isVisibleLeft = true;
                var nL = 0;
                for (var k = col - 1; k >= 0; k--) {
                    var y = data[row][k] - '0';
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
                for (var k = col + 1; k <= data[row].Count - 1; k++) {
                    var y = data[row][k] - '0';
                    if (y >= x) {
                        isVisibleRight = false;
                        nR = Math.Abs(col - k);
                        break;
                    }
                }
                if (isVisibleRight)
                    nR = data[row].Count - col - 1;

                var score = nT * nB * nL * nR;
                if (score > maxScore) {
                    maxScore = score;
                }
            }
        }

        Console.WriteLine(maxScore);
    }
}