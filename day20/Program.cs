namespace day20
{
    /// <summary>
    /// Grove Positioning System
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            var list = File.ReadLines(args[0]).Select(int.Parse).ToList();

            var mixer = list.Select((value, pos) => (Value: (long)value, Position: pos)).ToArray();
            for (int i = 0; i < mixer.Length; i++)
            {
                var fromIndex = mixer.Select((el, index) => (el, index)).First(x => x.el.Position == i).index;
                var toIndex = (int)Mod(fromIndex + mixer[fromIndex].Value, mixer.Length - 1);
                Move(mixer, fromIndex, toIndex);
            }
            
            var zero = Array.FindIndex(mixer, x => x.Value == 0);

            var part1 = mixer[(zero + 1000) % mixer.Length].Value +
                   mixer[(zero + 2000) % mixer.Length].Value +
                   mixer[(zero + 3000) % mixer.Length].Value;

            Console.WriteLine(part1);

            var cycles = 10;
            var key = 811589153L;
            mixer = list.Select((value, pos) => (Value: value * key, Position: pos)).ToArray();
            for (int cycle = 0; cycle < cycles; cycle++)
            {
                for (int i = 0; i < mixer.Length; i++)
                {
                    var fromIndex = mixer.Select((el, index) => (el, index)).First(x => x.el.Position == i).index;
                    var toIndex = (int)Mod(fromIndex + mixer[fromIndex].Value, mixer.Length - 1);
                    Move(mixer, fromIndex, toIndex);
                }
            }

            zero = Array.FindIndex(mixer, x => x.Value == 0);

            var part2 = mixer[(zero + 1000) % mixer.Length].Value +
                   mixer[(zero + 2000) % mixer.Length].Value +
                   mixer[(zero + 3000) % mixer.Length].Value;

            Console.WriteLine(part2);
        }

        public static void Move<T>(T[] array, int from, int to)
        {
            if (from - to == 0) return;
            var (movedElement, length) = (array[from], from - to);
            if (length > 0)
                Array.Copy(array, to, array, to + 1, length);
            else
                Array.Copy(array, from + 1, array, from, -length);
            array[to] = movedElement;
        }

        /// <summary>
        /// Modulus that works with negative numbers
        /// </summary>
        public static long Mod(long a, long b)
        {
            var c = a % b;
            return c < 0 ? c + b : c;
        }
    }
}