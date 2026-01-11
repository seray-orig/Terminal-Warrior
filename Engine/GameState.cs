/*
 * GAME STATE MANAGEMENT
 * 
 * Анемичная модель.
 * Хранит глобальное состояние игры.
 * 
 * Lua-окружение
 * Хранилище консольных переменных ConVars
 * Статус работы игры и параметры экрана
 */

using NLua;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Engine
{
    public class GameState
    {
        public Lua _G = new();
        public ConVar ConVar = new();
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
}
