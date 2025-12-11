/*
 * GAME STATE MANAGEMENT
 * 
 * Анемичная модель.
 * Хранит и управляет глобальным состоянием игры.
 * Включает:
 * - Lua-окружение
 * - Хранилище консольных переменных ConVars
 * - Статус работы игры и параметры экрана
 * 
 * Отделяет управление состоянием от игровой логики,
 * обеспечивая чистую архитектуру.
 */

using NLua;
using System.Text;

namespace Terminal_Warrior.Engine
{
    public class GameState
    {
        public Lua _G = new();
        public Dictionary<string, ConVar> ConVarList = new();
        public bool IsRunning { get; private set; } = false;
        public void StartGame() { IsRunning = true; }
        public void ShutDownGame() { IsRunning = false; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int ScreenSymbolsMax { get; private set; }
        public void UpdateScreenSize(int Width, int Height)
        {
            ScreenWidth = Width; ScreenHeight = Height;
            ScreenSymbolsMax = Width * Height;
        }
    }

    //
    //  "Консольные переменные"
    //
    public class ConVar
    {
        private object? _value;
        private (int, int) _limits;

        public ConVar(LuaTable args)    // [1] - Имя, [2] - Значение, [3 - 4] - Пределы включительно (если значение - число)
        {
            if (args[4] != null)
                _limits = (Convert.ToInt32(args[3]), Convert.ToInt32(args[4]));
            else if (args[3] != null)
                _limits = (Convert.ToInt32(args[3]), 0);
            else
                _limits = (0, 0);

            SetConVar(args);
        }

        public void SetConVar(LuaTable args)
        {
            switch (args[2])
            {
                // В Lua целое число - это тип Int64
                case long:
                    int value = Convert.ToInt32(args[2]);
                    if (_limits != (0, 0))
                    {
                        (var min, var max) = _limits;
                        if (max != 0)
                        {
                            // Если число меньше минимума, очевидно нужно присвоить минимум
                            // но, если есть лимит на максимум, то пусть будет максимум
                            // мне кажется в этом есть смысл.
                            if (value < min || value > max)
                            { if (_value == null) _value = max; return; }
                        }
                        else
                        {
                            if (value < min)
                            { if (_value == null) _value = min; return; }
                        }
                    }
                    _value = value;
                    break;
                case string:
                    string text = Convert.ToString(args[2])!;
                    if (text.Length == 1)
                        _value = Convert.ToChar(text);
                    else 
                        _value = text;
                        break;

                default:
                    _value = args[2];
                    break;
            }
        }

        public dynamic GetConVar()
        {
            return _value!;
        }
    }
}
