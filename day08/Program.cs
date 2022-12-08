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

        var data = File.ReadAllLines(args[0]);

        var arr = new List<List<char>>();
        for (var i = 0; i < data.Length; i++) {
            var row = data[i];
            arr.Add(new List<char>(row.AsEnumerable()));
        }

        var n = 0;
        var maxScore = 0;
        for (var row = 0; row < arr.Count; row++) {
            for (var col = 0; col < arr[row].Count; col++) {
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

        Console.WriteLine($"Part 1: {n}");
        Console.WriteLine($"Part 2: {maxScore}");
    }
}