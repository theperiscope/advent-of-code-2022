namespace day13;

/// <summary>
/// Distress Signal
/// </summary>
internal class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var pairs = File.ReadAllLines(args[0]).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        var sum = pairs.Chunk(2).Select((pair, i) => Compare(pair[0], pair[1]) == -1 ? i + 1 : 0).Where(x => x != 0).Sum();
        Console.WriteLine($"Part 1: {sum}");

        pairs.AddRange(new string[] { "[[2]]", "[[6]]" });
        pairs.Sort(Compare);
        Console.WriteLine($"Part 2: {(pairs.IndexOf("[[2]]") + 1) * (pairs.IndexOf("[[6]]") + 1)}");
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    private static string Listify(string s) => s.All(IsDigit) ? $"[{s}]" : s;

    private static (string value, string remaining) GetNext(string s)
    {
        if (IsDigit(s[0])) {
            var number = new string(s.TakeWhile(IsDigit).ToArray());
            var rem = number.Length == s.Length ? number.Length : number.Length + 1;
            return (number, s[rem..]);
        }

        // list: full leftValue will be at level 0 nestingLevel and finish at either end of string or comma
        // [1, [2, 3]] or [1, 2, 3], 4, 5
        var (nestingLevel, i) = (0, 0);
        while (!(nestingLevel == 0 && (i == s.Length || s[i] == ','))) {
            if (s[i] == '[')
                nestingLevel++;
            if (s[i] == ']')
                nestingLevel--;
            i++;
        }

        return i == s.Length ? (s, string.Empty) : (s[..i], s[(i + 1)..]);
    }
    private static int Compare(string left, string right)
    {
        while (left.Length > 0 && right.Length > 0) {
            var (leftValue, leftRemaining) = GetNext(left);
            var (rightValue, rightRemaining) = GetNext(right);
            if (leftValue.All(IsDigit) && rightValue.All(IsDigit)) {
                var (nLeft, nRight) = (int.Parse(leftValue), int.Parse(rightValue));
                if (nLeft < nRight)
                    return -1;
                if (nLeft > nRight)
                    return 1;
            } else {
                (leftValue, rightValue) = (Listify(leftValue), Listify(rightValue));
                var result = Compare(leftValue[1..^1], rightValue[1..^1]);
                if (result != 0)
                    return result;
            }
            (left, right) = (leftRemaining, rightRemaining);
        }

        return right.Length > 0 ? -1 : left.Length > 0 ? 1 : 0;
    }
}