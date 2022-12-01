namespace shared
{
    public static class Utils
    {

        /// <summary>
        /// Read all lines from file one-by-one to avoid storing all lines in memory
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
    }
}
