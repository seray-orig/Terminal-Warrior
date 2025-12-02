
namespace Terminal_Warrior.Engine.Core
{
    public abstract class EngineUpdater : CoreContext
    {
        protected EngineUpdater(GameContext gameContext) : base(gameContext) { }
        public abstract void Update();
    }
}
