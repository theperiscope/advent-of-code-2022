namespace shared
{
    public record Interval : IComparable<Interval>
    {
        public Interval(int start, int end)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException(nameof(start));

            Start = start;
            End = end;
        }

        public int Start { get; set; }
        public int End { get; set; }

        public int CompareTo(Interval? other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            return Start < other.Start ? -1 : Start > other.Start ? 1 : 0;
        }

        public static List<Interval> Merge(List<Interval> intervals)
        {
            if (intervals is null)
                throw new ArgumentNullException(nameof(intervals));
            if (intervals.Count == 0)
                return new List<Interval>();

            var s = new Stack<Interval>();
            intervals.Sort();
            s.Push(intervals[0]);
            for (var i = 1; i < intervals.Count; i++) {
                var top = s.Peek();
                if (top.End < intervals[i].Start) {
                    s.Push(intervals[i]);
                } else if (top.End < intervals[i].End) {
                    top.End = intervals[i].End;
                    s.Pop();
                    s.Push(top);
                }
            }

            return s.ToList();
        }

        public bool IsFullyContained(Interval other) => (Start >= other.Start && End <= other.End);

        public Interval? Overlap(Interval other) => IsOverlap(other) ? new Interval(Math.Max(Start, other.Start), Math.Min(End, other.End)) : null;

        public bool IsOverlap(Interval other) => other.Start <= End && Start <= other.End;

        public override string ToString() => $"[{Start}-{End}]";
    }
}
