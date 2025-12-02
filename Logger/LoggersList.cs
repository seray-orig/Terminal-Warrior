using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Logger
{
    public class LoggersList : ILogger
    {
        private readonly List<ILogger> _loggers;

        public LoggersList(List<ILogger> loggers)
        {
            _loggers = loggers;
        }

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
