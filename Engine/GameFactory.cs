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
            LuaSceneManager sceneManager = new(state, logger);

            var luaContext = new LuaContext();
            var gameContext = new GameContext(state, logger, luaContext);
            InitializeLua luaInit = new(gameContext);
            LuaScriptClinger scriptClinger = new(gameContext);
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
        public InitializeLua _luaInit;
        public LuaSceneManager _sceneManager;
        public LuaScriptClinger _luaScriptClinger;

        public LuaContext(
            InitializeLua luaInit,
            LuaSceneManager sceneManager,
            LuaScriptClinger luaScriptClinger
        )
        {
            _luaInit = luaInit;
            _sceneManager = sceneManager;
            _luaScriptClinger = luaScriptClinger;
        }
    }
}
