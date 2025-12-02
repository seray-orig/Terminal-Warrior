using System;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Game.lua
{
    /// <summary>
    /// Здесь создаются C# API функции для окружения Lua.
    /// </summary>
    public class LuaFunctions : CoreContext
    {
        public LuaFunctions(GameContext gameContext) : base(gameContext)
        {
            foreach (var (luaFunc, csFunc) in Functions())
                _state._G[luaFunc] = csFunc;

            // Write & Writeln вызывали утечку памяти по неизвестной причине
            // решено реализовать их так - средствами Lua
            _state._G.DoString("""

                Writeln = print
                Write = io.write

             """);
        }
        private Dictionary<string, object> Functions()
        {
            return new Dictionary<string, object>()
            {
                {
                    "ScrW", (Func<int>)(() => { return _state.ScreenWidth; })
                },
                {
                    "ScrH", (Func<int>)(() => { return _state.ScreenHeight; })
                },
                {
                    "SetDebugChar", (Action<string>)(
                    (symbol) =>
                    {
                        _state.SetDebugChar(Convert.ToChar(symbol));
                    })
                },
                {
                    "SetCursorPos", (Action<int, int>)((left, top) => { try { Console.SetCursorPosition(left, top); } catch { } })
                },
                { // Cursor Left
                    "CurL", (Func<int>)(() => { return Console.GetCursorPosition().Left; })
                },
                { // Cursor Top
                    "CurT", (Func<int>)(() => { return Console.GetCursorPosition().Top; })
                },
                {
                    "SetScene", (Action<string>)((sceneName) => { _sceneManager.SetScene(sceneName); })
                },
                {
                    "_ShutDownGame", (Action)(() =>  { _state.ShutDownGame(); })
                },
            };
        }
    }
}
