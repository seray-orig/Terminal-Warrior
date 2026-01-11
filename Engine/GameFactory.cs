using NLua;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Engine.Implementations;
using Terminal_Warrior.game.lua;
using Terminal_Warrior.game.scenes;
using Terminal_Warrior.Logger;

namespace Terminal_Warrior.Engine
{
    public static class GameFactory
    {
        public static IGame Create()
        {
            GameState state = new();
            ILogger logger = new MainLogger();

            GameContext gameContext = new(state, logger);

            InputHandler inputHandler = new InputLuaHandler(gameContext);
            EngineUpdater engineUpdater = new EngineLuaUpdater(gameContext);
            FrameRenderer frameRenderer = new FrameLuaRenderer(gameContext);

            return new Game(
                gameContext,
                inputHandler,
                engineUpdater,
                frameRenderer
            );
        }
    }

    public sealed class GameContext
    {
        public GameState _state;
        public ILogger _logger;

        public LuaSceneManager _sceneManager;
        public InitializeLua _luaInit;
        public LuaScriptClinger _luaScriptClinger;

        public GameContext(
            GameState state,
            ILogger logger
        )
        {
            _state = state;
            _logger = logger;

            _sceneManager = new(_state, _logger);
            _luaInit = new(_state, _logger, _sceneManager);
            _luaScriptClinger = new(_state, _logger, _sceneManager);
        }

        /// <summary>
        /// Пересоздаёт окружение Lua со всеми стартовыми параметрами.
        /// Полезно, если Lua померло, например от "C stack overflow".
        /// Не влияет на C# ядро игры, сохраняется текущая сцена.
        /// Безопасно вызывать в любой точке кода (наверное).
        /// </summary>
        public void HotLuaReload()
        {
            _state._G.Dispose();
            _state._G = new Lua();
            _state.ConVar.Dispose();

            _luaInit = new(_state, _logger, _sceneManager);

            _luaScriptClinger.Dispose();
            _luaScriptClinger = new(_state, _logger, _sceneManager);
        }
    }
}
