using Terminal_Warrior.Engine.Core;

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
                foreach (var item in message) Console.Write(item);
            }

            return true;
        }
    }
}
