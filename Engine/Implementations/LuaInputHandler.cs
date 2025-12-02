using KeraLua;
using NLua;
using System.Text;
using Terminal_Warrior.Engine.Core;
using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine.Implementations
{
    public class LuaInputHandler : InputHandler
    {
        private StringBuilder _readKey = new StringBuilder();
        private StringBuilder _readChar = new StringBuilder();
        private StringBuilder _lastScene;
        public LuaInputHandler(GameContext gameContext) : base(gameContext)
        {
            _lastScene = new StringBuilder(_sceneManager.CurrentScene);
        }
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
                    _lastScene.Clear(); _lastScene.Append(_sceneManager.CurrentScene);
                    _sceneManager.SetScene("cmd");
                }
                else if (_readChar.ToString() == "~" && _sceneManager.CurrentScene == "cmd")
                {
                    _sceneManager.SetScene(_lastScene.ToString());
                }

                _sceneManager.CallFunc("InputHandler", _readKey.ToString(), _readChar.ToString());
            }
        }
    }
}
