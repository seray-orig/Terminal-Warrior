

using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine.Core
{
    public abstract class CoreContext
    {
        protected readonly GameState _state;
        protected readonly ILogger _logger;
        protected readonly LuaSceneManager _sceneManager;

        protected CoreContext(GameContext gameContext)
        {
            _state = gameContext._state;
            _logger = gameContext._logger;
            _sceneManager = gameContext._sceneManager;
        }
    }
}
