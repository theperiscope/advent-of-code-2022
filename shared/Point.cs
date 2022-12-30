namespace shared;

public record Point
{
    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public int X;
    public int Y;

    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public int ManhattanDistanceTo(Point p) => Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
}