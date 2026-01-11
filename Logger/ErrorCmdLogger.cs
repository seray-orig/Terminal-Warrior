using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Logger.Util;

namespace Terminal_Warrior.Logger
{
    public sealed class ErrorCmdLogger : ILogger
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

            return StaticLog(message);
        }

        public static bool StaticLog(params object[] message)
        {
            // Как же тяжело работать с консолью ;(
            // Она не предназначена для игр, поэтому приходится костылять
            // иначе сообщения вызванные до рендера сотрутся.
            // Пусть слоем для ошибок будет 55
            ConsoleExtended.AddLayer(new Action(() =>
            {
                try { Console.SetCursorPosition(1, 1); } catch { }
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("/!\\");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" Что-то создаёт скриптовые ошибки.", 55);
                Console.ResetColor();
                Console.WriteLine();
            }), 55);
            foreach (var item in message)
            {
                ConsoleExtended.AddLayer(new Action(() =>
                    Console.Write(Perebor.Do(item)
                )), 55);
            }

            return true;
        }
    }
}
