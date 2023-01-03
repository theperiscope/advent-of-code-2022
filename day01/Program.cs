using shared;

namespace day01
{
    /// <summary>
    /// Calorie counting
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var lines = File.ReadAllLines(args[0]);

            // we have no specific requirements but turns out the sums are unique --
            // SortedList requires unique keys and allowing duplicates requires 
            // adding code for manual handing of equality (0) in Comparer below
            var sums = new SortedList<int, int>(new DecendingComparer<int>());
            foreach (var chunk in lines.Split((line) => line == string.Empty)) {
                var sum = chunk.Select(x => int.Parse(x)).Sum();
                sums.Add(sum, sums.Count + 1 /* the elf number */);
            }

            Console.WriteLine($"Part 1: Elf {sums.Values[0]} is carrying {sums.Keys[0]} calories.");

            Console.WriteLine($"Part 2: Top 3 Elves ({sums.Values[0]}, {sums.Values[1]}, {sums.Values[2]}) are carrying {sums.Keys[0] + sums.Keys[1] + sums.Keys[2]} calories.");
        }
    }
}

internal class DecendingComparer<TKey> : IComparer<TKey> where TKey : struct, IComparable<TKey>
{
    public int Compare(TKey x, TKey y) => y.CompareTo(x);
}