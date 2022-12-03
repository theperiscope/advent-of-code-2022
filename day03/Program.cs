using shared;

namespace day03
{
    /// <summary>
    /// Rucksacks
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            Part1(args[0]);
            Part2(args[0]);
        }

        private static int ToPoints(char c)
        {
            return c switch {
                >= 'A' and <= 'Z' => c - 'A' + 27,
                >= 'a' and <= 'z' => c - 'a' + 1,
                _ => throw new NotSupportedException()
            };
        }

        private static void Part1(string fileName)
        {
            var results = Utils.ReadAllLinesFrom(fileName)
                .Select(line => {
                    var left = line.Substring(0, line.Length / 2);
                    var right = line.Substring(left.Length);
                    return left.Length != right.Length ? throw new InvalidOperationException() : (left, right);
                });

            var total1 = results.Select(x => x.left.ToHashSet()).ToList();
            var total2 = results.Select(x => x.right.ToHashSet()).ToList();

            var points = total1.Select((x, i) => {
                var (n1, n2) = (total1[i], total2[i]);
                return ToPoints(n1.Intersect(n2).First());
            }).Sum();

            Console.WriteLine(points);
        }

        private static void Part2(string fileName)
        {
            var groups = Utils.ReadAllLinesFrom(fileName)
                .Chunk(3).Select(chunk => (chunk[0].ToHashSet(), chunk[1].ToHashSet(), chunk[2].ToHashSet()));

            var points = groups.Select(x => ToPoints(x.Item1.Intersect(x.Item2).Intersect(x.Item3).First())).Sum();

            Console.WriteLine(points);
        }

    }
}