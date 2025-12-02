
namespace Terminal_Warrior.Engine.Core
{
    public abstract class FrameRenderer : CoreContext
    {
        protected FrameRenderer(GameContext gameContext) : base(gameContext) { }
        public abstract void Render();
    }
}
