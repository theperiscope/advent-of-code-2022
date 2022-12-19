using System.Text.RegularExpressions;
namespace day19;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var input = File.ReadAllText(args[0]);
        Console.WriteLine(PartOne(input));
        Console.WriteLine(PartTwo(input));
    }

    public static object PartOne(string input)
    {
        var res = 0;
        foreach (var blueprint in Parse(input)) {
            res += blueprint.id * MaxGeodes(blueprint, 24);
        }
        return res;
    }

    public static object PartTwo(string input)
    {
        var res = 1;
        foreach (var blueprint in Parse(input).Where(bp => bp.id <= 3)) {
            res *= MaxGeodes(blueprint, 32);
        }
        return res;
    }

    private static int MaxGeodes(Blueprint blueprint, int timeLimit)
    {
        return MaxGeodes(
            blueprint,
            new State(
                remainingTime: timeLimit,
                available: new Inventory(ore: 0, 0, 0, 0),
                produced: new Inventory(ore: 1, 0, 0, 0)
            ),
            new Dictionary<State, int>()
        );
    }

    // Returns the maximum mineable geodes under the given state constraints,
    // Recursion with a cache.
    private static int MaxGeodes(Blueprint bluePrint, State state, Dictionary<State, int> cache)
    {
        if (state.remainingTime == 0) {
            return state.available.geode;
        }

        if (!cache.ContainsKey(state)) {
            cache[state] = (
                from afterFactory in NextSteps(bluePrint, state)
                let afterMining = afterFactory with
                {
                    remainingTime = state.remainingTime - 1,
                    available = afterFactory.available + state.produced
                }
                select MaxGeodes(bluePrint, afterMining, cache)
            ).Max();
        }

        return cache[state];
    }

    private static IEnumerable<State> NextSteps(Blueprint bluePrint, State state)
    {
        var now = state.available;
        var prev = now - state.produced;

        if (!CanBuild(bluePrint.geode, prev) && CanBuild(bluePrint.geode, now)) {
            yield return Build(state, bluePrint.geode);
            yield break;
        }

        if (!CanBuild(bluePrint.obsidian, prev) && CanBuild(bluePrint.obsidian, now)) {
            yield return Build(state, bluePrint.obsidian);
        }
        if (!CanBuild(bluePrint.clay, prev) && CanBuild(bluePrint.clay, now)) {
            yield return Build(state, bluePrint.clay);
        }
        if (!CanBuild(bluePrint.ore, prev) && CanBuild(bluePrint.ore, now)) {
            yield return Build(state, bluePrint.ore);
        }

        yield return state;
    }

    private static bool CanBuild(Robot robot, Inventory availableMaterial) => availableMaterial >= robot.cost;

    private static State Build(State state, Robot robot) =>
        state with
        {
            available = state.available - robot.cost,
            produced = state.produced + robot.produces
        };

    private static State Mine(State state, Inventory miners) =>
        state with
        {
            available = state.available + miners
        };

    private static IEnumerable<Blueprint> Parse(string input)
    {
        foreach (var line in input.Split("\n")) {
            var numbers = Regex.Matches(line, @"(\d+)").Select(x => int.Parse(x.Value)).ToArray();
            yield return new Blueprint(
                id: numbers[0],
                ore: new Robot(
                    cost: new Inventory(ore: numbers[1], clay: 0, obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 1, clay: 0, obsidian: 0, geode: 0)
                ),
                clay: new Robot(
                    cost: new Inventory(ore: numbers[2], clay: 0, obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 0, clay: 1, obsidian: 0, geode: 0)
                ),
                obsidian: new Robot(
                    cost: new Inventory(ore: numbers[3], clay: numbers[4], obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 0, clay: 0, obsidian: 1, geode: 0)
                ),
                geode: new Robot(
                    cost: new Inventory(ore: numbers[5], clay: 0, obsidian: numbers[6], geode: 0),
                    produces: new Inventory(ore: 0, clay: 0, obsidian: 0, geode: 1)
                )
            );
        }
    }
}

record Inventory(int ore, int clay, int obsidian, int geode)
{
    public static Inventory operator +(Inventory a, Inventory b)
    {
        return new Inventory(
            a.ore + b.ore,
            a.clay + b.clay,
            a.obsidian + b.obsidian,
            a.geode + b.geode
        );
    }

    public static Inventory operator -(Inventory a, Inventory b)
    {
        return new Inventory(
            a.ore - b.ore,
            a.clay - b.clay,
            a.obsidian - b.obsidian,
            a.geode - b.geode
        );
    }

    public static bool operator <=(Inventory a, Inventory b)
    {
        return
            a.ore <= b.ore &&
            a.clay <= b.clay &&
            a.obsidian <= b.obsidian &&
            a.geode <= b.geode;
    }

    public static bool operator >=(Inventory a, Inventory b)
    {
        return
            a.ore >= b.ore &&
            a.clay >= b.clay &&
            a.obsidian >= b.obsidian &&
            a.geode >= b.geode;
    }
}

record Robot(Inventory cost, Inventory produces);
record State(int remainingTime, Inventory available, Inventory produced);
record Blueprint(int id, Robot ore, Robot clay, Robot obsidian, Robot geode);