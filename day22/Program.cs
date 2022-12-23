using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var map = File.ReadLines(args[0]).ToList();
        var instructions = map[map.Count - 1];
        map.RemoveAt(map.Count - 1); // remove instructions
        map.RemoveAt(map.Count - 1); // and blank line, leaving only map
        var maxLineLength = map.Select(line => line.Length).Max();
        var spaces = new string(' ', maxLineLength);
        map = map.Select(line => string.Concat(line, spaces.AsSpan(0, maxLineLength - line.Length))).ToList(); // make column lengths same

        var rowLengths = map.Select(line => line.Trim().Length).ToArray();
        var rowStartX = map.Select((line, i) => line.TrimEnd().LastIndexOf(' ') + 1).ToArray();
        var columnHeights = new int[map[0].Length];
        var columnStartY = new int[map[0].Length];
        for (var i = 0; i < map[0].Length; i++) {
            var s = new string(map.Select(line => line[i]).ToArray());
            columnHeights[i] = s.Trim().Length;
            columnStartY[i] = s.TrimEnd().LastIndexOf(' ') + 1;
        }

        (int x, int y) cursor = (map[0].IndexOf('.'), 0);
        var dir = '>';
        var ip = 0;
        while (true) {
            var sb = new StringBuilder();
            while (ip <= instructions.Length - 1 && instructions[ip] >= '0' && instructions[ip] <= '9') {
                sb.Append(instructions[ip]);
                ip++;
            }
            var n = int.Parse(sb.ToString());

            switch (dir) {
                case '>':
                    cursor = MoveRight(map, cursor, n, rowLengths, rowStartX);
                    break;
                case '<':
                    cursor = MoveLeft(map, cursor, n, rowLengths, rowStartX);
                    break;
                case '^':
                    cursor = MoveUp(map, cursor, n, columnHeights, columnStartY);
                    break;
                case 'v':
                    cursor = MoveDown(map, cursor, n, columnHeights, columnStartY);
                    break;
                default:
                    break;
            }

            if (ip >= instructions.Length - 1)
                break;

            var instructionDirection = instructions[ip];
            ip++;
            dir = Turn(dir, instructionDirection == 'L');
        }

        Console.WriteLine(1000 * (cursor.y + 1) + (4 * (cursor.x + 1)) + (dir switch { '>' => 0, 'v' => 1, '<' => 2, '^' => 3, _ => throw new InvalidOperationException() }));

    }

    private static (int x, int y) MoveLeft(List<string> map, (int x, int y) cursor, int n, int[] rowLengths, int[] rowStartX)
    {
        var currentRowLength = rowLengths[cursor.y];
        var startX = rowStartX[cursor.y];
        for (var i = 1; i <= n; i++) {
            var next = startX + Mod(cursor.x - 1, currentRowLength);
            if (map[cursor.y][next] == '#') {
                break;
            } else
                cursor.x = next;
        }
        return cursor;
    }

    private static (int x, int y) MoveRight(List<string> map, (int x, int y) cursor, int n, int[] rowLengths, int[] rowStartX)
    {
        var currentRowLength = rowLengths[cursor.y];
        var startX = rowStartX[cursor.y];
        for (var i = 1; i <= n; i++) {
            var next = startX + Mod(cursor.x + 1, currentRowLength);
            if (map[cursor.y][next] == '#') {
                break;
            } else
                cursor.x = next;
        }
        return cursor;
    }

    private static (int x, int y) MoveUp(List<string> map, (int x, int y) cursor, int n, int[] columnHeights, int[] columnStartY)
    {
        var currentColumnLength = columnHeights[cursor.x];
        var startY = columnStartY[cursor.x];
        for (var i = 1; i <= n; i++) {
            var next = startY + Mod(cursor.y - 1, currentColumnLength);
            if (map[next][cursor.x] == '#') {
                break;
            } else
                cursor.y = next;
        }
        return cursor;
    }

    private static (int x, int y) MoveDown(List<string> map, (int x, int y) cursor, int n, int[] columnHeights, int[] columnStartY)
    {
        var currentColumnLength = columnHeights[cursor.x];
        var startY = columnStartY[cursor.x];
        for (var i = 1; i <= n; i++) {
            var next = startY + Mod(cursor.y + 1, currentColumnLength);
            if (map[next][cursor.x] == '#') {
                break;
            } else
                cursor.y = next;
        }
        return cursor;
    }

    private static char Turn(char currentDirection, bool isLeft) => currentDirection switch
    {
        '>' => isLeft ? '^' : 'v',
        '<' => isLeft ? 'v' : '^',
        '^' => isLeft ? '<' : '>',
        'v' => isLeft ? '>' : '<',
        _ => throw new InvalidOperationException(),
    };

    /// <summary>
    /// Modulus that works with negative numbers
    /// </summary>
    public static int Mod(int a, int b) => a % b < 0 ? a % b + b : a % b;
}