using System.Data;

namespace day21;

internal class Program
{
    static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var list = File.ReadLines(args[0]).ToList();
        var monkeys = list.Select(s => s[0..4]).ToList();
        var nodes = list.Where(s => s[6] >= 'a' && s[6] <= 'z').ToDictionary(s => s[0..4], s => (s[6..10], s[11].ToString(), s[13..17]));
        var leafs = list.Where(s => s[6] >= '0' && s[6] <= '9').ToDictionary(s => s[0..4], s => long.Parse(s[6..]));

        // build the tree as dictionary
        var nn = monkeys.ToDictionary(m => m, m => (Node)null);
        foreach (var n in nodes) {
            var id = n.Key;
            var (left, op, right) = n.Value;
            var isLeftANumber = leafs.ContainsKey(left);
            var isRightANumber = leafs.ContainsKey(right);
            var leftNode = isLeftANumber ? nn[left] ?? new NumberNode(left, leafs[left]) : nn[left] ?? new BinaryNode(left, null, null, null);
            var rightNode = isRightANumber ? nn[right] ?? new NumberNode(right, leafs[right]) : nn[right] ?? new BinaryNode(right, null, null, null);
            Node node;
            if (nn[id] == null) {
                node = new BinaryNode(id, leftNode, rightNode, op);
            } else {
                node = nn[id];
                ((BinaryNode)node).Left = leftNode;
                ((BinaryNode)node).Right = rightNode;
                ((BinaryNode)node).Op = op;
            }
            nn[left] = leftNode;
            nn[right] = rightNode;
            nn[id] = node;
        }

        Console.WriteLine($"Part 1: {nn["root"].Eval()}");
        Console.WriteLine($"Part 2: {Part2(nn, nn[((BinaryNode)nn["root"]).Right.Id].Eval())}");
    }

    public static long Part2(Dictionary<string, Node> tree, long target) {
        var humnParents = GetParents(tree, tree["humn"]);
        var humn = target;
        for (int i = humnParents.Count - 1; i > 0; i--) {
            var parent = (BinaryNode)tree[humnParents[i].Id];
            var parentValue = parent.Eval();
            var parentLeftValue = parent.Left.Eval();
            var parentRightValue = parent.Right.Eval();

            var childValue = tree[humnParents[i - 1].Id].Eval();
            var (theOtherChildValue, isLeft) = childValue == parentLeftValue ? (parentRightValue, true) : (parentLeftValue, false);

            humn = parent.Op switch {
                "+" => humn - theOtherChildValue,
                "*" => humn / theOtherChildValue,
                "-" => !isLeft ? theOtherChildValue - humn : humn + theOtherChildValue,
                "/" => !isLeft ? theOtherChildValue / humn : humn * theOtherChildValue,
                _ => throw new Exception(),
            };
        }
        return humn;
    }

    private static List<Node> GetParents(Dictionary<string, Node> tree, Node currentNode) {
        var parents = new List<Node> { currentNode };
        var p = GetParent(tree, currentNode);
        while (p != null && p.Id != "root") {
            parents.Add(p);
            p = GetParent(tree, p);
        }
        return parents;
    }
    private static Node GetParent(Dictionary<string, Node> tree, Node currentNode) => tree.Values.FirstOrDefault(n => (n is BinaryNode) && (((BinaryNode)n).Left.Id == currentNode.Id || ((BinaryNode)n).Right.Id == currentNode.Id));

    private static List<string> GetParents(Node root, string targetNode) {
        if (root == null) return new List<string>();
        if (root.Id == targetNode) return new List<string> { root.Id };

        var parents = new List<string>();
        if (root is BinaryNode node) {
            var result = GetParents(node.Left, targetNode);
            if (result.Count > 0) parents.AddRange(result);
            result = GetParents(node.Right, targetNode);
            if (result.Count > 0) parents.AddRange(result);
        } else if (root is NumberNode numberNode) {
            if (numberNode.Id == targetNode) parents.Add(numberNode.Id);
        }

        return parents;
    }
}

public class BinaryNode : Node
{
    public BinaryNode(string id, Node left, Node right, string op) : base(id) {
        Left = left;
        Right = right;
        Op = op;
    }

    public Node Left { get; set; }
    public Node Right { get; set; }
    public string Op { get; set; }

    public override long Eval() {
        Func<long, long, long> op = Op switch {
            "+" => Add,
            "-" => Subtract,
            "*" => Multiply,
            "/" => Divide,
            _ => throw new InvalidOperationException()
        };

        return op(Left.Eval(), Right.Eval());
    }

    private static long Add(long a, long b) => a + b;
    private static long Subtract(long a, long b) => a - b;
    private static long Multiply(long a, long b) => a * b;
    private static long Divide(long a, long b) => a / b;
    public override string ToString() => $"{Id}: {Left?.Id} {Op} {Right?.Id}";
}

public class NumberNode : Node
{
    public NumberNode(string id, long number) : base(id) { Number = number; }

    public long Number { get; set; }
    public override long Eval() => Number;
    public override string ToString() => $"{Id}: {Number}";
}

public abstract class Node
{
    protected Node(string id) { Id = id; }

    public string Id { get; set; }
    public abstract long Eval();
    public override string ToString() => $"{Id}";
}