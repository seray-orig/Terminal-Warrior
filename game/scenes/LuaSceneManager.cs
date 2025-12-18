/*
 * LUA SCENE MANAGEMENT SYSTEM
 * 
 * Менеджер сцен на основе Lua скриптов.
 * Имеет публичные методы для работы со списком сцен и вызова функций
 * 
 * Сцены зашитие в игру помечены "Internal", их код загружается из embedded ресурсов.
 * Кастомные из папки game/scenes "File", выполняются как файл.
 */

using System.Text;
using Terminal_Warrior.Engine;

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

        private StringBuilder _currentScene = new("MainMenu");
        public string CurrentScene { get { return _currentScene.ToString(); } }
        private StringBuilder _previousScene = new();
        public string PreviousScene { get { return _previousScene.ToString(); } }
        private readonly Dictionary<string, string> _scenes = new();
        public void SetScene(string scene)
        {
            _previousScene.Clear(); _previousScene.Append(_currentScene);
            _currentScene.Clear(); _currentScene.Append(scene);
        }
        public void AddScene(string Name)
        {
            if (!_scenes.ContainsKey(Name)) _scenes.Add(Name, $"game/scenes/{Name}.lua");
        }
        public void RemoveScene(string Name)
        {
            _scenes.Remove(Name);
        }

        public void CallFunc(string functionName, params object[] args)
        {
            if (!_scenes.TryGetValue(CurrentScene, out var scenePath))
            {
                _logger.Log($"{CurrentScene}.lua Сцена Lua не найдена.");
                return;
            }

            try { _state._G.DoFile(scenePath); }
            catch (Exception ex) { _logger.Log($"Сцена {CurrentScene}.lua При выполнении скрипта: {ex.Message}"); }

            try { _state._G.GetFunction(functionName)?.Call(args); }
            catch (Exception ex) { _logger.Log($"Сцена {CurrentScene}.lua При вызове {functionName}(): {ex.Message}"); }

            _state._G[functionName] = null;
        }
    }
}
