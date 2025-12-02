using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Logger.Util;

namespace Terminal_Warrior.Logger
{
    public class LoggerFactory : IFactory<ILogger>
    {
        public ILogger Create()
        {
            return new LoggersList(new List<ILogger> {
                new ErrorFileLogger(new FileSystemService(), new MessageValidator()),
                new ErrorCmdLogger(new MessageValidator()),
            });
        }
    }
}
