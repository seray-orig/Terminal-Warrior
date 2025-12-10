using System.Reflection;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.game.lua
{
    public class LuaScriptClinger : CoreContext
    {
        private List<string> _directories = new()
            { "game/scenes", "game/cfg", "game/lua", "logs", };
        private string _configPath { get; } = "game/cfg/config.lua";

        public LuaScriptClinger(GameContext gameContext) : base(gameContext)
        {
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

            FileSystemWatcher config = new("game/cfg", "config.lua");
            config.EnableRaisingEvents = true;
            config.Created += new FileSystemEventHandler((sender, e) => DoConfig());
            config.Changed += new FileSystemEventHandler((sender, e) => DoConfig());
            config.Deleted += new FileSystemEventHandler((sender, e) => DoConfig());
            config.Renamed += new RenamedEventHandler((sender, e) => DoConfig());

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

        }
    }
}
