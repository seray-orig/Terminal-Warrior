

namespace Terminal_Warrior.Logger
{
    public class FileSystemService : IFileService
    {
        public bool IsFileExist(string path) => File.Exists(path);
        public void AppendToFile(string path, string content) => File.AppendAllText(path, content);
        public void CreateFile(string path) => File.Create(path).Close();
    }
}