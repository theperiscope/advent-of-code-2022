using System.Runtime.CompilerServices;

namespace day13;

/// <summary>
/// Distress Signal
/// </summary>
internal class Program
{
    public static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            return;
        }

        var packetPairs = File.ReadAllLines(args[0]).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        var sum = packetPairs.Chunk(2).Select((pair, i) => Compare(pair[0], pair[1]) == -1 ? i + 1 : 0).Where(x => x != 0).Sum();
        Console.WriteLine($"Part 1: {sum}");

        packetPairs.AddRange(new[] { "[[2]]", "[[6]]" });
        packetPairs.Sort(Compare);
        Console.WriteLine($"Part 2: {(packetPairs.IndexOf("[[2]]") + 1) * (packetPairs.IndexOf("[[6]]") + 1)}");
    }

    /// <summary>
    /// Check if character is ASCII 0-9. <see cref="char.IsDigit(char)"/> is intentionally not used 
    /// as it includes Unicode characters
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    private static string Listify(string s) => s.All(IsDigit) ? $"[{s}]" : s;

    private static (string value, string remaining) GetNextItem(string packet) {
        if (IsDigit(packet[0])) {
            var number = new string(packet.TakeWhile(IsDigit).ToArray());
            var rem = number.Length == packet.Length ? number.Length : number.Length + 1;
            return (number, packet[rem..]);
        }

        // list: full item content will be at level 0 nestingLevel and finish at either end of string or comma
        // [1, [2, 3]] or [1, 2, 3], 4, 5
        var (nestingLevel, i) = (0, 0);
        while (nestingLevel > 0 || (i != packet.Length && packet[i] != ',')) {
            switch (packet[i]) {
                case '[':
                    nestingLevel++;
                    break;
                case ']':
                    nestingLevel--;
                    break;
            }
            i++;
        }

        return i == packet.Length ? (packet, string.Empty) : (packet[..i], packet[(i + 1)..]);
    }
    private static int Compare(string firstPacket, string secondPacket) {
        while (firstPacket.Length > 0 && secondPacket.Length > 0) {
            var (firstPacketCurrentItem, firstPacketRemaining) = GetNextItem(firstPacket);
            var (secondPacketCurrentItem, secondPacketRemaining) = GetNextItem(secondPacket);
            if (firstPacketCurrentItem.All(IsDigit) && secondPacketCurrentItem.All(IsDigit)) {
                var (nFirst, nSecond) = (int.Parse(firstPacketCurrentItem), int.Parse(secondPacketCurrentItem));
                if (nFirst < nSecond)
                    return -1;
                else if (nFirst > nSecond)
                    return 1;
            } else {
                (firstPacketCurrentItem, secondPacketCurrentItem) = (Listify(firstPacketCurrentItem), Listify(secondPacketCurrentItem));
                var result = Compare(firstPacketCurrentItem[1..^1], secondPacketCurrentItem[1..^1]);
                if (result != 0)
                    return result;
            }
            (firstPacket, secondPacket) = (firstPacketRemaining, secondPacketRemaining);
        }

        return secondPacket.Length > 0 ? -1 : firstPacket.Length > 0 ? 1 : 0;
    }
}