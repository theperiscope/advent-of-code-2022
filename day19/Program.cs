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
        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    public static int Part1(string input)
    {
        var res = 0;
        foreach (var blueprint in Parse(input)) {
            res += blueprint.id * MaxGeodes(blueprint, 24);
        }
        return res;
    }

    public static int Part2(string input)
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
                let afterMining = afterFactory with {
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

        if (!CanBuild(bluePrint.geodeRobot, prev) && CanBuild(bluePrint.geodeRobot, now)) {
            yield return Build(state, bluePrint.geodeRobot);
            yield break;
        }

        if (!CanBuild(bluePrint.obsidianRobot, prev) && CanBuild(bluePrint.obsidianRobot, now)) {
            yield return Build(state, bluePrint.obsidianRobot);
        }
        if (!CanBuild(bluePrint.clayRobot, prev) && CanBuild(bluePrint.clayRobot, now)) {
            yield return Build(state, bluePrint.clayRobot);
        }
        if (!CanBuild(bluePrint.oreRobot, prev) && CanBuild(bluePrint.oreRobot, now)) {
            yield return Build(state, bluePrint.oreRobot);
        }

        yield return state;
    }

    private static bool CanBuild(Robot robot, Inventory availableMaterial) => availableMaterial >= robot.cost;

    private static State Build(State state, Robot robot) =>
        state with {
            available = state.available - robot.cost,
            produced = state.produced + robot.produces
        };

    private static IEnumerable<Blueprint> Parse(string input)
    {
        foreach (var line in input.Split("\n")) {
            var numbers = Regex.Matches(line, @"(\d+)").Select(x => int.Parse(x.Value)).ToArray();
            yield return new Blueprint(
                id: numbers[0],
                oreRobot: new Robot(
                    cost: new Inventory(ore: numbers[1], clay: 0, obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 1, clay: 0, obsidian: 0, geode: 0)
                ),
                clayRobot: new Robot(
                    cost: new Inventory(ore: numbers[2], clay: 0, obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 0, clay: 1, obsidian: 0, geode: 0)
                ),
                obsidianRobot: new Robot(
                    cost: new Inventory(ore: numbers[3], clay: numbers[4], obsidian: 0, geode: 0),
                    produces: new Inventory(ore: 0, clay: 0, obsidian: 1, geode: 0)
                ),
                geodeRobot: new Robot(
                    cost: new Inventory(ore: numbers[5], clay: 0, obsidian: numbers[6], geode: 0),
                    produces: new Inventory(ore: 0, clay: 0, obsidian: 0, geode: 1)
                )
            );
        }
    }
}

internal record Inventory(int ore, int clay, int obsidian, int geode)
{
    public static Inventory operator +(Inventory a, Inventory b) => new Inventory(a.ore + b.ore, a.clay + b.clay, a.obsidian + b.obsidian, a.geode + b.geode);
    public static Inventory operator -(Inventory a, Inventory b) => new Inventory(a.ore - b.ore, a.clay - b.clay, a.obsidian - b.obsidian, a.geode - b.geode);
    public static bool operator <=(Inventory a, Inventory b) => a.ore <= b.ore && a.clay <= b.clay && a.obsidian <= b.obsidian && a.geode <= b.geode;
    public static bool operator >=(Inventory a, Inventory b) => a.ore >= b.ore && a.clay >= b.clay && a.obsidian >= b.obsidian && a.geode >= b.geode;
}

internal record Robot(Inventory cost, Inventory produces);

internal record State(int remainingTime, Inventory available, Inventory produced);

internal record Blueprint(int id, Robot oreRobot, Robot clayRobot, Robot obsidianRobot, Robot geodeRobot);