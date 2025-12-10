using NLua;
using System.Text;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.game.scenes;

namespace Terminal_Warrior.Engine.Implementations
{
    public class InputLuaHandler : InputHandler
    {
        private StringBuilder _readKey = new StringBuilder();
        private StringBuilder _readChar = new StringBuilder();
        public InputLuaHandler(GameContext gameContext) : base(gameContext) { }
        public override void Handle()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                _readKey.Clear(); _readKey.Append(key.Key.ToString());
                _readChar.Clear(); _readChar.Append(key.KeyChar);

                // Перехват нажатия ~ на открытие консоли на любой сцене
                if (_readChar.ToString() == "~" && _sceneManager.CurrentScene != "cmd")
                {
                    _sceneManager.SetScene("cmd");
                }
                else if (_readChar.ToString() == "~" && _sceneManager.CurrentScene == "cmd")
                {
                    _sceneManager.SetScene(_sceneManager.PreviousScene);
                }
                // Перехват нажатия на перезагрузку Lua
                else if (_readChar.ToString() == "}")
                {
                    _luaContext.ReloadLua();
                }

                _sceneManager.CallFunc("InputHandler", _readKey.ToString(), _readChar.ToString());
            }
        }
    }
}
