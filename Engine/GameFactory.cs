using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Engine.Implementations;
using Terminal_Warrior.Game.lua;
using Terminal_Warrior.Game.scenes;
using Terminal_Warrior.Logger;

namespace Terminal_Warrior.Engine
{
    public class GameFactory : IFactory<IGame>
    {
        public IGame Create()
        {
            GameState state = new GameState();
            IFactory<ILogger> loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.Create();
            LuaSceneManager sceneManager = new LuaSceneManager(state, logger);

            var gameContext = new GameContext(state, logger, sceneManager);
            LuaFunctions luaFunctions = new LuaFunctions(gameContext);
            InputHandler inputHandler = new LuaInputHandler(gameContext);
            EngineUpdater engineUpdater = new LuaEngineUpdater(gameContext);
            FrameRenderer frameRenderer = new LuaFrameRenderer(gameContext);

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
        public LuaSceneManager _sceneManager;
        public GameContext(
            GameState state,
            ILogger logger,
            LuaSceneManager sceneManager
        )
        {
            _state = state;
            _logger = logger;
            _sceneManager = sceneManager;
        }
    }
}
