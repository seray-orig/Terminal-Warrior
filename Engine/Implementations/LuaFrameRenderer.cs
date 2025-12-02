using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine.Implementations
{
    public class LuaFrameRenderer : FrameRenderer
    {
        public LuaFrameRenderer(GameContext gameContext) : base(gameContext) { }

        public override void Render()
        {
            // Очистка консоли перед отрисовкой нового кадра
            try { Console.SetCursorPosition(0, 0); }
            finally { Console.Write(_state.ScreenSymbolsMax); }
            try { Console.SetCursorPosition(0, 0); } catch { }

            _sceneManager.CallFunc("FrameRenderer");
        }
    }
}
