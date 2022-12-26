using shared;

internal class Program
{
    const int maxWidth = 7;

    private static void Main(string[] args)
    {
        if (args.Length != 2) {
            Console.WriteLine("Usage: {0} <file> <target>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var lines = File.ReadAllLines(args[0]);
        var target = long.Parse(args[1]);
        if (target != 2_022L && target != 1_000_000_000_000L)
            throw new InvalidOperationException(nameof(target));

        var hBar = new Shape(new Point[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) });
        var plus = new Shape(new Point[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2) });
        var rightL = new Shape(new Point[] { new(2, 0), new(2, 1), new(2, 2), new(1, 2), new(0, 2) });
        var vBar = new Shape(new Point[] { new(0, 0), new(0, 1), new(0, 2), new(0, 3) });
        var box = new Shape(new Point[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) });

        var shapes = new List<Shape> { hBar, plus, rightL, vBar, box };
        var i = 0L;
        var j = 0L;
        var input = lines[0];

        var board = new List<char[]> { new string('X', maxWidth).ToArray() };

        var heightChange = new List<int>();
        var patternScanDone = false;
        var NN = target == 2022 ? 500L : 5_000L;
        var pattern = "";
        var patternStart = 0L;
        var nextPatternStart = 0L;
        var canSkip = 0L;

        while (true) {

            var shape = shapes[(int)(i % 5L)];
            var (sX, sY) = (2, 0);
            var isDown = false;

            var h = board.Count - 1;
            board = newBoard(board, shape);
            do {
                var direction = input[(int)j];
                ((sX, sY), isDown) = Move(board, direction, shape, (sX, sY));
                j = (j + 1) % input.Length;
            } while (!isDown);

            board = board.Where(x => new string(x) != ".......").ToList();
            var newH = board.Count - 1;
            heightChange.Add(newH - h);

            if (heightChange.Count > NN && !patternScanDone) {
                patternScanDone = true;
                var s = string.Join("", heightChange.Select(h => h.ToString()));
                var x = FindPattern(s, 0);
                pattern = x.repeatText;
                patternStart = x.patternStart;
                if (pattern.Length > 1) { // there is no pattern
                    nextPatternStart = ((i + patternStart - 1) / pattern.Length) * pattern.Length;
                    canSkip = (target - nextPatternStart) / pattern.Length;
                } else {
                    Console.WriteLine("No pattern found.");
                }
            }

            if (patternScanDone && i == nextPatternStart && patternScanDone) {
                i += pattern.Length * canSkip + 1;
            } else {
                i++;
            }

            if (i >= target)
                break;
        }

        var patternSum = pattern.Length > 1 ? pattern.Select(c => c - '0').Sum() : 0;
        var n = (target - nextPatternStart) / pattern.Length;
        var sum = n * patternSum + heightChange.Sum();

        Console.WriteLine($"Part {(target == 2022 ? "1" : "2")}: {sum}");
    }

    private static (string repeatText, int patternStart) FindPattern(string s, int start)
    {
        var patternStart = start;
        var patternLength = 1;
        var bestPattern = "";
        var bestN = 0;
        while (patternStart < s.Length / 2 && patternLength <= s.Length / 2) {
            var pattern = s.Substring(patternStart, patternLength);
            var n = (s.Count(pattern) - 1) * pattern.Length;
            if (n > bestN) {
                bestN = n;
                bestPattern = pattern;
                patternLength++;
            } else if (n == 0) {
                bestN = 0;
                patternStart++;
                patternLength = 1;
            } else {
                patternLength++;
            }
        }
        return (bestPattern, s.IndexOf(bestPattern));
    }

    private static List<char[]> newBoard(List<char[]> board, Shape s)
    {
        var newBoard = new List<char[]>();
        for (var i = 0; i < 3 + s.Height; i++) {
            newBoard.Add(new string('.', maxWidth).ToArray());
        }
        newBoard.AddRange(board);
        return newBoard;
    }

    private static ((int startX, int startY), bool isDown) Move(List<char[]> board, char direction, Shape s, (int startX, int startY) start)
    {
        var newStart = start;
        var isDown = false;
        switch (direction) {
            case '>':
                if (s.Points.All(p => newStart.startX + p.X + 1 < maxWidth) &&
                    s.Points.All(p => board[newStart.startY + p.Y][newStart.startX + p.X + 1] == '.'))
                    newStart.startX++;
                break;
            case '<':
                if (s.Points.All(p => newStart.startX + p.X - 1 >= 0) &&
                    s.Points.All(p => board[newStart.startY + p.Y][newStart.startX + p.X - 1] == '.'))
                    newStart.startX--;
                break;
            default:
                throw new InvalidOperationException();
        }
        var atBottom = newStart.startY > board.Count - 1;
        var atAnotherPiece = s.Points.Any(p => board[p.Y + newStart.startY + 1][p.X + newStart.startX] != '.');

        if (!atBottom && !atAnotherPiece) {
            newStart.startY++;
            atBottom = newStart.startY > board.Count - 1;
            atAnotherPiece = s.Points.Any(p => board[p.Y + newStart.startY][p.X + newStart.startX] != '.');
            if (atBottom || atAnotherPiece) {
                isDown = true;
                foreach (var p in s.Points) {
                    board[p.Y + newStart.startY][p.X + newStart.startX] = '#';
                }

            }
        } else {
            isDown = true;
            foreach (var p in s.Points) {
                board[p.Y + newStart.startY][p.X + newStart.startX] = '#';
            }
        }

        return (newStart, isDown);
    }
}

public record Shape
{
    private int w, h;
    public Shape() { Points = new List<Point>(); w = h = 0; }
    public Shape(IEnumerable<Point> points)
    {
        Points = new List<Point>(points);
        w = Points.Select(p => p.X).Max() - Points.Select(p => p.X).Min() + 1;
        h = Points.Select(p => p.Y).Max() - Points.Select(p => p.Y).Min() + 1;
    }
    public IReadOnlyList<Point> Points { get; set; }

    public int Width => w;
    public int Height => h;
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
}