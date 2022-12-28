using System.Diagnostics;

// original source: https://stackoverflow.com/questions/969290/exact-time-measurement-for-performance-testing
namespace shared
{
    public class Perf
    {
        interface IStopwatch
        {
            bool IsRunning { get; }
            TimeSpan Elapsed { get; }

            void Start();
            void Stop();
            void Reset();
        }

        class TimeWatch : IStopwatch
        {
            private readonly Stopwatch stopwatch = new();

            public TimeSpan Elapsed => stopwatch.Elapsed;
            public bool IsRunning => stopwatch.IsRunning;

            public TimeWatch()
            {
                if (!Stopwatch.IsHighResolution)
                    throw new NotSupportedException("Your hardware doesn't support high resolution counter");

                //prevent the JIT Compiler from optimizing Fkt calls away
                long seed = Environment.TickCount;

                //use the second Core/Processor for the test
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

                //prevent "Normal" Processes from interrupting Threads
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                //prevent "Normal" Threads from interrupting this thread
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }

            public void Start() => stopwatch.Start();
            public void Stop() => stopwatch.Stop();
            public void Reset() => stopwatch.Reset();
        }

        class CpuWatch : IStopwatch
        {
            private TimeSpan startTime;
            private TimeSpan endTime;
            private bool isRunning;

            public TimeSpan Elapsed
            {
                get
                {
                    if (IsRunning)
                        throw new NotImplementedException("Getting elapsed span while watch is running is not implemented");

                    return endTime - startTime;
                }
            }

            public bool IsRunning => isRunning;

            public void Start()
            {
                startTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = true;
            }

            public void Stop()
            {
                endTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = false;
            }

            public void Reset()
            {
                startTime = TimeSpan.Zero;
                endTime = TimeSpan.Zero;
            }
        }

        static (TResult[], double[]) Benchmark<T, TResult>(Func<TResult> action, int iterations = 1, int numberOfTimings = 1, bool shouldWarmUp = false) where T : IStopwatch, new()
        {
            //clean Garbage
            GC.Collect();

            //wait for the finalizer queue to empty
            GC.WaitForPendingFinalizers();

            //clean Garbage
            GC.Collect();

            if (shouldWarmUp) {
                //warm up
                action();
            }

            var stopwatch = new T();
            var timings = new double[numberOfTimings];
            var results = new TResult[numberOfTimings];
            for (int i = 0; i < timings.Length; i++) {
                stopwatch.Reset();
                stopwatch.Start();
                for (int j = 0; j < iterations; j++)
                    results[i] = action();
                stopwatch.Stop();
                timings[i] = stopwatch.Elapsed.TotalMilliseconds;
            }

            return (results, timings);
        }

        public static (TResult[], double[]) BenchmarkTime<TResult>(Func<TResult> action, int iterations = 1, int numberOfTimings = 1, bool shouldWarmUp = false) => Benchmark<TimeWatch, TResult>(action, iterations, numberOfTimings);
        public static (TResult[], double[]) BenchmarkCpu<TResult>(Func<TResult> action, int iterations = 1, int numberOfTimings = 1, bool shouldWarmUp = false) => Benchmark<CpuWatch, TResult>(action, iterations, numberOfTimings);
    }

    public static class ICollectionExtensions
    {
        public static double NormalizedMean(this ICollection<double> values)
        {
            if (values.Count == 0)
                return double.NaN;

            var deviations = values.Deviations().ToArray();
            var meanDeviation = deviations.Sum(t => Math.Abs(t.Item2)) / values.Count;
            return deviations.Where(t => t.Item2 > 0 || Math.Abs(t.Item2) <= meanDeviation).Average(t => t.Item1);
        }

        public static IEnumerable<(double, double)> Deviations(this ICollection<double> values)
        {
            if (values.Count == 0)
                yield break;

            var avg = values.Average();
            foreach (var d in values)
                yield return (d, avg - d);
        }
    }
}
