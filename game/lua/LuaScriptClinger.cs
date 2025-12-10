using System.Reflection;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.game.lua
{
    public class LuaScriptClinger
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
            void DoScene()
            {

            }

            _scenesWatcher = new("game/scenes", "*.lua");
            _configWatcher.EnableRaisingEvents = true;
            _configWatcher.Created += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Changed += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Deleted += new FileSystemEventHandler((sender, e) => DoConfig());
            _configWatcher.Renamed += new RenamedEventHandler((sender, e) => DoConfig());
        }

        public void FileSystemWatchersDispose()
        {
            _configWatcher.Dispose();
        }
    }
}
