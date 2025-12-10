using NLua;
using System.Text;

namespace Terminal_Warrior.Engine
{
    public class GameState
    {
        //
        //  Движок \ Свойства
        //
        public Lua _G { get; set; } = new();
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

        public ConVar(LuaTable args)
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
                case long:
                    int value = Convert.ToInt32(args[2]);
                    if (_limits != (0, 0))
                    {
                        (var min, var max) = _limits;
                        if (max != 0)
                        {
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

                default:
                    _value = args[2];
                    break;
            }
        }

        public object GetConVar()
        {
            return _value!;
        }
        public int GetInt()
        {
            return Convert.ToInt32(_value);
        }
        public char GetChar()
        {
            return Convert.ToChar(_value);
        }
    }
}
