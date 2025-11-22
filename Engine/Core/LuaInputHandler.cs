using NLua;
using System.Text;
using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine.Core
{
    public class LuaInputHandler : InputHandler
    {
        private readonly LuaSceneManager _luaScenes;
        private StringBuilder _readKey = new StringBuilder();
        private StringBuilder _readChar = new StringBuilder();
        private StringBuilder _lastScene;
        public LuaInputHandler(GameState state, LuaSceneManager luaScenes) : base(state)
        {
            _luaScenes = luaScenes;
            _lastScene = new StringBuilder(_state.CurrentScene);
        }
        public override void Handle()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                _readKey.Clear(); _readKey.Append(key.Key.ToString());
                _readChar.Clear(); _readChar.Append(key.KeyChar);
                _state._G["_readKey"] = _readKey.ToString();
                _state._G["_readChar"] = _readChar.ToString();

                // Перехват нажатия ~ на открытие консоли на любой сцене
                if (_readChar.ToString() == "~" && _state.CurrentScene != "cmd")
                {
                    _lastScene.Clear(); _lastScene.Append(_state.CurrentScene);
                    _state.SetScene("cmd");
                }
                else if (_readChar.ToString() == "~" && _state.CurrentScene == "cmd")
                {
                    _state.SetScene(_lastScene.ToString());
                }

                _luaScenes.RunLua(_state.Scenes, _state.CurrentScene, "InputHandler");
            }
        }
    }
}
