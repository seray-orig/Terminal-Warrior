using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;
using NLua;
using Terminal_Warrior.Logger;

namespace Terminal_Warrior.Engine.Implementations
{
    public class FrameLuaRenderer : FrameRenderer
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
        }
    }
}
