using System.Text;

namespace day22;

/// <summary>
/// Monkey Map
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var map = File.ReadLines(args[0]).ToList();
        var instructions = map[map.Count - 1];
        map.RemoveAt(map.Count - 1); // remove instructions
        map.RemoveAt(map.Count - 1); // and blank line, leaving only map

        if (args[0] == "input-short.txt") {
            Solve(map, Blocks, N, instructions, structureMapPart1Short);
            Solve(map, Blocks, N, instructions, structureMapCubeShort);
        } else {
            Solve(map, BigBlocks, NN, instructions, structureMapPart1);
            Solve(map, BigBlocks, NN, instructions, structureMapCube);
        }
    }

    private static void Solve(List<string> map, Dictionary<string, Block> blockMap, int blockSize, string instructions, Dictionary<string, NeighborMap> topology)
    {
        var state = new State("A", (0, 0), Dir.R);
        var ip = 0;
        while (true) {
            var sb = new StringBuilder();
            while (ip <= instructions.Length - 1 && instructions[ip] >= '0' && instructions[ip] <= '9') {
                sb.Append(instructions[ip]);
                ip++;
            }
            var n = int.Parse(sb.ToString());

            for (var i = 0; i < n; i++) {
                var stateNext = Move(topology, state, blockSize);
                var global = ToInputCoordinates(blockMap, stateNext);
                if (map[global.y][global.x] == '.') {
                    state = stateNext;
                }
            }

            if (ip >= instructions.Length - 1)
                break;

            var instructionDirection = instructions[ip];
            ip++;

            switch (instructionDirection) {
                case 'L':
                    state = state with
                    {
                        dir = state.dir switch
                        {
                            Dir.R => Dir.U,
                            Dir.D => Dir.R,
                            Dir.L => Dir.D,
                            Dir.U => Dir.L,
                            _ => throw new NotImplementedException(),
                        }
                    };
                    break;
                case 'R':
                    state = state with
                    {
                        dir = state.dir switch
                        {
                            Dir.R => Dir.D,
                            Dir.D => Dir.L,
                            Dir.L => Dir.U,
                            Dir.U => Dir.R,
                            _ => throw new NotImplementedException(),
                        }
                    };
                    break;
            }
        }

        Console.WriteLine(1000 * (ToInputCoordinates(blockMap, state).y + 1) + 4 * (ToInputCoordinates(blockMap, state).x + 1) + state.dir);
    }

    private static State Move(Dictionary<string, NeighborMap> structureMap, State state, int blockSize)
    {
        bool isOutsideCurrentBlock((int x, int y) p) => p.x < 0 || p.x >= blockSize || p.y < 0 || p.y >= blockSize;
        var (startBlock, p, dir) = state;
        var endBlock = startBlock;
        p = dir switch
        {
            Dir.R => p with { x = p.x + 1 },
            Dir.D => p with { y = p.y + 1 },
            Dir.L => p with { x = p.x - 1 },
            Dir.U => p with { y = p.y - 1 },
            _ => throw new InvalidOperationException()
        };

        if (isOutsideCurrentBlock(p)) {
            var mapping = structureMap[startBlock];
            var directionalNeighbor = dir switch
            {
                Dir.R => mapping.right,
                Dir.D => mapping.down,
                Dir.L => mapping.left,
                Dir.U => mapping.up,
                _ => throw new InvalidOperationException(),
            };

            endBlock = directionalNeighbor.name;

            var rotate = directionalNeighbor.wrapRightRotations;

            p = ((p.x + blockSize) % blockSize, (p.y + blockSize) % blockSize);

            for (var i = 0; i < rotate; i++) {
                p = p with { y = p.x, x = blockSize - p.y - 1 };
                dir = dir switch
                {
                    Dir.R => Dir.D,
                    Dir.D => Dir.L,
                    Dir.L => Dir.U,
                    Dir.U => Dir.R,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        return new State(endBlock, p, dir);
    }

    // block definition that matches input-short.txt
    public const int N = 4;
    public static Block A = new("A", (N * 2, 0));
    public static Block B = new("B", (0, N));
    public static Block C = new("C", (N, N));
    public static Block D = new("D", (N * 2, N));
    public static Block E = new("E", (N * 2, N * 2));
    public static Block F = new("F", (N * 3, N * 2));
    public static Dictionary<string, Block> Blocks = new() { { "A", A }, { "B", B }, { "C", C }, { "D", D }, { "E", E }, { "F", F } };

    // block definition that matches input.txt
    public const int NN = 50;
    public static Block AA = new("A", (NN, 0));
    public static Block BB = new("B", (NN * 2, 0));
    public static Block CC = new("C", (NN, NN));
    public static Block DD = new("D", (0, NN * 2));
    public static Block EE = new("E", (NN, NN * 2));
    public static Block FF = new("F", (0, NN * 3));
    public static Dictionary<string, Block> BigBlocks = new() { { "A", AA }, { "B", BB }, { "C", CC }, { "D", DD }, { "E", EE }, { "F", FF } };

    private static (int x, int y) ToInputCoordinates(Dictionary<string, Block> blocks, State state) => (blocks[state.block].from.x + state.cursor.x, blocks[state.block].from.y + state.cursor.y);

    private static Dictionary<string, NeighborMap> structureMapPart1Short = new()
    {
        { "A", new(new("A", 0), new("C", 0), new("A", 0), new("E", 0)) },
        { "B", new(new("C", 0), new("B", 0), new("D", 0), new("B", 0)) },
        { "C", new(new("D", 0), new("C", 0), new("B", 0), new("C", 0)) },
        { "D", new(new("B", 0), new("E", 0), new("C", 0), new("A", 0)) },
        { "E", new(new("F", 0), new("A", 0), new("F", 0), new("D", 0)) },
        { "F", new(new("E", 0), new("F", 0), new("E", 0), new("F", 0)) }
    };

    private static Dictionary<string, NeighborMap> structureMapPart1 = new()
    {
        { "A", new(new("B", 0), new("C", 0), new("B", 0), new("E", 0)) },
        { "B", new(new("A", 0), new("B", 0), new("A", 0), new("B", 0)) },
        { "C", new(new("C", 0), new("E", 0), new("C", 0), new("A", 0)) },
        { "D", new(new("E", 0), new("F", 0), new("E", 0), new("F", 0)) },
        { "E", new(new("D", 0), new("A", 0), new("D", 0), new("C", 0)) },
        { "F", new(new("F", 0), new("D", 0), new("F", 0), new("D", 0)) }
    };

    private static Dictionary<string, NeighborMap> structureMapCubeShort = new()
    {
        { "A", new(new("F", 2), new("D", 0), new("C", 3), new("B", 2)) },
        { "B", new(new("C", 0), new("E", 2), new("F", 1), new("A", 2)) },
        { "C", new(new("D", 0), new("E", 3), new("B", 0), new("A", 1)) },
        { "D", new(new("F", 1), new("E", 0), new("C", 0), new("A", 0)) },
        { "E", new(new("F", 0), new("B", 2), new("C", 1), new("D", 0)) },
        { "F", new(new("A", 2), new("B", 3), new("E", 0), new("D", 3)) }
    };

    private static Dictionary<string, NeighborMap> structureMapCube = new()
    {
        { "A", new(new("B", 0), new("C", 0), new("D", 2), new("F", 1)) },
        { "B", new(new("E", 2), new("C", 1), new("A", 0), new("F", 0)) },
        { "C", new(new("B", 3), new("E", 0), new("D", 3), new("A", 0)) },
        { "D", new(new("E", 0), new("F", 0), new("A", 2), new("C", 1)) },
        { "E", new(new("B", 2), new("F", 1), new("D", 0), new("C", 0)) },
        { "F", new(new("E", 3), new("B", 0), new("A", 3), new("D", 0)) }
    };
}

internal enum Dir { R = 0, D = 1, L = 2, U = 3 }
internal record Block(string name, (int x, int y) from);
internal record State(string block, (int x, int y) cursor, Dir dir);

internal record NeighborMap(Neighbor right, Neighbor down, Neighbor left, Neighbor up);
internal record Neighbor(string name, int wrapRightRotations);