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
                    "CreateConVar", (Func<LuaTable, List<object>>)((args) => {
                        if (!_state.ConVarList.ContainsKey((string)args[1]))
                        {
                            var convar = new ConVar(args);
                            if (convar.GetConVar() == null)
                                return new List<object>(){ false, "значение переменной равно null" };
                            _state.ConVarList.Add((string)args[1], convar);
                            return new List<object>(){ true, "переменная создана" };
                        }
                        return new List<object>(){ false, "переменная с таким именем уже существует" };
                    })
                },
                {
                    "SetConVar", (Action<LuaTable>)((args) => { _convar[(string)args[1]].SetConVar(args); })
                },
                {
                    "GetConVar", (Func<string, object>)((name) => { return _convar[name].GetConVar(); })
                },
                {
                    "Log", (Func<LuaTable, bool>)((message) => { return _logger.Log(message); })
                },
                {
                    "SpawnEntity", (Func<string, string, LuaTable, Entity>)((Name, Texture, SpawnPoint) =>
                    { return new Entity(Name, Texture, (Convert.ToUInt32(SpawnPoint[1]), Convert.ToUInt32(SpawnPoint[2])) ); })
                },
                {
                    "GetEntityTable", (Action<LuaTable>)((table) =>
                    {
                        foreach (var kv in Entity.EntityDictionary)
                            table[kv.Key] = kv.Value;
                    })
                },
                {
                    "SpawnNpc", (Func<string, int, LuaTable, string, Npc>)((Name, HP, SpawnPoint, Texture) =>
                    { return new Npc(Name, HP, (Convert.ToUInt32(SpawnPoint[1]), Convert.ToUInt32(SpawnPoint[2])), Texture); })
                },
                {
                    "GetNpcTable", (Action<LuaTable>)((table) =>
                    {
                        foreach (var kv in Npc.NpcDictionary)
                            table[kv.Key] = kv.Value;
                    })
                },
                {
                    "SpawnPlayer", (Func<string, int, LuaTable, string, Player>)((Name, HP, SpawnPoint, Texture) =>
                    { return new Player(Name, HP, (Convert.ToUInt32(SpawnPoint[1]), Convert.ToUInt32(SpawnPoint[2])), Texture); })
                },
                {
                    "GetCurrentPlayer", (Func<Player?>)(() => { return Player.CurrentPlayer; } )
                },
                {
                    "include", (Action<string>)((path) =>
                    {
                        if (_includes.Contains(path))
                            return;

                        path = path.Replace("..", "");
                        try
                        {
                            _state._G.DoFile($"game/lua/{path}");
                            _includes.Add(path);
                        }
                        catch(Exception ex)
                        {
                            _logger.Log($"Не удалось выполнить скрипт game/lua/{path}: {ex}");
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

                Writeln = print
                Write = io.write

                local function ReturnTable(...)
                    local LuaTable = {}
                    for _, v in ipairs({...}) do
                        table.insert(LuaTable, v)
                    end
                    return LuaTable
                end

                local oldLod = Log
                Log = function(...)
                    oldLod(ReturnTable(...))
                end

                local oldCreateConVar = CreateConVar
                CreateConVar = function(...)
                    local list = oldCreateConVar(ReturnTable(...))
                    if (list[0] == false) then  -- list[0] - удалось создать или нет (true / false)
                        Log(string.format("Не удалось создать convar: %s %s", ..., list[1]))    -- list[1] сообщение
                    end
                end

                local oldSetConVar = SetConVar
                SetConVar = function(...)
                    oldSetConVar(ReturnTable(...))
                end

             """);
        }
    }
}
