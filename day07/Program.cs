using shared;
using System.Text;

namespace day07;

/// <summary>
/// No Space Left On Device
/// </summary>
internal class Program
{
    private const long DISKSIZE = 70_000_000L;
    private const long TARGETFREESPACE = 30_000_000L;

    private static void Main(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var (input, timings) = Perf.BenchmarkTime(() => {
            var data = System.IO.File.ReadAllLines(args[0]).TrimTrailingEndOfLine();
            Directory root = BuildFileSystem(data);
            return root;
        });

        Directory root = input[0];
        Console.WriteLine($"Parsing: {root.Size} root folder size in {timings[0]}ms");

        var (results, timings1) = Perf.BenchmarkTime(() => Part1(root));
        Console.WriteLine($"Part 1 : {results[0]} in {timings1[0]}ms");

        (results, timings1) = Perf.BenchmarkTime(() => Part2(root));
        Console.WriteLine($"Part 2 : {results[0]} in {timings1[0]}ms");
    }

    private static Directory BuildFileSystem(string[] data)
    {
        var currentDir = new Directory(string.Empty) { Parent = null };
        var root = currentDir;
        for (var i = 0; i < data.Length; i++) {
            var row = data[i];
            // "$ ls" command does nothing of value so it is not included below
            if (row == "$ cd /") {
                currentDir = root;
            } else if (row == "$ cd ..") {
                var dir = row[5..];
                currentDir = currentDir.Parent!;
            } else if (row.StartsWith("$ cd")) { // not cd / or cd ..
                var dir = row[5..];
                currentDir = currentDir.Children.First(d => d.Name == dir);
            } else if (row.StartsWith("dir")) {
                var dir = row[4..];
                currentDir.Children.Add(new Directory(dir) { Parent = currentDir });
            } else if (row[0] is >= '0' and <= '9') {
                var space = row.IndexOf(' ');
                // having a file increases current folder size and also all its parents
                var p = currentDir;
                while (p != null) {
                    p.Size += long.Parse(row[..space]);
                    p = p.Parent;
                }
            }
        }
        return root;
    }

    private static long Part1(Directory root) => FindFolders(root, 0).Where(x => x.Size <= 100_000).Sum(x => x.Size);
    private static long Part2(Directory root) => FindFolders(root, TARGETFREESPACE - (DISKSIZE - root.Size)).Min(x => x.Size);

    private static List<Directory> FindFolders(Directory root, long minimumSize)
    {
        var candidates = new List<Directory>();
        var totalFileSize = root.Size;
        if (totalFileSize >= minimumSize)
            candidates.Add(root);
        foreach (var dir in root.Children) {
            foreach (var result in FindFolders(dir, minimumSize))
                candidates.Add(result);
        }
        return candidates;
    }

    private class Directory
    {
        public Directory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Children = new List<Directory>();
            Size = 0L;
        }

        public Directory? Parent { get; set; }
        public string Name { get; }
        public List<Directory> Children { get; }
        public long Size { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var n = this;
            sb.Insert(0, $"{n.Name}/");
            while (n.Parent != null) {
                n = n.Parent;
                sb.Insert(0, $"{n.Name}/");
            }
            return sb.ToString();
        }
    }
}