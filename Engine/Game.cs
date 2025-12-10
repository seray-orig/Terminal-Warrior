using System.Text;
using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Engine
{
    public class Game : CoreContext, IGame
    {
        private InputHandler _inputHandler;
        private EngineUpdater _engineUpdater;
        private FrameRenderer _frameRenderer;

        public Game(
            GameContext gameContext,
            InputHandler inputHandler,
            EngineUpdater engineUpdater,
            FrameRenderer frameRenderer
            ) : base(gameContext)
        {
            _inputHandler = inputHandler;
            _engineUpdater = engineUpdater;
            _frameRenderer = frameRenderer;
        }
        public void Run()
        {
            Initialize();
            GameLoop();
        }

        private void Initialize()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Title = "Terminal Warrior";
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            _state.StartGame();
        }

        private void GameLoop()
        {
            while (_state.IsRunning)
            {
                var frameStart = DateTime.Now;

                // Костыль на "фиксированное" значение видимости курсора
                Console.CursorVisible = false;

                _inputHandler.Handle();
                _engineUpdater.Update();
                _frameRenderer.Render();

                // Подгон под фпс
                int elapsed = (int)(DateTime.Now - frameStart).TotalMilliseconds;
                int delay = Math.Max(0, (1000 / _convar["fps_target"].GetInt()) - elapsed);
                Thread.Sleep(delay);
            }
        }
    }
}
