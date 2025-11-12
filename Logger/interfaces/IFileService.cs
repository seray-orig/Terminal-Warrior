public interface IFileService
{
    bool IsFileExist(string path);
    void AppendToFile(string path, string content);
    void CreateFile(string path);
}