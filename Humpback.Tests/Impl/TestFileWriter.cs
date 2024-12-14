using Humpback.Interfaces;
using System;

namespace Humpback.Tests.Impl
{
    public class TestFileWriter : IFileWriter
    {
        public void WriteFile(string path, string contents)
        {
            FileContents += contents;
            FileName += path;
            Console.WriteLine(path);
        }

        public bool FileExists(string path)
        {
            return false;
        }

        public string FileName { get; private set; }
        public string FileContents { get; private set; }

    }
}
