using NLua;

namespace Terminal_Warrior.Logger
{
    public class ErrorCmdLogger : ILogger
    {
        private readonly IValidator _messageValidator;

        public ErrorCmdLogger(IValidator messageValidator)
        {
            _messageValidator = messageValidator;
        }

        public bool Log(object[] message)
        {
            if (!_messageValidator.IsValid(message))
                return false;

            try { Console.SetCursorPosition(1, 1); }
            finally
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("/!\\");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" Что-то создаёт скриптовые ошибки.");
                Console.ResetColor();
                Console.WriteLine();
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
                            Console.Write(obj);
                            break;
                    }
                }
                foreach (var item in message)
                {
                    Perebor(item);
                }
            }

            return true;
        }

    }
}
