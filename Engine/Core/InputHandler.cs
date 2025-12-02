
namespace Terminal_Warrior.Engine.Core
{
    public abstract class InputHandler : CoreContext
    {
        protected InputHandler(GameContext gameContext) : base(gameContext) { }
        public abstract void Handle();
    }
}
