using NLua;
using System.Text;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

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
                if (key.KeyChar == _convar["second_scene_char"].GetConVar() && _sceneManager.CurrentScene != _convar["second_scene_name"].GetConVar())
                {
                    _sceneManager.SetScene("cmd");
                }
                else if (key.KeyChar == _convar["second_scene_char"].GetConVar() && _sceneManager.CurrentScene == _convar["second_scene_name"].GetConVar())
                {
                    _sceneManager.SetScene(_sceneManager.PreviousScene);
                }
                // Перехват нажатия на перезагрузку Lua
                else if (key.KeyChar == _convar["hot_lua_reload_char"].GetConVar())
                {
                    _luaContext.HotLuaReload();
                }

                _sceneManager.CallFunc("InputHandler", key.Key.ToString(), Convert.ToString(key.KeyChar));
            }
        }
    }
}
