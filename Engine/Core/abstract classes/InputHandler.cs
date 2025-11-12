
namespace Terminal_Warrior.Engine.Core
{
    public abstract class InputHandler
    {
        protected readonly GameState _state;
        protected InputHandler(GameState state)
        {
            _state = state;
        }
        public abstract void Handle();
    }
}
