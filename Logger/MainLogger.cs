/*
 * Main Logger
 * 
 * Хранитель всех логгеров, сам является логгером
 * При вызове проходится по списку логгеров и отправляет им всем аргументы.
 * 
 * Чтобы добавить логгер достаточно создать его в списке.
 */

using Terminal_Warrior.Logger.Util;

namespace Terminal_Warrior.Logger
{
    public class MainLogger : ILogger
    {
        private readonly List<ILogger> _loggers = new()
        {
            new ErrorFileLogger(new FileSystemService(), new MessageValidator()),

            new ErrorCmdLogger(new MessageValidator()),
        };

        public bool Log(object[] message)
        {
            foreach (var logger in _loggers)
            {
                if (!logger.Log(message))
                    return false;
            }

            return true;
        }
    }
}
