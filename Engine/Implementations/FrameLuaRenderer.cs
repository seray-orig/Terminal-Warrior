using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Engine.Implementations
{
    public sealed class FrameLuaRenderer : FrameRenderer
    {
        public FrameLuaRenderer(GameContext gameContext) : base(gameContext) { }
        
        public override void Render()
        {
            char DebugChar = _convar["DebugChar"].GetConVar();

            // Очистка консоли перед отрисовкой нового кадра
            try { Console.SetCursorPosition(0, 0); }
            finally { Console.Write(new string(DebugChar, _state.ScreenSymbolsMax)); }
            try { Console.SetCursorPosition(0, 0); } catch { }

            _sceneManager.CallFunc("FrameRenderer");

            // Костыль. Общая точка рендера Write & WriteLine
            // Читать подробнее в ConsoleExtended.cs
            foreach (var list in ConsoleExtended.Actions.ToArray().OrderBy(m => m.Layer))
            {
                list.Action.Invoke();
            }
            ConsoleExtended.Actions.Clear();
        }
    }
}
