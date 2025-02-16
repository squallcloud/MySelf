namespace CodeLineCounter
{
    internal class FileParser
    {
        public string Path {
            get;
            private set;
        }

        public int LineCount {
            get;
            private set;
        }

        public FileParser(string in_path) {
            var lines = File.ReadAllLines(in_path).ToList();
            LineCount = lines.Count;
            Path = in_path;
        }
    }
}
