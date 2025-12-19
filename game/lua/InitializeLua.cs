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
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.game.lua
{
    public class InitializeLua
    {
        private GameState _state;
        private Dictionary<string, ConVar> _convar;
        private ILogger _logger;
        private LuaSceneManager _sceneManager;
        private List<string> _includes = new(); // Хранит список скриптов подключенных к среде
        public InitializeLua(GameState state, ILogger logger, LuaSceneManager sceneManager)
        {
            _state = state;
            _convar = _state.ConVarList;
            _logger = logger;
            _sceneManager = sceneManager;

            _state._G.State.Encoding = Encoding.UTF8;

            //
            //  Создание API функций - мост между игрой и Lua скриптами
            //
            Dictionary<string, object> CStoLua = new Dictionary<string, object>()
            {
                {
                    "WriteLayer", (Action<LuaTable>)((args) =>
                    {
                        int layer = Convert.ToInt32(args[1]);
                        args[1] = null;
                        try
                        {
                            ConsoleExtended.AddLayer(new Action(() =>
                            {
                                try
                                {
                                    Console.SetCursorPosition(Convert.ToInt32(args[2]), Convert.ToInt32(args[3]));
                                    args[2] = null; args[3] = null;
                                } catch {}
                                foreach (dynamic item in args)
                                    Console.Write(item.Value);
                            }), layer);
                        }
                        catch(Exception ex) { _logger.Log($"Сцена {_sceneManager.CurrentScene} Не указан слой или позиция для WriteLayer(): {ex.Message}"); }
                    })
                },
                {
                    "ScrW", (Func<int>)(() => { return _state.ScreenWidth; })
                },
                {
                    "ScrH", (Func<int>)(() => { return _state.ScreenHeight; })
                },
                {
                    "SetCursorPos", (Action<int, int>)((left, top) => { try { Console.SetCursorPosition(left, top); } catch { } })
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
                        var convar = new ConVar(args);
                        if (convar.GetConVar() == null)
                            _logger.Log($"Сцена {_sceneManager.CurrentScene} CreateConVar({(string)args[1]}) значение равно null");

                        // Создаём, если нет. Меняем значение, если есть.
                        if (!_state.ConVarList.ContainsKey((string)args[1]))
                            _state.ConVarList.Add((string)args[1], convar);
                        else
                            _convar[(string)args[1]].SetConVar(args);
                    })
                },
                {
                    "SetConVar", (Action<LuaTable>)((args) => {
                        if (_convar.TryGetValue((string)args[1], out var convar))
                        {
                            if (args[2] == null)
                                _logger.Log($"Сцена {_sceneManager.CurrentScene} SetConVar({(string)args[1]}) значение равно null.");
                            else
                                convar.SetConVar(args);
                        }
                        else
                            _logger.Log($"Сцена {_sceneManager.CurrentScene} ConVar с именем {(string)args[1]} не существует.");
                    })
                },
                {
                    "GetConVar", (Func<string, object>)((name) => {
                        if (_convar.TryGetValue(name, out var convar))
                            return convar.GetConVar();
                        return null!;
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
                    "GetEntityTable", (Action<LuaTable>)((table) =>
                    {
                        foreach (var kv in Entity.EntityDictionary)
                            table[kv.Key] = kv.Value;
                    })
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
