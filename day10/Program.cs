using shared;

namespace day10;

/// <summary>
/// Cathode-Ray Tube
/// </summary>
internal class Program
{
    private static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => File.ReadAllLines(args[0]).TrimTrailingEndOfLine());
        var data = parseResults[0];
        Console.WriteLine($"Parsing: {data.Length} rows in {parseTimings[0]}ms");

        var (part1Results, part1Timings) = Perf.BenchmarkTime(() => {
            var sum = 0;
            var cpu = new CPU(new CRT());

            cpu.AfterCycle += cpuAfterCycle;
            foreach (var instruction in data) {
                if (instruction == "noop")
                    cpu.Noop();
                else
                    cpu.AddX(int.Parse(instruction[5..]));
            }
            cpu.AfterCycle -= cpuAfterCycle;

            void cpuAfterCycle(object? sender, (int cycle, int x) e) {
                var cpu = sender as CPU;
                var duringCycle = e.cycle + 1;
                var positionToDraw = duringCycle - 1;
                if (e.cycle is 19 or 59 or 99 or 139 or 179 or 219) {
                    sum += (e.cycle + 1) * e.x;
                    //Console.WriteLine($"After cycle {e.cycle} (i.e. during cycle {duringCycle}), X = {e.x}.");
                }

                var col = positionToDraw % 40;
                if (col == e.x || col == e.x - 1 || col == e.x + 1) {
                    cpu!.Crt.Display[positionToDraw] = true;
                }
            }

            return (cpu, sum);
        });

        Console.WriteLine($"Part 1 : {part1Results[0].sum} in {part1Timings[0]}ms");

        Console.WriteLine($"Part 2 : (timing included in Part 1)");
        part1Results[0].cpu.Crt.Render();

        Console.WriteLine();
        Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0]}ms");

    }

    internal class CRT
    {
        public readonly bool[] Display = new bool[240];

        public void Render() {
            for (var row = 0; row < 6; row++) {
                for (var col = 0; col < 40; col++)
                    Console.Write(Display[row * 40 + col] ? "█" : "░");
                Console.WriteLine();
            }
        }
    }

    internal class CPU
    {
        public CPU(CRT crt) {
            Cycle = 0;
            X = 1;
            Crt = crt ?? throw new ArgumentNullException(nameof(crt));
            crt.Display[0] = true;
        }

        public int Cycle { get; set; }

        public int X { get; set; }
        public CRT Crt { get; }

        public event EventHandler<(int cycle, int x)> AfterCycle;

        protected void OnAfterCycle(int cycle, int x) {
            AfterCycle?.Invoke(this, (cycle, x));
        }

        public void Noop() {
            Cycle++;
            OnAfterCycle(Cycle, X);
        }

        public void AddX(int n) {
            Cycle++;
            OnAfterCycle(Cycle, X);

            Cycle++;
            X += n;
            OnAfterCycle(Cycle, X);
        }
    }
}