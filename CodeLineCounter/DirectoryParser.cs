using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeLineCounter
{
    internal class DirectoryParser
    {
        public string Path {
            get;
            private set;
        }

        public int AllLineCount {
            get;
            private set;
        }

        public string Pattern {
            get;
            private set;
        }

        public List<FileParser> FileList {
            get;
            private set;
        } = new List<FileParser>();

        public DirectoryParser(string in_path, string in_pattern = @"(\.cpp|\.c|\.inl|\.inc|\.h|\.hpp|\.cs)$") {
            Path = in_path;
            Pattern = in_pattern;
            AllLineCount = 0;

            var dir_info = new DirectoryInfo(in_path);
            var files = dir_info.GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => Regex.IsMatch(f.Extension, Pattern));

            FileList.AddRange(files.Select(file => new FileParser(file.FullName)));
            AllLineCount = FileList.Sum(parser => parser.LineCount);
        }
    }
}
