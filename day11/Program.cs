using shared;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace day11;

/// <summary>
/// Monkey in the Middle
/// </summary>
internal class Program
{
    private static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
        var monkeys = parseResults[0].Clone();
        Console.WriteLine($"Parsing: {monkeys.Count} monkeys in {parseTimings[0]:F2}ms");

        var (part1Results, part1Timings) = Perf.BenchmarkTime(() => Part1(monkeys));
        var (m1, m2) = part1Results[0];
        Console.WriteLine($"Part 1 : {m1 * m2} in {part1Timings[0]:F2}ms");

        monkeys = parseResults[0].Clone();
        var (part2Results, part2Timings) = Perf.BenchmarkTime(() => Part2(monkeys));
        (m1, m2) = part2Results[0];
        Console.WriteLine($"Part 2 : {m1 * m2} in {part2Timings[0]:F2}ms");
        Console.WriteLine();
        Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0] + part2Timings[0]:F2}ms");
    }

    private static List<Monkey> Parse(string fileName) {
        var data = File.ReadAllLines(fileName);
        var monkeys = new List<Monkey>();
        for (var i = 0; i < data.Length; i += 7) { // 7 rows per monkey in input file
            var startingItems = data[i + 1][18..].Split(", ").Select(x => long.Parse(x));
            var operation = Monkey.ParseOperation(data[i + 2]);
            var divisibleByTest = int.Parse(data[i + 3][21..]);
            var testSuccess = int.Parse(data[i + 4][(data[i + 4].Length - 2)..]);
            var testFailure = int.Parse(data[i + 5][(data[i + 5].Length - 2)..]);
            monkeys.Add(new Monkey(startingItems, operation, divisibleByTest, testSuccess, testFailure, 0));
        }
        return monkeys;
    }

    private static (long m1, long m2) Part1(IList<Monkey> monkeys) {
        for (var i = 0; i < 20; i++) {
            foreach (var monkey in monkeys) {
                foreach (var item in monkey.Items) {
                    var worryLevel = monkey.Operation(item) / 3;
                    var nextMonkey = worryLevel % monkey.DivisibleByTest == 0 ? monkey.TestSuccess : monkey.TestFailure;
                    monkeys[nextMonkey].Items.Add((int)worryLevel);
                }

                monkey.InspectedItems += monkey.Items.Count;
                monkey.Items.Clear();
            }
        }

        var top2 = monkeys.Select(x => x.InspectedItems).OrderByDescending(x => x).Take(2).ToList();
        return (top2[0], top2[1]);
    }

    private static (long m1, long m2) Part2(IList<Monkey> monkeys) {
        // multiply all divisible-by tests (they are all prime in input)
        var allTestsMod = monkeys.Select(m => m.DivisibleByTest).Aggregate((a, b) => a * b);
        for (var i = 0; i < 10_000; i++) {
            foreach (var monkey in monkeys) {
                foreach (var item in monkey.Items) {
                    var worryLevel = monkey.Operation(item) % allTestsMod; // prevents worryLevel from getting too large
                    var nextMonkey = worryLevel % monkey.DivisibleByTest == 0 ? monkey.TestSuccess : monkey.TestFailure;
                    monkeys[nextMonkey].Items.Add(worryLevel);
                }

                monkey.InspectedItems += monkey.Items.Count;
                monkey.Items.Clear();
            }
        }

        var top2 = monkeys.Select(x => x.InspectedItems).OrderByDescending(x => x).Take(2).ToList();
        return (top2[0], top2[1]);
    }
}

internal class Monkey : ICloneable
{
    public Monkey(IEnumerable<long> items, Func<long, long> operation, int divisibleByTest, int testSuccess, int testFailure, int inspectedItems) {
        Items = new (items);
        Operation = operation;
        DivisibleByTest = divisibleByTest;
        TestSuccess = testSuccess;
        TestFailure = testFailure;
        InspectedItems = inspectedItems;
    }

    public List<long> Items { get; set; }
    public Func<long, long> Operation { get; set; }
    public int DivisibleByTest { get; set; }
    public int TestSuccess { get; set; }
    public int TestFailure { get; set; }
    public int InspectedItems { get; set; }
    public static Func<long, long> ParseOperation(string input) {
        var match = pattern.Match(input);
        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(match), "invalid format");

        var secondArg = string.Equals("old", match.Groups["secondArg"].Value) ? int.MinValue : long.Parse(match.Groups["secondArg"].Value);
        var op = match.Groups["op"].Value;
        return arg =>
             op switch {
                 "+" => arg + ((secondArg == int.MinValue) ? arg : secondArg),
                 "-" => arg - ((secondArg == int.MinValue) ? arg : secondArg),
                 "*" => arg * ((secondArg == int.MinValue) ? arg : secondArg),
                 "/" => arg / ((secondArg == int.MinValue) ? arg : secondArg),
                 _ => throw new InvalidOperationException(),
             };
    }

    private static readonly Regex pattern = new(@"new\s=\s(?<old>old)\s*(?<op>[\+\-\*\/])\s*(?<secondArg>\d+|old)", RegexOptions.Compiled);

    public object Clone() {
        return new Monkey(new List<long>(Items.ToArray()), Operation, DivisibleByTest, TestSuccess, TestFailure, InspectedItems);
    }
}