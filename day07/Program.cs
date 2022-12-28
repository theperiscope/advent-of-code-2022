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

        var data = System.IO.File.ReadAllLines(args[0]).TrimTrailingEndOfLine();
        Directory root = BuildFileSystem(data);

        var (results, timings) = Perf.BenchmarkTime(() => Part1(root));
        Console.WriteLine($"Part 1: {results[0]} in {timings[0]}ms");

        (results, timings) = Perf.BenchmarkTime(() => Part2(root));
        Console.WriteLine($"Part 2: {results[0]} in {timings[0]}ms");
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
            } else if (row.StartsWith("$ cd") && row.IndexOf('/') == -1) { // not-root cd
                var dir = row[5..];
                currentDir = currentDir.Directories.First(d => d.Name == dir);
            } else if (row.StartsWith("dir")) {
                var dir = row[4..];
                currentDir.Directories.Add(new Directory(dir) { Parent = currentDir });
            } else if (row[0] is >= '0' and <= '9') {
                var space = row.IndexOf(' ');
                currentDir.Files.Add(new File(currentDir, row[(space + 1)..], long.Parse(row[..space])));
            }
        }
        return root;
    }

    private static long Part1(Directory root)
    {
        return FindFolders(root, 0).Where(x => x.Value <= 100_000).Sum(x => x.Value);
    }

    private static long Part2(Directory root)
    {
        var allFilesSize = FindAllFiles(root).Select(x => x.Size).Sum();
        var availableSpace = DISKSIZE - allFilesSize;
        var needToFree = TARGETFREESPACE - availableSpace;
        return FindFolders(root, needToFree).Min(x => x.Value);
    }

    private static List<File> FindAllFiles(Directory root)
    {
        var list = new List<File>(root.Files);
        foreach (var dir in root.Directories)
            list.AddRange(FindAllFiles(dir));
        return list;
    }

    private static Dictionary<Directory, long> FindFolders(Directory root, long minimumSize)
    {
        var candidates = new Dictionary<Directory, long>();
        var totalFileSize = FindAllFiles(root).Select(x => x.Size).Sum();
        if (totalFileSize >= minimumSize)
            candidates.Add(root, totalFileSize);
        foreach (var dir in root.Directories) {
            foreach (var result in FindFolders(dir, minimumSize))
                candidates.Add(result.Key, result.Value);
        }
        return candidates;
    }

    private class Directory
    {
        public Directory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Directories = new List<Directory>();
            Files = new List<File>();
        }

        public Directory? Parent { get; set; }
        public string Name { get; }
        public List<Directory> Directories { get; }
        public List<File> Files { get; }

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

    private class File
    {
        public File(Directory dir, string name, long size)
        {
            Dir = dir ?? throw new ArgumentNullException(nameof(dir));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
        }

        public Directory Dir { get; }
        public string Name { get; }
        public long Size { get; }

        public override string ToString()
        {
            return $"{Dir}{Name}: {Size}";
        }
    }
}