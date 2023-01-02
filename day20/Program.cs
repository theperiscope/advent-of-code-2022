using shared;

namespace day20
{
    /// <summary>
    /// Grove Positioning System
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var list = File.ReadLines(args[0]).Select(int.Parse).ToList();

            var mixer = list.Select((value, pos) => (Value: (long)value, Position: pos)).ToArray();
            for (var i = 0; i < mixer.Length; i++) {
                var fromIndex = mixer.Select((el, index) => (el, index)).First(x => x.el.Position == i).index;
                var toIndex = (int)Utils.Mod(fromIndex + mixer[fromIndex].Value, mixer.Length - 1);
                Utils.Move(mixer, fromIndex, toIndex);
            }

            var zero = Array.FindIndex(mixer, x => x.Value == 0);
            Console.WriteLine(mixer[(zero + 1000) % mixer.Length].Value + mixer[(zero + 2000) % mixer.Length].Value + mixer[(zero + 3000) % mixer.Length].Value);

            var (cycles, key) = (10, 811589153L);
            mixer = list.Select((value, pos) => (Value: value * key, Position: pos)).ToArray();
            for (int cycle = 0; cycle < cycles; cycle++) {
                for (var i = 0; i < mixer.Length; i++) {
                    var fromIndex = mixer.Select((el, index) => (el, index)).First(x => x.el.Position == i).index;
                    var toIndex = (int)Utils.Mod(fromIndex + mixer[fromIndex].Value, mixer.Length - 1);
                    Utils.Move(mixer, fromIndex, toIndex);
                }
            }

            zero = Array.FindIndex(mixer, x => x.Value == 0);
            Console.WriteLine(mixer[(zero + 1000) % mixer.Length].Value + mixer[(zero + 2000) % mixer.Length].Value + mixer[(zero + 3000) % mixer.Length].Value);
        }
    }
}