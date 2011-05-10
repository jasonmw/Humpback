namespace Humpback.Interfaces {
    public interface IFileWriter {
        void WriteFile(string path, string contents);
        bool FileExists(string path);
    }
}
