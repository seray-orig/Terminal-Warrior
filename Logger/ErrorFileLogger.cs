using NLua;
using System.Text;

namespace Terminal_Warrior.Logger
{
    public class ErrorFileLogger : ILogger
    {
        private readonly string _path = "logs/errors.txt";
        private readonly StringBuilder _lastError = new StringBuilder();
        private readonly IFileService _fileService;
        private readonly IValidator _messageValidator;

        public ErrorFileLogger(IFileService fileService, IValidator validator)
        {
            _fileService = fileService;
            _messageValidator = validator;
        }

        public bool Log(object[] message)
        {
            if (!_messageValidator.IsValid(message))
                return false;

            if (!_fileService.IsFileExist(_path))
                _fileService.CreateFile(_path);

            var text = new StringBuilder();
            void Perebor(object obj)
            {
                switch (obj)
                {
                    case LuaTable:
                        LuaTable content = (LuaTable)obj;
                        foreach (var item in content.Values)
                            Perebor(item);
                        break;
                    case System.Collections.IEnumerable:
                        dynamic content2 = obj;
                        foreach (var item in content2)
                            Perebor(item);
                        break;
                    default:
                        text.Append(obj.ToString());
                        break;
                }
            }
            foreach (var item in message)
            {
                Perebor(item);
            }

            if (_lastError.ToString() == text.ToString()) return true;
            else
            {
                _lastError.Clear(); _lastError.Append(text.ToString());
                _fileService.AppendToFile(_path, $"\n[{DateTime.Now}] {text}");
                return true;
            }
        }
    }
}