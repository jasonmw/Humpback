using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.Tests.Impl {
    public class TestFileWriter:IFileWriter {
        public void WriteFile(string path, string contents) {
            FileContents += contents;
            FileName = path;
            Console.WriteLine(path);
        }

        public string FileName { get; private set; }
        public string FileContents { get; private set; }
        
    }
}
