using System.Text;
using NLua;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine
{
    public class Game
    {
        private GameState State;
        private LuaSceneManager LuaSceneManager;
        private InputHandler InputHandler;
        private EngineUpdater EngineUpdater;
        private FrameRenderer FrameRenderer;

        public Game() 
        {
            State = new GameState();
            LuaSceneManager = new LuaSceneManager(State._G);
            InputHandler = new LuaInputHandler(State);
            EngineUpdater = new StandartUpdater(State);
            FrameRenderer = new FullScreenRenderer(State, LuaSceneManager);
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
            State.Initialize_G();
            LuaSceneManager.AddLuaFunctions(FunctionsForLua());

            ///<summary>
            /// Директории которые создаются при запуске. Необходимы для игры
            /// </summary>
            var Directories = new List<string>()
            { "logs", "scenes", "cfg", "addons" };
            foreach (string Name in Directories)
            {
                if (!Directory.Exists(Name))
                    Directory.CreateDirectory(Name);
            }

            State.LoadConfig();
            State.StartGame();
        }

        private void GameLoop()
        {
            while (State.IsRunning)
            {
                var frameStart = DateTime.Now;

                // Костыль на "фиксированное" значение видимости курсора
                Console.CursorVisible = false;
                
                InputHandler.Handle();
                EngineUpdater.Update();
                FrameRenderer.Render();

                // Подгон под фпс
                int elapsed = (int)(DateTime.Now - frameStart).TotalMilliseconds;
                int delay = Math.Max(0, ( 1000 / Math.Max(1, Convert.ToInt32(State._G["fps_target"])) ) - elapsed);
                Thread.Sleep(delay);
            }
        }

        /// <summary>
        /// Здесь хранятся инструменты для созидания и последующей передачи в окружение Lua
        /// </summary>
        private Dictionary<string, object> FunctionsForLua()
        {
            return new Dictionary<string, object>()
            {
                {
                    "ScrW", (Func<int>)(() => { return State.ScreenWidth; })
                },
                {
                    "ScrH", (Func<int>)(() => { return State.ScreenHeight; })
                },
                {
                    "frameClear",
                    () =>
                    {
                        try { Console.SetCursorPosition(0, 0); }
                        finally { Console.Write(new string(State.DebugChar, State.ScreenSymbolsMax)); }
                        try { Console.SetCursorPosition(0, 0); } catch { }
                    }
                },
                {
                    "converter", (Action<LuaTable>)(args => {
                        foreach (var item in args.Values)
                            Console.Write(item);
                    })
                },
                {
                    "writeln", (Action<object>)(args => {
                        //foreach (var item in args.Values)
                            Console.Write(args);
                        Console.WriteLine();
                    })
                },
                {
                    "cursorPos", (Action<int, int>)((left, top) => { try { Console.SetCursorPosition(left, top); } catch { } })
                },
                {
                    "setScene", (Action<string>)(sceneName => { State.SetScene(sceneName); })
                },
            };
        }
    }
}
