using NLua;
using System.Text;
using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.game.lua
{
    /// <summary>
    /// Здесь создаются C# API функции для окружения Lua.
    /// </summary>
    public class InitializeLua
    {
        private GameState _state;
        private Dictionary<string, ConVar> _convar;
        private ILogger _logger;
        private LuaSceneManager _sceneManager;
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
                /*{
                    "DoScript", (Action<string>)((fileName) => { try { _state._G.DoFile($"Game/lua/{fileName}"); }
                        catch { _logger.Log($"Не удалось выполнить скрипт: {fileName}"); } })
                },*/
            };
            foreach (var (luaFunc, csFunc) in CStoLua)
                _state._G[luaFunc] = csFunc;

            //
            // Создание функций методами самой Lua или костыляция API функций выше
            //
            _state._G.DoString("""

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
