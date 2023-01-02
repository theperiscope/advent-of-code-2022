using shared;
using System.Text;
using System.Text.RegularExpressions;

namespace day05;

/// <summary>
/// Supply Stacks
/// </summary>
internal class Program
{
    private static readonly Regex instructionRegex = new(@"^move (?<n>\d+) from (?<from>\d+) to (?<to>\d+)$", RegexOptions.Compiled);

    private static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
        var data = parseResults[0];
        Console.WriteLine($"Parsing: {data.Length} stacks in {parseTimings[0]:F2}ms");

        var (part1Results, part1Timings) = Perf.BenchmarkTime(() => Part1(data));
        var part1 = part1Results[0];
        Console.WriteLine($"Part 1 : {part1} in {part1Timings[0]:F2}ms");

        var (part2Results, part2Timings) = Perf.BenchmarkTime(() => Part2(data));
        var part2 = part2Results[0];
        Console.WriteLine($"Part 2 : {part2} in {part2Timings[0]:F2}ms");
     
        Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0] + part2Timings[0]:F2}ms");
    }

    private static string[] Parse(string fileName) => File.ReadAllLines(fileName).TrimTrailingEndOfLine();

    /// <summary>
    /// Processes stack information contained before the blank line in the input files
    /// </summary>
    /// <returns>List of stacks containing characters, and the file row number where processing instructions begin.</returns>
    private static List<Stack<char>> CreateStacksFromInput(string[] data, out int initialRowWithInstructions) {
        var row = 0;
        // "[A] " is 4 characters and the file is formatted with spaces on those lines.
        // The +1 is for missing final space after last stack on the line.
        var numberOfStacks = (data[0].Length + 1) / 4;

        // Maintain list of stacks containing characters. Index 0 is Stack 1 in the input file, etc.
        var stacks = new List<Stack<char>>(numberOfStacks);
        var stacksInput = new List<StringBuilder>(numberOfStacks);
        for (var i = 0; i < numberOfStacks; i++) {
            stacks.Add(new Stack<char>());
            stacksInput.Add(new StringBuilder());
        }

        while (data[row].IndexOf('[') >= 0) {
            for (var i = 0; i < numberOfStacks; i++) {
                var ch = data[row][i * 4 + 1];
                if (ch != ' ') {
                    stacksInput[i].Insert(0, ch);
                }
            }
            row++;
        }

        for (var i = 0; i < numberOfStacks; i++) {
            for (var j = 0; j < stacksInput[i].Length; j++) {
                stacks[i].Push(stacksInput[i][j]);
            }
        }

        initialRowWithInstructions = row + 2; // we are at rows with number, skip it along with next (blank) one
        return stacks;
    }

    private static string GetStackTops(List<Stack<char>> stacks) => new string(stacks.Select(s => s.Peek()).ToArray());

    private static string Part1(string[] data) {
        var stacks = CreateStacksFromInput(data, out var initialRowWithInstructions);

        for (var i = initialRowWithInstructions; i < data.Length; i++) {
            var m = instructionRegex.Match(data[i]);
            var n = int.Parse(m.Groups["n"].Value);
            var from = int.Parse(m.Groups["from"].Value) - 1;
            var to = int.Parse(m.Groups["to"].Value) - 1;

            for (var j = 0; j < n; j++) {
                var ch = stacks[from].Pop();
                stacks[to].Push(ch);
            }
        }

        return GetStackTops(stacks);
    }

    private static string Part2(string[] data) {
        var stacks = CreateStacksFromInput(data, out var initialRowWithInstructions);

        for (var i = initialRowWithInstructions; i < data.Length; i++) {
            var m = instructionRegex.Match(data[i]);
            var n = int.Parse(m.Groups["n"].Value);
            var from = int.Parse(m.Groups["from"].Value) - 1;
            var to = int.Parse(m.Groups["to"].Value) - 1;

            var boxes = new StringBuilder();
            for (var j = 0; j < n; j++) {
                var ch = stacks[from].Pop();
                boxes.Insert(0, ch);
            }
            for (var j = 0; j < n; j++) {
                var ch = boxes[j];
                stacks[to].Push(ch);
            }
        }

        return GetStackTops(stacks);
    }
}