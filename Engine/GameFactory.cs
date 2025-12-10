using NLua;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Engine.Implementations;
using Terminal_Warrior.game.lua;
using Terminal_Warrior.game.scenes;
using Terminal_Warrior.Logger;

namespace Terminal_Warrior.Engine
{
    public class GameFactory : IFactory<IGame>
    {
        public IGame Create()
        {
            GameState state = new();
            IFactory<ILogger> loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.Create();

            var luaContext = new LuaContext(state, logger);
            var gameContext = new GameContext(state, logger, luaContext);

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

    public class GameContext
    {
        public GameState _state;
        public ILogger _logger;
        public LuaContext _luaContext;
        public GameContext(
            GameState state,
            ILogger logger,
            LuaContext luaContext
        )
        {
            _state = state;
            _logger = logger;
            _luaContext = luaContext;
        }
    }

    public class LuaContext
    {
        private readonly GameState _state;
        private readonly ILogger _logger;

        public LuaSceneManager _sceneManager;
        public InitializeLua _luaInit;
        public LuaScriptClinger _luaScriptClinger;

        public LuaContext(
            GameState state,
            ILogger logger
        )
        {
            _state = state;
            _logger = logger;

            _sceneManager = new(state, logger);
            _luaInit = new(state, logger, _sceneManager);
            _luaScriptClinger = new(state, logger, _sceneManager);
        }

        public void ReloadLua()
        {
            _state._G.Dispose();
            _state._G = new Lua();
            _state.ConVarList.Clear();

            _luaInit = new(_state, _logger, _sceneManager);

            _luaScriptClinger.FileSystemWatchersDispose();
            _luaScriptClinger = new(_state, _logger, _sceneManager);
        }
    }
}
