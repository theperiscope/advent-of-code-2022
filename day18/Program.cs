using shared;

internal class Program
{
    /// <summary>
    /// Boiling Boulders
    /// </summary>
    private static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (parseResults, parseTimings) = Perf.BenchmarkTime(() => Parse(args[0]));
        var cubes = parseResults[0];
        Console.WriteLine($"Parsing: {cubes.Count} cubes in {parseTimings[0]:F2}ms");

        var (part1Results, part1Timings) = Perf.BenchmarkTime(() => Part1(cubes));
        var sum = part1Results[0];
        Console.WriteLine($"Part 1 : {sum} in {part1Timings[0]:F2}ms");

        var (part2Results, part2Timings) = Perf.BenchmarkTime(() => Part2(cubes));
        var sum2 = part2Results[0];
        Console.WriteLine($"Part 2 : {sum2} in {part2Timings[0]:F2}ms");

        Console.WriteLine($"Total  : {parseTimings[0] + part1Timings[0] + part2Timings[0]:F2}ms");
    }

    private static int Part2(HashSet<int> cubes) {
        var cubePoints = cubes.Select(c => FromInt(c));
        var xmin = cubePoints.Select(p => p.x).Min();
        var xmax = cubePoints.Select(p => p.x).Max();
        var ymin = cubePoints.Select(p => p.y).Min();
        var ymax = cubePoints.Select(p => p.y).Max();
        var zmin = cubePoints.Select(p => p.z).Min();
        var zmax = cubePoints.Select(p => p.z).Max();

        // expand range by 1 in all directions and if there's no cube it's a space
        var spaces = new HashSet<int>();
        for (var x = xmin - 1; x <= xmax + 1; x++)
            for (var y = ymin - 1; y <= ymax + 1; y++)
                for (var z = zmin - 1; z <= zmax + 1; z++) {
                    var p = (x, y, z);
                    if (!cubes.Contains(ToInt(p)))
                        spaces.Add(ToInt(p));
                }

        // visit spaces
        var q = new Queue<int>();
        var visited = new HashSet<int>();
        q.Enqueue(spaces.First());

        while (q.Count > 0) {
            var here = q.Dequeue();
            visited.Add(here);
            foreach (var n in Neighbors(FromInt(here))) {
                if (visited.Contains(n))
                    continue;
                if (!spaces.Contains(n))
                    continue;

                if (!q.Contains(n))
                    q.Enqueue(n);
            }
        }

        // any spaces lefts are void, add them as cubes
        foreach (var p in spaces) {
            if (visited.Contains(p))
                continue;
            cubes.Add(p);
        }

        // and re-run part1
        var sum2 = cubes.Select(c => FromInt(c)).Select(p => {
            var sides = 6;
            var neighbors = Neighbors(p);
            foreach (var n in neighbors) {
                if (cubes.Contains(n))
                    sides--;
            }
            return sides;
        }).Sum();
        return sum2;
    }

    private static int Part1(HashSet<int> cubes) {
        return cubes.Select(c => FromInt(c)).Select(p => {
            var sides = 6;
            var neighbors = Neighbors(p);
            foreach (var n in neighbors) {
                if (cubes.Contains(n))
                    sides--;
            }
            return sides;
        }).Sum();
    }

    private static HashSet<int> Parse(string fileName) => File.ReadAllLines(fileName).Select(l => l.Split(",").Select((s, i) => int.Parse(s) * ((int)Math.Pow(10, i * 2))).Sum()).ToHashSet();

    private static HashSet<int> Neighbors((int x, int y, int z) p) {
        return new HashSet<int>
        {
            ToInt(new(p.x+1, p.y  ,p.z)),
            ToInt(new(p.x-1, p.y  ,p.z)),
            ToInt(new(p.x  , p.y+1,p.z)),
            ToInt(new(p.x  , p.y-1,p.z)),
            ToInt(new(p.x  , p.y  ,p.z+1)),
            ToInt(new(p.x  , p.y  ,p.z-1))
        };
    }

    private static int ToInt((int x, int y, int z) p) {
        return p.x * 10000 + p.y * 100 + p.z;
    }

    private static (int x, int y, int z) FromInt(int coord) {
        return (coord / 10000, coord / 100 % 100, coord % 100);
    }
}
