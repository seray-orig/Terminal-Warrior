using NLua;
using System.Text;
using Terminal_Warrior.Engine;

namespace Terminal_Warrior.Game.scenes
{
    public class LuaSceneManager : IDisposable
    {
        private Lua _lua;
        private readonly StringBuilder _lastException = new StringBuilder();

        public LuaSceneManager(Lua _G)
        {
            _lua = _G;
        }

        public void AddLuaFunctions(Dictionary<string, object> actions)
        {
            foreach (var (key, value) in actions)
                _lua[key] = value;
        }

        public void RunLua(Dictionary<string, (string, string)> scenes, string sceneName, string functionName)
        {
            if (!scenes.TryGetValue(sceneName, out var sceneData))
            {
                LuaError(sceneName, functionName, new Exception("Сцена Lua не найдена."));
                return;
            }
            (string Mode, string luaCode) = sceneData;

            if (Mode == "Internal")
            {
                try { _lua.DoString(luaCode); }
                catch (Exception ex) { LuaError(sceneName, functionName, ex); }
            }
            else
            {
                try { _lua.DoFile(luaCode); }
                catch (Exception ex) { LuaError(sceneName, functionName, ex); return; }
            }

            var func = _lua.GetFunction(functionName);
            if (func == null)
            {
                LuaError(sceneName, functionName, new Exception("Функция Lua не найдена"));
                return;
            }

            try { func?.Call(); }
            catch (Exception ex)
            {
                LuaError(sceneName, functionName, ex);
            }

            _lua[functionName] = null;
        }
        private void LuaError(string Name, string Function, Exception exception)
        {
            try { Console.SetCursorPosition(1, 1); }
            finally {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("/!\\");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" Что-то создаёт скриптовые ошибки.");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write(exception.Message);
            }

            if (_lastException.ToString() == exception.ToString()) return;
            string text = $"LuaSceneManager {Name}.lua {Function}(): {exception.Message}";
            GameState.ErrorLogger.Log(text);

            _lastException.Clear();
            _lastException.Append(exception.ToString());
        }

        public void Dispose()
        {
            _lua.Dispose();
        }
    }
}
