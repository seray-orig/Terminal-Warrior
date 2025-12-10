using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.Engine.Implementations
{
    public class EngineLuaUpdater : EngineUpdater
    {
        public EngineLuaUpdater(GameContext gameContext) : base(gameContext) { }

        public override void Update()
        {
            // Обновление полей характеристик
            _state.UpdateScreenSize(Console.WindowWidth, Console.WindowHeight);

            // Сперва монтируются скрипты внутренних папок
            MountDirectory("Game/scenes");
        }

        /// <summary>
        /// Монтирование скриптов сцен из указанной папки
        /// </summary>
        private void MountDirectory(string path)
        {
            var FileList = Directory.GetFiles(path, "*.lua");
            foreach (string filePath in FileList)
            {
                var Scene = Path.GetFileName(filePath);
                _sceneManager.AddScene(Scene.Substring(0, Scene.Length - 4), filePath);
            }
        }
    }
}
