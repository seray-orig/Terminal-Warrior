/*
 * LUA SCRIPT TRACKING SYSTEM
 * 
 * Система динамической загрузки и отслеживания Lua-скриптов.
 * Обеспечивает:
 * - Автоматическое отслеживание изменений скриптов через FileSystemWatcher
 * - Создание необходимых директорий при инициализации
 * 
 * Реализован метод Dispose для освобождения неуправляемых ресурсов.
 * Используется при горячей перезагрузки Lua окружения.
 */

using System.Reflection;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.game.lua
{
    public class LuaScriptClinger : IDisposable
    {
        private GameState _state;
        private Dictionary<string, ConVar> _convar;
        private ILogger _logger;
        private LuaSceneManager _sceneManager;

        private FileSystemWatcher _configWatcher;
        private FileSystemWatcher _scenesWatcher;

        private List<string> _directories = new()
            { "game/scenes", "game/cfg", "game/lua", "logs", };
        private string _configPath { get; } = "game/cfg/config.lua";

        public LuaScriptClinger(GameState state, ILogger logger, LuaSceneManager sceneManager)
        {
            _state = state;
            _convar = _state.ConVarList;
            _logger = logger;
            _sceneManager = sceneManager;

            //
            // Директории которые необходимы для игры
            //
            foreach (string Name in _directories)
            {
                if (!Directory.Exists(Name))
                    Directory.CreateDirectory(Name);
            }

            //
            // Файл конфигурации
            //
            void DoConfig()
            {
                try
                {
                    _state._G.DoFile(_configPath);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message, " Используется стандартный файл конфигурации.");
                    _state._G.DoString(new StreamReader(Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Terminal_Warrior.game.cfg.config.lua")!, Encoding.UTF8).ReadToEnd());
                }
            }

            _configWatcher = new("game/cfg", "config.lua");
            _configWatcher.EnableRaisingEvents = true;
            _configWatcher.Created += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Changed += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Deleted += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Renamed += new RenamedEventHandler((sender, e) => DoConfig());

            if (!File.Exists(_configPath))
            {
                File.Create(_configPath).Close();
                File.WriteAllText(_configPath, new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Terminal_Warrior.game.cfg.config.lua")!, Encoding.UTF8).ReadToEnd());
            }

            DoConfig();

            //
            // Сцены
            //
            _scenesWatcher = new("game/scenes", "*.lua");
            _scenesWatcher.EnableRaisingEvents = true;
            _scenesWatcher.Created += new FileSystemEventHandler((sender, e) =>
            {
                var Name = Path.GetFileNameWithoutExtension(e.FullPath);
                if (!string.IsNullOrEmpty(Name))
                    _sceneManager.AddScene(Name);
            });
            _scenesWatcher.Changed += new FileSystemEventHandler((sender, e) =>
            {
                var Name = Path.GetFileNameWithoutExtension(e.FullPath);
                if (!string.IsNullOrEmpty(Name))
                {
                    _sceneManager.RemoveScene(Name);
                    _sceneManager.AddScene(Name);
                }
            });
            _scenesWatcher.Deleted += new FileSystemEventHandler((sender, e) =>
            {
                var Name = Path.GetFileNameWithoutExtension(e.FullPath);
                if (!string.IsNullOrEmpty(Name))
                    _sceneManager.RemoveScene(Name);
            });
            _scenesWatcher.Renamed += new RenamedEventHandler((sender, e) =>
            {
                var Name = Path.GetFileNameWithoutExtension(e.FullPath);
                if (!string.IsNullOrEmpty(Name))
                {
                    if (Path.GetExtension(e.FullPath) == ".lua")
                    {
                        _sceneManager.RemoveScene(Name);
                        _sceneManager.AddScene(Name);
                    }
                    else
                        _sceneManager.RemoveScene(Name);
                }
            });

            foreach (string filePath in Directory.GetFiles("game/scenes", "*.lua"))
                _sceneManager.AddScene(Path.GetFileNameWithoutExtension(filePath));
        }

        public void Dispose()
        {
            _configWatcher.Dispose();
            _scenesWatcher.Dispose();
        }
    }
}
