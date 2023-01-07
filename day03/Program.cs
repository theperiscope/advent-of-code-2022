using shared;

namespace day03
{
    /// <summary>
    /// Rucksacks
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
            var input = parseResults[0];
            Console.WriteLine($"Parsing: {input.Length} rucksacks in {parseTimings[0]:F2}ms");

            var (part1Results, part1Timings) = Perf.BenchmarkTime(() => Part1(input));
            var part1 = part1Results[0];
            Console.WriteLine($"Part 1 : {part1} in {part1Timings[0]:F2}ms");

            var (part2Results, part2Timings) = Perf.BenchmarkTime(() => Part2(input));
            var part2 = part2Results[0];
            Console.WriteLine($"Part 2 : {part2} in {part2Timings[0]:F2}ms");

            Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0] + part2Timings[0]:F2}ms");
        }

        private static string[] Parse(string fileName) => File.ReadAllLines(fileName);

        private static int Part1(string[] lines) {
            var sum = 0;
            foreach (var s in lines) {
                var (a, b) = (s[..(s.Length / 2)], s[(s.Length / 2)..]); // left and right side
                var (A, B) = (0L, 0L);

                // a and b have same length here by definition
                for (var i = 0; i < a.Length; i++) {
                    A |= 1L << (a[i] - 'A');
                    B |= 1L << (b[i] - 'A');
                }

                // 0 bit - A, 25th bit - Z, 32nd bit - a, 57th bit = z
                // priority: 0..25th bit (A..Z): i + 27
                // priority: 32..57th bit (a..z): i - 32 + 1
                for (var i = 0; i <= 25; i++) {
                    if (((A >> i) & 1) == 1 && ((B >> i) & 1) == 1) {
                        sum += i >= 0 && i <= 25 ? i + 27 : i - 32 + 1;
                    }
                }
                for (var i = 32; i <= 57; i++) {
                    if (((A >> i) & 1) == 1 && ((B >> i) & 1) == 1) {
                        sum += i >= 0 && i <= 25 ? i + 27 : i - 32 + 1;
                    }
                }
            }
            return sum;
        }

        private static int Part2(string[] lines) {
            var sum = 0;
            foreach (var s in lines.Chunk(3)) {
                var (a, b, c) = (s[0], s[1], s[2]);
                var (A, B, C) = (0L, 0L, 0L);

                // a, b, c here can have different lengths so multiple loops are needed
                for (var i = 0; i < a.Length; i++) A |= 1L << (a[i] - 'A');
                for (var i = 0; i < b.Length; i++) B |= 1L << (b[i] - 'A');
                for (var i = 0; i < c.Length; i++) C |= 1L << (c[i] - 'A');

                for (var i = 0; i <= 25; i++) {
                    if (((A >> i) & 1) == 1 && ((B >> i) & 1) == 1 && ((C >> i) & 1) == 1) {
                        sum += i >= 0 && i <= 25 ? i + 27 : i - 32 + 1;
                    }
                }
                for (var i = 32; i <= 57; i++) {
                    if (((A >> i) & 1) == 1 && ((B >> i) & 1) == 1 && ((C >> i) & 1) == 1) {
                        sum += i >= 0 && i <= 25 ? i + 27 : i - 32 + 1;
                    }
                }
            }
            return sum;
        }
    }
}