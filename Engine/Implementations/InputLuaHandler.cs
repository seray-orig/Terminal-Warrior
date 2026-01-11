using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Engine.Implementations
{
    public sealed class InputLuaHandler : InputHandler
    {
        public InputLuaHandler(GameContext gameContext) : base(gameContext) { }

        public override void Handle()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();

                // Перехват нажатия на открытие консоли на любой сцене
                if (key.KeyChar == _convar["second_scene_char"] && _sceneManager.CurrentScene != _convar["second_scene_name"])
                {
                    _sceneManager.SetScene("cmd");
                }
                else if (key.KeyChar == _convar["second_scene_char"] && _sceneManager.CurrentScene == _convar["second_scene_name"])
                {
                    _sceneManager.SetScene(_sceneManager.PreviousScene);
                }
                // Перехват нажатия на перезагрузку Lua
                else if (key.KeyChar == _convar["hot_lua_reload_char"])
                {
                    HotLuaReload();
                }

                _sceneManager.CallFunc("InputHandler", key.Key.ToString(), Convert.ToString(key.KeyChar));
            }
        }
    }
}
