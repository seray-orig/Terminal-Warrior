
namespace Terminal_Warrior.Engine.Core
{
    public abstract class FrameRenderer
    {
        protected readonly GameState _state;
        protected FrameRenderer(GameState state)
        {
            _state = state;
        }
        public abstract void Render();
    }
}
