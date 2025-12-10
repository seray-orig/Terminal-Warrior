using NLua;
using System.Reflection;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.game.scenes
{
    public class LuaSceneManager
    {
        private Lua _lua;
        private GameState _state;
        private ILogger _logger;
        public LuaSceneManager(GameState state, ILogger logger)
        {
            _lua = state._G;
            _state = state;
            _logger = logger;
        }

        private StringBuilder _currentScene = new StringBuilder("MainMenuTest");
        public string CurrentScene { get { return _currentScene.ToString(); } }
        public void SetScene(string scene) { _currentScene.Clear(); _currentScene.Append(scene); }
        public void AddScene(string Name, string LuaCode)
        {
            if (!Scenes.ContainsKey(Name)) Scenes.Add(Name, ("File", LuaCode));
        }
        public void RemoveScene(string Name)
        {
            Scenes.Remove(Name);
        }

        public void CallFunc(string functionName, params object[] args)
        {
            if (!Scenes.TryGetValue(CurrentScene, out var sceneData))
            {
                _logger.Log($"{CurrentScene} Сцена Lua не найдена.");
                return;
            }
            (string Mode, string luaCode) = sceneData;

            if (Mode == "Internal")
            {
                try { _lua.DoString(luaCode); }
                catch (Exception ex) { _logger.Log($"{CurrentScene} {ex.Message}"); }
            }
            else if (Mode == "File")
            {
                try { _lua.DoFile(luaCode); }
                catch (Exception ex) { _logger.Log($"{CurrentScene} {ex.Message}"); }
            }
            else { _logger.Log($"{CurrentScene} Неверный режим сцены - {Mode}"); return; }
            
            try { _lua.GetFunction(functionName)?.Call(args); }
            catch (Exception ex) { _logger.Log($"При вызове {functionName}(): {ex.Message}"); }

            _lua[functionName] = null;
        }

        //
        //  Сцены
        //
        public Dictionary<string, (string, string)> Scenes { get; set; } = new Dictionary<string, (string, string)>()
        {
            {   // Эти сцены зашиты в игру
                "MainMenu", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.game.scenes.MainMenu.lua")!, Encoding.UTF8).ReadToEnd())
            },
            {
                "Gameplay", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.game.scenes.Gameplay.lua")!, Encoding.UTF8).ReadToEnd())
            },
            {
                "cmd", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.game.scenes.cmd.lua")!, Encoding.UTF8).ReadToEnd())
            },
        };
    }
}
