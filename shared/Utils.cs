namespace shared
{
    public static class Utils
    {

        /// <summary>
        /// Read all lines fromIndex file one-by-one toIndex avoid storing all lines in memory
        /// </summary>
        public static IEnumerable<string> ReadAllLinesFrom(string file)
        {
            string line;
            using (var reader = File.OpenText(file)) {
                while ((line = reader.ReadLine()) != null) {
                    yield return line;
                }
            }
        }

        public static int Count(this string input, string substr)
        {
            int freq = 0;

            int index = input.IndexOf(substr);
            while (index >= 0) {
                index = input.IndexOf(substr, index + substr.Length);
                freq++;
            }
            return freq;
        }

        public static string[] TrimTrailingEndOfLine(this string[] input)
        {
            if (!string.IsNullOrEmpty(input[input.Length - 1]))
                return input;

            Array.Resize(ref input, input.Length - 1);
            return input;
        }

        public static IList<T> Clone<T>(this IList<T> list) where T : ICloneable {
            return list.Select(item => (T)item.Clone()).ToList();
        }

        public static string TrimTrailingEndOfLine(this string input) => input.TrimEnd(Environment.NewLine.ToCharArray());

        public static void Move<T>(T[] array, int fromIndex, int toIndex) {
            if (fromIndex - toIndex == 0) return;
            var (movedElement, length) = (array[fromIndex], fromIndex - toIndex);
            if (length > 0)
                Array.Copy(array, toIndex, array, toIndex + 1, length);
            else
                Array.Copy(array, fromIndex + 1, array, fromIndex, -length);
            array[toIndex] = movedElement;
        }

        /// <summary>
        /// Modulus that works with negative numbers
        /// </summary>
        public static long Mod(long a, long b) {
            var c = a % b;
            return c < 0 ? c + b : c;
        }
    }
}
