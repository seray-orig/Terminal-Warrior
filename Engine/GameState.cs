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
        public Lua _G { get; } = new Lua();
        public void Initialize_G()
        {
            _G.State.Encoding = Encoding.UTF8;

            
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

        //
        //  Рендер
        //
        public char DebugChar { get; private set; } = ' ';
        public void SetDebugChar(char symbol) { DebugChar = symbol; }
    }
}
