/*
 * LUA API BRIDGE INITIALIZATION
 * 
 * Инициализирует мост между C# и Lua окружением.
 * Расширяет стандартное Lua поведение через переопределение и
 * создание API функций, тем самым образуя возможность моддинга игры.
 */

using NLua;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.game.lua
{
    public class InitializeLua
    {
        private GameState _state;
        private ILogger _logger;
        private LuaSceneManager _sceneManager;
        private List<string> _includes = new(); // Хранит список скриптов подключенных к среде Lua
        public InitializeLua(GameState state, ILogger logger, LuaSceneManager sceneManager)
        {
            _state = state;
            _logger = logger;
            _sceneManager = sceneManager;

            _state._G.State.Encoding = Encoding.UTF8;

            //
            //  Создание API функций - мост между игрой и Lua скриптами
            //
            Dictionary<string, object> CStoLua = new Dictionary<string, object>()
            {
                {
                    "ScrW", (Func<int>)(() => { return _state.ScreenWidth; })
                },
                {
                    "ScrH", (Func<int>)(() => { return _state.ScreenHeight; })
                },
                {
                    "SetBackgroundColor", (Action<string>)((text) =>
                    {
                        if (text == "Reset")
                            Console.ResetColor();
                        else if (Enum.TryParse<ConsoleColor>(text, out ConsoleColor color))
                            Console.BackgroundColor = color;
                    })
                },
                {
                    "SetForegroundColor", (Action<string>)((text) =>
                    {
                        if (text == "Reset")
                            Console.ResetColor();
                        else if (Enum.TryParse<ConsoleColor>(text, out ConsoleColor color))
                            Console.ForegroundColor = color;
                    })
                },
                {
                    "SetCursorPos", (Func<int, int, bool>)((left, top) =>
                    {
                        try
                        {
                            Console.SetCursorPosition(left, top);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                },
                {
                    "CurL", (Func<int>)(() => { return Console.GetCursorPosition().Left; })
                },
                {
                    "CurT", (Func<int>)(() => { return Console.GetCursorPosition().Top; })
                },
                {
                    "SetScene", (Action<string>)((sceneName) => { _sceneManager.SetScene(sceneName); })
                },
                {
                    "ShutDownGame", (Action)(() =>  { _state.ShutDownGame(); })
                },
                {
                    "CreateConVar", (Action<LuaTable>)((args) => {
                        _state.ConVar[(string)args[1]] = args;
                        if (_state.ConVar[(string)args[1]] == null)
                            _logger.Log($"Сцена {_sceneManager.CurrentScene} CreateConVar({(string)args[1]}) Имя или значение равно null");
                    })
                },
                {   // Решил разделить создание и изменение значения консольной переменной явно. Хотя эти две функции взаимозаменяемы
                    "SetConVar", (Action<LuaTable>)((args) => {
                        _state.ConVar[(string)args[1]] = args;
                        if (_state.ConVar[(string)args[1]] == null)
                            _logger.Log($"Сцена {_sceneManager.CurrentScene} SetConVar({(string)args[1]}) Значение или имя равно null");
                    })
                },
                {
                    "GetConVar", (Func<string, object>)((name) => {
                        return _state.ConVar[name];
                    })
                },
                {
                    "Log", (Func<LuaTable, bool>)((message) => { return _logger.Log(message); })
                },
                {
                    "SpawnEntity", (Func<string, LuaTable, Entity>)((Name, SpawnPoint) =>
                    { return new Entity(Name, (Convert.ToUInt32(SpawnPoint[1]), Convert.ToUInt32(SpawnPoint[2])) ); })
                },
                {
                    // Одна из ключевых команд - подключает скрипты внутри скриптов
                    "include", (Action<string>)((path) =>
                    {
                        // Подключает один раз
                        if (_includes.Contains(path))
                            return;

                        // Блокируем доступ к родительским каталогам
                        path = path.Replace("..", "");
                        try
                        {
                            _state._G.DoFile($"game/lua/{path}");
                            _includes.Add(path);
                        }
                        catch(Exception ex)
                        {
                            _logger.Log($"Не удалось выполнить скрипт game/lua/{path}: {ex.Message}");
                        }
                    })
                },
            };
            foreach (var (luaFunc, csFunc) in CStoLua)
                _state._G[luaFunc] = csFunc;

            //
            // Создание функций методами самой Lua или костыляция API функций выше
            //
            _state._G.DoString("""

                os.execute = nil
                io.popen = nil
                debug = nil
                loadfile = nil
                require = nil
                dofile = nil

                Write = io.write
                Writeln = print

                local function ReturnTable(...)
                    local LuaTable = {}
                    for _, v in ipairs({...}) do
                        table.insert(LuaTable, v)
                    end
                    return LuaTable
                end

                local oldWriteLayer = WriteLayer
                function WriteLayer(...)
                    oldWriteLayer(ReturnTable(...))
                end

                local oldLod = Log
                Log = function(...)
                    oldLod(ReturnTable(...))
                end

                local oldCreateConVar = CreateConVar
                CreateConVar = function(...)
                    oldCreateConVar(ReturnTable(...))
                end

                local oldSetConVar = SetConVar
                SetConVar = function(...)
                    oldSetConVar(ReturnTable(...))
                end

             """);
        }
    }
}
