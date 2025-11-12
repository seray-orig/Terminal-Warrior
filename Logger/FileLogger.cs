

namespace Terminal_Warrior.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _path;
        private readonly IFileService _fileService;
        private readonly IMessageValidator _messageValidator;

        public FileLogger(string path, IFileService fileService, IMessageValidator validator)
        {
            _path = path;
            _fileService = fileService;
            _messageValidator = validator;
        }

        public bool Log(string message)
        {
            if (!_messageValidator.IsValid(message))
                return false;

            if (!_fileService.IsFileExist(_path))
                _fileService.CreateFile(_path);

            _fileService.AppendToFile(_path, $"\n[{DateTime.Now}] {message}");
            return true;
        }
    }
}