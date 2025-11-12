
namespace Terminal_Warrior.Engine.Core
{
    public class StandartUpdater : EngineUpdater
    {
        public StandartUpdater(GameState state) : base(state) { }

        public override void Update()
        {
            // Обновление полей характеристик
            _state.UpdateScreenSize(Console.WindowWidth, Console.WindowHeight);
            _state.LoadConfig();

            // Очистка предыдущих сцен
            foreach (var Scene in _state.Scenes)
            {
                (string Mode, string LuaCode) = Scene.Value;
                if (Mode == "File") _state.RemoveScene(Scene.Key);
            }

            // Сперва монтируются скрипты внутренних папок
            MountScenes("scenes");

            // Вторичное группированное монтирование скриптов из папки addons
            var AddonsList = Directory.GetDirectories("addons");
            foreach (string addonPath in AddonsList)
            {
                var AddonContent = Directory.GetDirectories(addonPath);
                foreach (string directoryPath in AddonContent)
                {
                    var directoryName = Path.GetFileName(directoryPath);
                    switch (directoryName)
                    {
                        case "scenes":
                            MountScenes(directoryPath);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Монтирование скриптов сцен из указанной папки
        /// </summary>
        private void MountScenes(string path)
        {
            var FileList = Directory.GetFiles(path, "*.lua");
            foreach (string filePath in FileList)
            {
                var Scene = Path.GetFileName(filePath);
                _state.AddScene(Scene.Substring(0, Scene.Length - 4), filePath);
            }
        }
    }
}
