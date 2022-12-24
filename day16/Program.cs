using System.Text.RegularExpressions;

/// <summary>
/// Proboscidea Volcanium
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var input = File.ReadLines(args[0]).ToList();
        var inputRegex = new Regex(@"Valve ([A-Z]{2}).*rate=(\d+);.*to\svalve[s]*\s(.*)", RegexOptions.Compiled);
        var valves = input.Select(line => inputRegex.Match(line)).Select(m => new Valve(m.Groups[1].Value, int.Parse(m.Groups[2].Value), m.Groups[3].Value.Split(", "), new Dictionary<string, int>())).ToDictionary(v => v.Name, v => v);
        valves = valves.Select(v => new Valve(v.Value.Name, v.Value.FlowRate, v.Value.LeadsTo, FindShortestTimes(v.Value, valves))).ToDictionary(v => v.Name, v => v);
        var usefulValves = valves.Where(v => v.Value.FlowRate > 0).ToDictionary(v => v.Value.Name, v => v.Value);

        var answer1 = CalculateBestReleasedPressure(30, valves["AA"], usefulValves);
        Console.WriteLine($"Part 1: {answer1}");

        var answer2 = CalculateCombinedBestReleasedPressure(new[] { 26, 26 }, new[] { valves["AA"], valves["AA"] }, usefulValves);
        Console.WriteLine($"Part 2: {answer2}");
    }

    private static int CalculateBestReleasedPressure(int timeLeft, Valve currentValve, Dictionary<string, Valve> usefulValves)
    {
        var best = 0;
        foreach (var v in usefulValves) {
            var newTimeLeft = timeLeft - currentValve.ShortestTimeTo[v.Key] - 1;
            if (newTimeLeft > 0) {
                var proposed = newTimeLeft * v.Value.FlowRate + CalculateBestReleasedPressure(newTimeLeft, v.Value, usefulValves.Where(c => c.Value != v.Value).ToDictionary(c => c.Key, c => c.Value));
                if (proposed > best)
                    best = proposed;
            }
        }
        return best;
    }

    private static int CalculateCombinedBestReleasedPressure(int[] timeLeft, Valve[] currentValves, Dictionary<string, Valve> usefulValves)
    {
        var best = 0;
        var index = timeLeft[0] > timeLeft[1] ? 0 : 1;
        var currentValve = currentValves[index];
        foreach (var v in usefulValves) {
            var newTimeLeft = timeLeft[index] - currentValve.ShortestTimeTo[v.Key] - 1;
            if (newTimeLeft > 0) {
                var newTimes = new[] { newTimeLeft, timeLeft[1 - index] /* the other */ };
                var newCurrentValves = new[] { v.Value, currentValves[1 - index] /* the other */ };
                var proposed = newTimeLeft * v.Value.FlowRate + CalculateCombinedBestReleasedPressure(newTimes, newCurrentValves, usefulValves.Where(c => c.Value != v.Value).ToDictionary(c => c.Key, c => c.Value));
                if (proposed > best)
                    best = proposed;
            }
        }
        return best;
    }

    /// <summary>
    /// Dijkstra implementation all shortest times from source to reach other valves
    /// </summary>
    private static Dictionary<string, int> FindShortestTimes(Valve source, Dictionary<string, Valve> valves)
    {
        var (distances, vertices) = (new Dictionary<string, int>(valves.Count), valves.Keys.ToList());
        for (var i = 0; i < vertices.Count; i++)
            distances[vertices[i]] = Int32.MaxValue;
        distances[source.Name] = 0;

        var pq = new PriorityQueue<Valve, int>();
        pq.Enqueue(source, 0);
        while (pq.Count > 0) {
            var current = pq.Dequeue();
            foreach (var tunnel in current.LeadsTo) {
                var proposal = distances[current.Name] + 1;
                if (proposal < distances[tunnel]) {
                    distances[tunnel] = proposal;
                    pq.Enqueue(valves[tunnel], distances[tunnel]);
                }
            }
        }

        return distances;
    }
}

internal record Valve(string Name, int FlowRate, string[] LeadsTo, Dictionary<string, int> ShortestTimeTo);