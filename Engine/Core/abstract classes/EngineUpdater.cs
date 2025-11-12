
namespace Terminal_Warrior.Engine.Core
{
    public abstract class EngineUpdater
    {
        protected readonly GameState _state;
        protected EngineUpdater(GameState state)
        {
            _state = state;
        }
        public abstract void Update();
    }
}
