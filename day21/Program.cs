using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace day21;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var list = File.ReadLines(args[0]).ToList();
        var monkeys = list.Select(s => s[0..4]).ToList();
        var nodes = list.Where(s => s[6] >= 'a' && s[6] <= 'z').ToDictionary(s => s[0..4], s => (s[6..10], s[11].ToString(), s[13..17]));
        var leafs = list.Where(s => s[6] >= '0' && s[6] <= '9').ToDictionary(s => s[0..4], s => long.Parse(s[6..]));

        var nn = monkeys.ToDictionary(m => m, m => (Node)null);
        foreach (var n in nodes) {
            var id = n.Key;
            var left = n.Value.Item1;
            var op = n.Value.Item2;
            var right = n.Value.Item3;
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

        Console.WriteLine(nn["root"].Eval());

        var rootLeft = nn[((BinaryNode)nn["root"]).Left.Id].Eval();
        var rootRight = nn[((BinaryNode)nn["root"]).Right.Id].Eval();
        Console.WriteLine(rootLeft);
        Console.WriteLine(rootRight);
        var sb = new StringBuilder();
        Traverse(nn[((BinaryNode)nn["root"]).Left.Id], sb);
        var humn = (NumberNode)nn["humn"];
        sb = sb.Replace($"({humn.Number})", "(X)");
        sb.Append($"-{rootRight}");
        Console.WriteLine(sb.ToString());
        // Part to is manual using generated string above
        // https://www.mathpapa.com/algebra-calculator.html
        // 3087390115721
    }

    public static void Traverse(Node tree, StringBuilder sb)
    {
        sb.Append("(");
        if (tree is BinaryNode b) {
            Traverse(b.Left, sb);
        } else
            sb.Append(((NumberNode)tree).Number);

        if (tree is BinaryNode b1)
            sb.Append(b1.Op);

        if (tree is BinaryNode b2)
            Traverse(b2.Right, sb);
        sb.Append(")");
    }
}

public class BinaryNode : Node
{
    public BinaryNode(string id, Node left, Node right, string op) : base(id)
    {
        Left = left;
        Right = right;
        Op = op;
    }

    public Node Left { get; set; }
    public Node Right { get; set; }
    public string Op { get; set; }

    public override long Eval()
    {
        Func<long, long, long> op = Op switch
        {
            "+" => Add,
            "-" => Subtract,
            "*" => Multiply,
            "/" => Divide,
            _ => throw new InvalidOperationException()
        };

        return op(Left.Eval(), Right.Eval());
    }

    public override string ToString()
    {
        return $"{Id}: {Left?.Id} {Right?.Id}";
    }

    private static long Add(long a, long b) => a + b;
    private static long Subtract(long a, long b) => a - b;
    private static long Multiply(long a, long b) => a * b;
    private static long Divide(long a, long b) => a / b;
}

public class NumberNode : Node
{
    public NumberNode(string id, long number) : base(id)
    {
        Number = number;
    }

    public long Number { get; set; }

    public override long Eval()
    {
        return Number;
    }
}

public abstract class Node
{

    protected Node(string id)
    {
        Id = id;
    }
    public string Id { get; set; }

    public abstract long Eval();
}