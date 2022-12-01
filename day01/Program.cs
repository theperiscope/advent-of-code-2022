namespace day01
{
    /// <summary>
    /// Calorie counting
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var lines = File.ReadAllLines(args[0]);
            if (lines[lines.Length - 1].Length != 0) { // add extra blank line for ease of processing
                Array.Resize(ref lines, lines.Length + 1);
                lines[lines.Length - 1] = "";
            }

            var n = 0;
            var chunkStart = 0;

            var sums = new List<(int, int)>();
            foreach (var line in lines) {
                if (line.Length == 0) {
                    var sum = lines.Skip(chunkStart).Take(n - chunkStart).Select(x => int.Parse(x)).Sum();
                    chunkStart = n + 1;
                    sums.Add((sums.Count + 1, sum));
                }
                n++;
            }

            var sorted = sums.OrderByDescending(x => x.Item2).ToList();

            // part 1
            Console.WriteLine("Max Elf: {0}, {1} calories", sorted[0].Item1, sorted[0].Item2);

            // part 2
            Console.WriteLine("Top 3: {0} calories", sorted.Select(x => x.Item2).Take(3).Sum());
        }
    }
}