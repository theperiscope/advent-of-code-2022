using System.Text;

namespace day25
{
    /// <summary>
    /// Full of Hot Air
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var part1 = ToSnafu(File.ReadLines(args[0]).Select(line => FromSnafu(line)).Sum());
            Console.WriteLine($"Part 1: {part1}");
        }

        private static long FromSnafu(string snafu)
        {
            var digit = new Dictionary<char, int> { { '=', -2 }, { '-', -1 }, { '0', 0 }, { '1', 1 }, { '2', 2 } };
            var n = 0L;
            for (var i = 0; i < snafu.Length; i++) {
                var c = snafu[i];
                var mult = (long)Math.Pow(5, snafu.Length - i - 1);
                n += digit[c] * mult;
            }
            return n;
        }

        private static string ToSnafu(long snafu)
        {
            const string alpha = "=-012";
            var sb = new StringBuilder();
            var n = snafu;
            do {
                var i = (int)((n + 2) % 5);
                sb.Insert(0, alpha[i]);
                n = (n + 2) / 5;
            } while (n > 0);
            return sb.ToString();
        }
    }
}