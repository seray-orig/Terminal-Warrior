/*
 * LUA SCENE MANAGEMENT SYSTEM
 * 
 * Менеджер сцен на основе Lua скриптов.
 * Имеет публичные методы для работы со списком сцен и вызова функций
 * 
 * Сцены зашитие в игру помечены "Internal", их код загружается из embedded ресурсов.
 * Кастомные из папки game/scenes "File", выполняются как файл.
 */

using NLua;
using System.Reflection;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.game.scenes
{
    public class LuaSceneManager
    {
        private GameState _state;
        private ILogger _logger;
        public LuaSceneManager(GameState state, ILogger logger)
        {
            _state = state;
            _logger = logger;
        }

        private StringBuilder _currentScene = new("MainMenuTest");
        public string CurrentScene { get { return _currentScene.ToString(); } }
        private StringBuilder _previousScene = new();
        public string PreviousScene {get { return _previousScene.ToString(); } }
        private Dictionary<string, (string, string)> _scenes = new()
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
        public Dictionary<string, (string, string)> Scenes { get { return _scenes; } }
        public void SetScene(string scene)
        {
            _previousScene.Clear(); _previousScene.Append(_currentScene);
            _currentScene.Clear(); _currentScene.Append(scene);
        }
        public void AddScene(string Name)
        {
            if (!_scenes.ContainsKey(Name)) _scenes.Add(Name, ("File", $"game/scenes/{Name}.lua"));
        }
        public void RemoveScene(string Name)
        {
            _scenes.Remove(Name);
        }

        public void CallFunc(string functionName, params object[] args)
        {
            if (!_scenes.TryGetValue(CurrentScene, out var sceneData))
            {
                _logger.Log($"{CurrentScene} Сцена Lua не найдена.");
                return;
            }
            (string Mode, string luaCode) = sceneData;

            if (Mode == "Internal")
            {
                try { _state._G.DoString(luaCode); }
                catch (Exception ex) { _logger.Log($"{CurrentScene} {ex.Message}"); }
            }
            else if (Mode == "File")
            {
                try { _state._G.DoFile(luaCode); }
                catch (Exception ex) { _logger.Log($"{CurrentScene} {ex.Message}"); }
            }
            else { _logger.Log($"{CurrentScene} Неверный режим сцены - {Mode}"); return; }
            
            try { _state._G.GetFunction(functionName)?.Call(args); }
            catch (Exception ex) { _logger.Log($"При вызове {functionName}(): {ex.Message}"); }

            _state._G[functionName] = null;
        }
    }
}
