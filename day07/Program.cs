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

        var data = System.IO.File.ReadAllLines(args[0]);

        Directory root = BuildFileSystem(data);
        Part1(root);
        Part2(root);
    }

    private static Directory BuildFileSystem(string[] data)
    {
        Directory root = null;
        Directory currentDir = null;
        for (var i = 0; i < data.Length; i++) {
            if (data[i].StartsWith("$ cd")) { // command
                var dir = data[i].Substring(5);
                if (dir == "/") {
                    currentDir = new Directory(string.Empty) { Parent = null };
                    root = currentDir;
                } else if (dir == "..") {
                    if (currentDir == null || currentDir.Parent == null)
                        throw new InvalidOperationException();
                    currentDir = currentDir.Parent;
                } else {
                    currentDir = currentDir.Directories.First(d => d.Name == dir);
                }
            } else if (data[i] == "$ ls") {
            } else if (data[i].StartsWith("dir")) {
                var dir = data[i].Substring(4);
                if (currentDir == null)
                    throw new InvalidOperationException();
                currentDir.Directories.Add(new Directory(dir) { Parent = currentDir });
            } else if (data[i][0] is >= '0' and <= '9') {
                var fileSize = long.Parse(data[i].Substring(0, data[i].IndexOf(' ')));
                var fileName = data[i].Substring(data[i].IndexOf(' ') + 1);
                if (currentDir == null)
                    throw new InvalidOperationException();
                currentDir.Files.Add(new File(currentDir, fileName, fileSize));
            }
        }
        return root;
    }

    private static void Part1(Directory root)
    {
        Console.WriteLine(SearchFolders100KOrLess(root));
    }

    private static long SearchFolders100KOrLess(Directory root)
    {
        var total = 0L;
        var totalFileSize = SearchFiles(root).Select(x => x.Size).Sum();
        if (totalFileSize <= 100_000) {
            total += totalFileSize;
        }
        foreach (var dir in root.Directories) {
            total += SearchFolders100KOrLess(dir);
        }
        return total;
    }

    private static List<File> SearchFiles(Directory root)
    {
        if (root == null)
            return new List<File>();

        var list = new List<File>();
        list.AddRange(root.Files);
        foreach (var dir in root.Directories) {
            var files = SearchFiles(dir);
            if (files.Count > 0)
                list.AddRange(files);
        }

        return list;
    }

    private static void Part2(Directory root)
    {
        var allFilesSize = SearchFiles(root).Select(x => x.Size).Sum();
        var availableSpace = DISKSIZE - allFilesSize;
        var needToFree = TARGETFREESPACE - availableSpace;
        Console.WriteLine(SearchFolders(root, needToFree).Min(x => x.Value));
    }

    private static Dictionary<Directory, long> SearchFolders(Directory root, long minimumSize)
    {
        var candidates = new Dictionary<Directory, long>();
        var files = SearchFiles(root);
        var totalFileSize = files.Select(x => x.Size).Sum();
        if (totalFileSize >= minimumSize) {
            candidates.Add(root, totalFileSize);
        }
        foreach (var dir in root.Directories) {
            var results = SearchFolders(dir, minimumSize);
            foreach (var result in results) {
                candidates.Add(result.Key, result.Value);
            }
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