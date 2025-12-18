using NLua;
using Terminal_Warrior.Engine.Core;

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
            void Perebor(object obj)
            {
                switch (obj)
                {
                    case LuaTable:
                        LuaTable content = (LuaTable)obj;
                        foreach (var item in content.Values)
                            Perebor(item);
                        break;
                    case string:
                        ConsoleExtended.AddLayer(new Action(() => Console.Write(obj)), 55);
                        break;
                    case System.Collections.IEnumerable:
                        dynamic content2 = obj;
                        foreach (var item in content2)
                            Perebor(item);
                        break;
                    default:
                        ConsoleExtended.AddLayer(new Action(() => Console.Write(obj)), 55);
                        break;
                }
            }
            foreach (var item in message)
            {
                Perebor(item);
            }

            return true;
        }
    }
}
