using CodeLineCounter;

int retval = -1;

if (args.Length == 0) {
    Console.WriteLine("No arguments found.");
} else {

    var path = args[0];

    try {
        var file_info = new FileInfo(path);
        FileAttributes attributes = File.GetAttributes(path);

        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) {
            var parser = new DirectoryParser(path);
            parser.FileList.ForEach(file => {
                Console.WriteLine($"File: {file.Path}");
                Console.WriteLine($"Lines: {file.LineCount}");
            });
            Console.WriteLine($"総行数：{parser.AllLineCount}");

        } else {
            var parser = new FileParser(path);
            Console.WriteLine($"File: {parser.Path}");
            Console.WriteLine($"Lines: {parser.LineCount}");
        }

        retval = 0;
    } catch (FileNotFoundException) {
        Console.WriteLine($"指定されたパスは存在しません。path={path}");
        retval = -2;
    }
}

return retval;
