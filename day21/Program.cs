namespace day21;

internal class Program
{
    private static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var input = File.ReadLines(args[0]).ToList();
        var nodes = input.Where(s => s[6] >= 'a' && s[6] <= 'z').ToDictionary(s => s[0..4], s => (s[6..10], s[11].ToString(), s[13..17]));
        var leafs = input.Where(s => s[6] >= '0' && s[6] <= '9').ToDictionary(s => s[0..4], s => long.Parse(s[6..])); // numbers are always leafs

        // build the tree as dictionary
        var tree = input.Select(s => s[0..4]).ToDictionary(m => m, m => (Node)null);
        foreach (var n in nodes) {
            var (id, (left, op, right)) = (n.Key, n.Value);
            var leftNode = leafs.ContainsKey(left) ? tree[left] ?? new NumberNode(left, leafs[left]) : tree[left] ?? new Node(left);
            var rightNode = leafs.ContainsKey(right) ? tree[right] ?? new NumberNode(right, leafs[right]) : tree[right] ?? new Node(right);
            Node node;
            if (tree[id] == null) {
                node = new Node(id) { Left = leftNode, Op = op, Right = rightNode };
            } else {
                node = tree[id];
                node!.Left = leftNode;
                node!.Right = rightNode;
                node!.Op = op;
            }
            tree[left] = leftNode;
            tree[right] = rightNode;
            tree[id] = node;
        }

        Console.WriteLine($"Part 1: {tree["root"].Eval()}");
        Console.WriteLine($"Part 2: {Part2(tree, tree[((Node)tree["root"]).Right.Id].Eval())}");
    }

    private static long Part2(Dictionary<string, Node> tree, long target) {
        var (humnParents, humn) = (GetParents(tree["humn"]), target);
        for (var i = humnParents.Count - 2 /* skip root */; i > 0; i--) {
            var parent = tree[humnParents[i].Id];
            var (parentLeftValue, parentRightValue) = (parent.Left.Eval(), parent.Right.Eval());

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

    private static List<Node> GetParents(Node currentNode) {
        var (parents, p) = (new List<Node> { currentNode }, currentNode.Parent);
        while (p != null) {
            parents.Add(p);
            p = p.Parent;
        }
        return parents; // parents will be in reverse order (root is last)
    }
}

internal class NumberNode : Node // Number nodes are leafs - their Left, Right and Op are all null
{
    public NumberNode(string id, long number) : base(id) { Number = number; }

    public long Number { get; set; }
    public override long Eval() => Number;
    public override string ToString() => $"{Id}: {Number}";

}

internal class Node
{
    private Node left, right;

    public Node(string id) {
        Id = id;
    }

    public string Id { get; set; }
    public Node Parent { get; set; }
    public string Op { get; set; }
    public Node Left { get => left; set { left = value; if (left != null) left.Parent = this; } }
    public Node Right { get => right; set { right = value; if (right != null) right.Parent = this; } }

    public virtual long Eval() {
        Func<long, long, long> op = Op switch {
            "+" => (a, b) => a + b,
            "-" => (a, b) => a - b,
            "*" => (a, b) => a * b,
            "/" => (a, b) => a / b,
            _ => throw new InvalidOperationException()
        };

        return op(Left.Eval(), Right.Eval());
    }

    public override string ToString() => $"{Id}: {Left?.Id} {Op} {Right?.Id}";
}
