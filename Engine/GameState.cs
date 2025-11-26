using NLua;
using System.Reflection;
using System.Text;
using Terminal_Warrior.Logger;

namespace Terminal_Warrior.Engine
{
    public class GameState
    {
        //
        //  Движок \ Свойства
        //
        public Lua _G { get; private set; } = new Lua();
        public void Initialize_G()
        {
            _G.State.Encoding = Encoding.UTF8;

            // Write & Writeln вызывали утечку памяти по неизвестной причине
            // решено реализовать их так - средствами Lua
            _G.DoString("""

                Writeln = print
                Write = io.write

             """);
        }
        private string configPath { get; } = "cfg/config.lua";
        public void LoadConfig()
        {
            if (!File.Exists(configPath))
                CreateConfig();
            try
            {
                _G.DoFile(configPath);
            }
            catch (Exception ex)
            {
                try { Console.SetCursorPosition(1, 1); }
                finally
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("/!\\");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" Что-то создаёт скриптовые ошибки.");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                    Console.Write("используется стандартный файл конфигурации.");
                    Thread.Sleep(10);
                }

                _G.DoString(new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.cfg.config.lua")!, Encoding.UTF8).ReadToEnd());
            }
        }
        private void CreateConfig()
        {
            File.Create(configPath).Close();
            File.WriteAllText(configPath, new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.cfg.config.lua")!, Encoding.UTF8).ReadToEnd());
        }

        public bool IsRunning { get; private set; } = false;
        public void StartGame() { IsRunning = true; }
        public void ShutDownGame() { IsRunning = false; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public StringBuilder ScreenSymbolsMax { get; private set; } = new StringBuilder();
        public void UpdateScreenSize(int Width, int Height)
        {
            ScreenWidth = Width; ScreenHeight = Height;
            ScreenSymbolsMax.Clear();
            ScreenSymbolsMax.Append(DebugChar, Width * Height);
        }
        public static FileLogger ErrorLogger = new FileLogger("logs/errors.txt", new FileSystemService(), new MessageValidator());

        //
        //  Рендер
        //
        public char DebugChar { get; private set; } = ' ';
        public void SetDebugChar(char symbol) { DebugChar = symbol; }

        //
        //  Сцены
        //
        public StringBuilder _currentScene = new StringBuilder("MainMenuTest");
        public string CurrentScene { get { return _currentScene.ToString(); } }
        public void SetScene(string scene) { _currentScene.Clear(); _currentScene.Append(scene); }
        // Имя сцены = Lua код
        public Dictionary<string, (string, string)> Scenes { get; private set; } = new Dictionary<string, (string, string)>()
        {
            {   // Эти сцены зашиты в игру
                "MainMenu", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.Game.scenes.MainMenu.lua")!, Encoding.UTF8).ReadToEnd())
            },
            {
                "Gameplay", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.Game.scenes.Gameplay.lua")!, Encoding.UTF8).ReadToEnd())
            },
            {
                "cmd", (
                "Internal",
                new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Terminal_Warrior.Game.scenes.cmd.lua")!, Encoding.UTF8).ReadToEnd())
            },
        };
        public void AddScene(string Name, string LuaCode)
        {
            if (!Scenes.ContainsKey(Name)) Scenes.Add(Name, ("File", LuaCode));
        }
        public void RemoveScene(string Name)
        {
            Scenes.Remove(Name);
        }
    }
}
