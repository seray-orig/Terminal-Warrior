/*
 * CORE CONTEXT BASE CLASS
 * 
 * Базовый класс для основных компонентов движка.
 * Предоставляет доступ к общим зависимостям:
 * - GameState (состояние игры)
 * - ConVar словарь (консольные переменные)
 * - LuaContext (Lua окружение)
 * - Logger (система логгирования)
 * - SceneManager (менеджер сцен)
 * 
 * Все наследники должны получать GameContext и
 * передавать его в base() через конструктор.
 */

using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.Engine.Core
{
    public abstract class CoreContext
    {
        protected readonly GameState _state;
        protected readonly ConVar _convar;
        protected readonly ILogger _logger;
        protected readonly LuaSceneManager _sceneManager;
        private readonly GameContext _gameContext;

        protected CoreContext(GameContext gameContext)
        {
            _state = gameContext._state;
            _convar = _state.ConVar;
            _logger = gameContext._logger;
            _sceneManager = gameContext._sceneManager;
            _gameContext = gameContext;
        }

        protected void HotLuaReload()
        {
            _gameContext.HotLuaReload();
        }
    }
}
