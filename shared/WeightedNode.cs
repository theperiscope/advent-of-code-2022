namespace shared;

public class WeightedNode : IComparable<WeightedNode>
{
    public WeightedNode(int vertex, int weight)
    {
        Vertex = vertex;
        Weight = weight;
    }

    public int Vertex { get; init; }
    public int Weight { get; init; }

    public int CompareTo(WeightedNode other)
    {
        return Weight - other.Weight;
    }
}
