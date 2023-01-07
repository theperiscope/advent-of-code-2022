# 2022 Advent of Code Solutions

Code is written in C# and .NET 6 mostly as an opportunity to newer language features (e.g. pattern matching, tuple types).

## Challenges

### December 2022

| Mon | Tue | Wed | Thu | Fri | Sat | Sun |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
|     |     |     |   [1](https://adventofcode.com/2022/day/1) |   [2](https://adventofcode.com/2022/day/2) |   [3](https://adventofcode.com/2022/day/3) |   [4](https://adventofcode.com/2022/day/4) |
|   [5](https://adventofcode.com/2022/day/5) |   [6](https://adventofcode.com/2022/day/6) |   [7](https://adventofcode.com/2022/day/7) |   [8](https://adventofcode.com/2022/day/8) |   [9](https://adventofcode.com/2022/day/9) |  [10](https://adventofcode.com/2022/day/10) |  [11](https://adventofcode.com/2022/day/11) |
|  [12](https://adventofcode.com/2022/day/12) |  [13](https://adventofcode.com/2022/day/13) |  [14](https://adventofcode.com/2022/day/14) |  [15](https://adventofcode.com/2022/day/15) |  [16](https://adventofcode.com/2022/day/16) |  [17](https://adventofcode.com/2022/day/17) |  [18](https://adventofcode.com/2022/day/18) |
|  [19](https://adventofcode.com/2022/day/19) |  [20](https://adventofcode.com/2022/day/20) |  [21](https://adventofcode.com/2022/day/21) |  [22](https://adventofcode.com/2022/day/22) |  [23](https://adventofcode.com/2022/day/23) |  [24](https://adventofcode.com/2022/day/24) |  [25](https://adventofcode.com/2022/day/25) |
|  26 |  27 |  28 |  29 |  30 |  31 | &nbsp; |

### Interesting Code and Algorithms

* Day 1: [DescendingComparer](https://github.com/theperiscope/advent-of-code-2022/blob/main/day01/Program.cs#L34)
* Day 1: [ArraySegment use and array split using predicate](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Utils.cs#L55)
* Day 1: [A note about SortedList and unique key requirements](https://github.com/theperiscope/advent-of-code-2022/blob/main/day01/Program.cs#L18)
* Day 2: [File.ReadLines instead of File.ReadAllLines](https://github.com/theperiscope/advent-of-code-2022/blob/main/day02/Program.cs#L33)
* Day 3: [Singe-variable bitmap to track rucksack items](https://github.com/theperiscope/advent-of-code-2022/blob/main/day03/Program.cs#L41)
* Day 4: [Numberic interval operations](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Interval.cs)
* Day 6: [Unique character counting using bitmask resulting in 20x performance improvement over LINQ usage](https://github.com/theperiscope/advent-of-code-2022/commit/a8bd8e9b63aad69afcef23546046ee9c3b1c4548)
* Day 9: [20x faster by using HashSet.UnionWith](https://github.com/theperiscope/advent-of-code-2022/commit/2cf8723d02b63c31d0ca4cdd38c134ba119b13a6)
* Day 11: [ICloneable, list cloning to prevent double file reading](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Utils.cs#L24)
* Day 12: [Parsing into array that has 1-element border on all sides to make finding neighbors easier](https://github.com/theperiscope/advent-of-code-2022/blob/main/day12/Program.cs#L36)
* Day 12: [BFS path-finding from one origin to other destinations](https://github.com/theperiscope/advent-of-code-2022/blob/main/day12/Program.cs#L82)
* Day 13: [Parsing expressions, accounting for nesting levels](https://github.com/theperiscope/advent-of-code-2022/blob/main/day13/Program.cs#L34)
* Day 13: [IsDigit vs char.IsDigit](https://github.com/theperiscope/advent-of-code-2022/blob/main/day13/Program.cs#L30)
* Day 15: [Interval merge algorithm](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Interval.cs#L24)
* Day 15: [Parallel.ForEach and ConcurrentQueue for faster execution](https://github.com/theperiscope/advent-of-code-2022/blob/main/day15/Program.cs#L53)
* Day 15: [Manhattan distance between points](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Point.cs#L15)
* Day 16: [Dijkstra algorithm to find shortest times needed from source to all other valves](https://github.com/theperiscope/advent-of-code-2022/blob/main/day16/Program.cs#L63)
* Day 16: [Use of .NET's PriorityQueue](https://github.com/theperiscope/advent-of-code-2022/blob/main/day16/Program.cs#L70)
* Day 17: [Finding a pattern in array of numbers](https://github.com/theperiscope/advent-of-code-2022/blob/main/day17/Program.cs#L91)
* Day 18: [Expanding a 3D shape by 1, filling voids by exploring spaces between](https://github.com/theperiscope/advent-of-code-2022/blob/main/day18/Program.cs)
* Day 18: [int-based X/Y/Z-coordinates for faster searching](https://github.com/theperiscope/advent-of-code-2022/blob/main/day18/Program.cs#L113)
* Day 19: [Searching for best solution using specified costs](https://github.com/theperiscope/advent-of-code-2022/blob/main/day19/Program.cs)
* Day 20: [Modulus function for negative numbers](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Utils.cs#L43)
* Day 20: [Moving array element from one position to another while shifting others](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Utils.cs#L30)
* Day 21: [Evaluating expression represented in binary tree](https://github.com/theperiscope/advent-of-code-2022/blob/main/day21/Program.cs#L93)
* Day 21: [Reversing a path in binary tree to find needed value](https://github.com/theperiscope/advent-of-code-2022/blob/main/day21/Program.cs#L40)
* Day 23: [Parallel.ForEach for 75% faster execution](https://github.com/theperiscope/advent-of-code-2022/commit/f111e9542a5988cc07e916d1e6991e16b1d27a96)
* Day 24: [50x faster by pre-calculating blizzard locations](https://github.com/theperiscope/advent-of-code-2022/commit/cb03b1ef292b380acc343d108f43f15cab2480de)
* Day 24: [.NET PriorityQueue and A* algorithm to find best parh to single destination](https://github.com/theperiscope/advent-of-code-2022/blob/main/day24/Program.cs#L53)
* [Performance Timing](https://github.com/theperiscope/advent-of-code-2022/blob/main/shared/Perf.cs)
