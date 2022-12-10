using System.Text.RegularExpressions;

namespace day11;

/// <summary>
/// Monkey in the Middle
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
        var monkeys = new List<Monkey>();
        for (var i = 0; i < data.Length; i += 7) {
            var startingItems = data[i + 1][(data[i + 1].IndexOf(':') + 2)..].Split(", ").Select(x => long.Parse(x)).ToList();
            var operation = Monkey.ParseOperation(data[i + 2]);
            var divisibleByTest = int.Parse(data[i + 3][(data[i + 3].IndexOf("by ") + 3)..]);
            var testSuccess = int.Parse(data[i + 4][(data[i + 4].Length - 2)..]);
            var testFailure = int.Parse(data[i + 5][(data[i + 5].Length - 2)..]);

            var m = new Monkey(startingItems, operation, divisibleByTest, testSuccess, testFailure);
            monkeys.Add(m);
        }

        for (var i = 0; i < 20; i++) {
            foreach (var monkey in monkeys) {
                foreach (var item in monkey.Items) {
                    var value = monkey.Operation(item) / 3;
                    var nextMonkey = value % monkey.DivisibleByTest == 0 ?
                        monkey.TestSuccess : monkey.TestFailure;
                    monkeys[nextMonkey].Items.Add((int)value);
                }

                monkey.InspectedItems += monkey.Items.Count;
                monkey.Items.Clear();
            }
        }

        var top2 = monkeys
            .Select(x => x.InspectedItems)
            .OrderByDescending(x => x)
            .Take(2).ToList();

        Console.WriteLine($"Part 1: {top2[0] * top2[1]}");

        monkeys.Clear(); // reset so we can get again initial start items
        for (var i = 0; i < data.Length; i += 7) {
            var startingItems = data[i + 1][(data[i + 1].IndexOf(':') + 2)..].Split(", ").Select(x => long.Parse(x)).ToList();
            var operation = Monkey.ParseOperation(data[i + 2]);
            var divisibleByTest = int.Parse(data[i + 3][(data[i + 3].IndexOf("by ") + 3)..]);
            var testSuccess = int.Parse(data[i + 4][(data[i + 4].Length - 2)..]);
            var testFailure = int.Parse(data[i + 5][(data[i + 5].Length - 2)..]);

            var m = new Monkey(startingItems, operation, divisibleByTest, testSuccess, testFailure);
            monkeys.Add(m);
        }

        for (var i = 0; i < 10_000; i++) {
            foreach (var monkey in monkeys) {
                foreach (var item in monkey.Items) {
                    var value = monkey.Operation(item);
                    var nextMonkey = value % monkey.DivisibleByTest == 0 ?
                        monkey.TestSuccess : monkey.TestFailure;
                    monkeys[nextMonkey].Items.Add(value);
                }

                monkey.InspectedItems += monkey.Items.Count;
                monkey.Items.Clear();
            }
        }

        var top2_part2 = monkeys
            .Select(x => x.InspectedItems)
            .OrderByDescending(x => x)
            .Take(2).ToList();

        Console.WriteLine($"Part 2: {top2_part2[0]} * {top2_part2[1]} = {top2_part2[0] * top2_part2[1]}");
    }
}

internal class Monkey
{
    public Monkey(List<long> items, Func<long, long> operation, int divisibleByTest, int testSuccess, int testFailure)
    {
        Items = items;
        Operation = operation;
        DivisibleByTest = divisibleByTest;
        TestSuccess = testSuccess;
        TestFailure = testFailure;
        InspectedItems = 0;
    }

    public List<long> Items { get; set; }
    public Func<long, long> Operation { get; set; }
    public int DivisibleByTest { get; set; }
    public int TestSuccess { get; set; }
    public int TestFailure { get; set; }
    public long InspectedItems { get; set; }
    public static Func<long, long> ParseOperation(string input)
    {
        var pattern = new Regex(@"new\s=\s(?<old>old)\s*(?<op>[\+\-\*\/])\s*(?<n>\d+|old)$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var match = pattern.Match(input);
        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(match), "invalid format");

        var n =
            string.Equals("old", match.Groups["n"].Value, StringComparison.InvariantCultureIgnoreCase) ?
            Int32.MinValue : long.Parse(match.Groups["n"].Value);

        var op = match.Groups["op"].Value;

        return i =>
             op switch {
                 "+" => i + ((n == int.MinValue) ? i : n),
                 "-" => i - ((n == int.MinValue) ? i : n),
                 "*" => i * ((n == int.MinValue) ? i : n),
                 "/" => i / ((n == int.MinValue) ? i : n),
                 _ => throw new InvalidOperationException(),
             };
    }
}