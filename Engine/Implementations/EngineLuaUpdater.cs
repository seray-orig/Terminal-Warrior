using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.Engine.Implementations
{
    public sealed class EngineLuaUpdater : EngineUpdater
    {
        public EngineLuaUpdater(GameContext gameContext) : base(gameContext) { }

        public override void Update()
        {
            // Обновление полей характеристик
            _state.UpdateScreenSize(Console.WindowWidth, Console.WindowHeight);

            _sceneManager.CallFunc("EngineUpdater");
        }
    }
}
