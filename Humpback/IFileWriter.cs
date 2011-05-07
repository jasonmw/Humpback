using System;
using System.Collections.Generic;
using System.Linq;

namespace Humpback {
    public interface IFileWriter {
        void WriteFile(string path, string contents);
        bool FileExists(string path);
    }
}
