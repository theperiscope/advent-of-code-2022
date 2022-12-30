using shared;

namespace day24;

/// <summary>
/// Blizzard Basin
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
        var (mapW, mapH) = (input[0].Length, input.Count);

        var blizzardMaps = new List<HashSet<Blizzard>>(); // index is time
        var blizzards = GetBlizzards(input);
        do {
            blizzardMaps.Add(blizzards);
            blizzards = blizzards.Select(b => b.Next(mapW, mapH)).ToHashSet();
            if (blizzardMaps.Any(b => b.SetEquals(blizzards)))
                break;
        } while (true);

        var start = new Point(1, 0);
        var end = new Point(mapW - 2, mapH - 1);
        var initialState = new State(0, start);

        Part1(initialState, end, mapW, mapH, blizzardMaps);
        Part2(initialState, start, end, mapW, mapH, blizzardMaps);
    }

    private static void Part1(State initialState, Point end, int mapW, int mapH, List<HashSet<Blizzard>> blizzardMaps)
    {
        var state = FindPath(initialState, end, mapW, mapH, blizzardMaps);
        Console.WriteLine($"Part 1: {state.time}");
    }

    private static void Part2(State initialState, Point start, Point end, int mapW, int mapH, List<HashSet<Blizzard>> blizzardMaps)
    {
        var state = FindPath(initialState, end, mapW, mapH, blizzardMaps);
        state = FindPath(state, start, mapW, mapH, blizzardMaps);
        state = FindPath(state, end, mapW, mapH, blizzardMaps);
        Console.WriteLine($"Part 2: {state.time}");
    }

    private static State FindPath(State currentState, Point end, int mapWidth, int mapHeight, List<HashSet<Blizzard>> blizzardMaps)
    {
        var seen = new HashSet<State>();
        var q = new PriorityQueue<State, int>();

        q.Enqueue(currentState, currentState.time + currentState.location.ManhattanDistanceTo(end));
        while (q.Count > 0) {
            var state = q.Dequeue();
            if (state.location == end)
                return state;

            foreach (var option in Next(state, end, mapWidth, mapHeight, blizzardMaps)) {
                if (!seen.Contains(option)) {
                    seen.Add(option);
                    q.Enqueue(option, option.time + option.location.ManhattanDistanceTo(end));
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static List<State> Next(State state, Point end, int mapWidth, int mapHeight, List<HashSet<Blizzard>> blizzardMaps)
    {
        var options = new Point[] { new(0, 0), new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };
        var nextStates = new List<State>();
        foreach (var option in options) {
            var proposedLocation = state.location + option;
            var hasBlizzardInProposedLocation = blizzardMaps[(state.time + 1) % blizzardMaps.Count].Any(b => b.Location == proposedLocation);
            var proposedLocationWithinMapLimits =
                proposedLocation == state.location ||
                proposedLocation == end ||
                (proposedLocation.X >= 1 && proposedLocation.X <= mapWidth - 2 && proposedLocation.Y >= 1 && proposedLocation.Y <= mapHeight - 2);

            if (!hasBlizzardInProposedLocation && proposedLocationWithinMapLimits) {
                nextStates.Add(state with
                {
                    time = state.time + 1,
                    location = proposedLocation
                });
            }
        }

        return nextStates;
    }

    private static HashSet<Blizzard> GetBlizzards(List<string> input)
    {
        var blizzards = new HashSet<Blizzard>();
        for (var y = 0; y < input.Count; y++) {
            var line = input[y];
            for (var x = 0; x < line.Length; x++) {
                if (!new[] { '>', 'v', '<', '^' }.Contains(line[x]))
                    continue;

                var dir = line[x] switch
                {
                    '>' => Dir.R,
                    'v' => Dir.D,
                    '<' => Dir.L,
                    '^' => Dir.U,
                    _ => throw new NotImplementedException(),
                };

                blizzards.Add(new(new(x, y), dir));
            }
        }

        return blizzards;
    }
}

internal enum Dir { R = 0, D = 1, L = 2, U = 3 }
internal record State(int time, Point location);

internal record Blizzard
{
    public Point Location { get; }
    public Dir Direction { get; }

    public Blizzard(Point location, Dir direction)
    {
        Location = location;
        Direction = direction;
    }

    public Blizzard Next(int mapWidth, int mapHeight)
    {
        var offset = Direction switch
        {
            Dir.R => new Point(1, 0),
            Dir.D => new Point(0, 1),
            Dir.L => new Point(-1, 0),
            Dir.U => new Point(0, -1),
            _ => throw new NotImplementedException(),
        };

        var newLocation = Location + offset;
        newLocation.X = newLocation.X == 0 ? mapWidth - 2 : newLocation.X == mapWidth - 1 ? 1 : newLocation.X;
        newLocation.Y = newLocation.Y == 0 ? mapHeight - 2 : newLocation.Y == mapHeight - 1 ? 1 : newLocation.Y;

        return new Blizzard(newLocation, Direction);
    }
}