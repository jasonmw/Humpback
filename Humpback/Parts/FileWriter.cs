using Humpback.Interfaces;
using System.Text;

namespace Humpback
{
    class FileWriter : IFileWriter
    {
        public void WriteFile(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents, Encoding.UTF8);
        }

        public bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}