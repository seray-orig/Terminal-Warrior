

using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.Engine.Core
{
    public abstract class CoreContext
    {
        protected readonly GameState _state;
        protected readonly Dictionary<string, ConVar> _convar;
        protected readonly LuaContext _luaContext;
        protected readonly ILogger _logger;
        protected readonly LuaSceneManager _sceneManager;

        protected CoreContext(GameContext gameContext)
        {
            _state = gameContext._state;
            _convar = _state.ConVarList;
            _luaContext = gameContext._luaContext;
            _logger = gameContext._logger;
            _sceneManager = gameContext._luaContext._sceneManager;
        }
    }
}
