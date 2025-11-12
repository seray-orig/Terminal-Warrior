using Terminal_Warrior.Game.scenes;

namespace Terminal_Warrior.Engine.Core
{
    public class FullScreenRenderer : FrameRenderer
    {
        private readonly LuaSceneManager _luaScenes;
        public FullScreenRenderer(GameState state, LuaSceneManager luaScenes) : base(state)
        {
            _luaScenes = luaScenes;
        }

        public override void Render()
        {
            _luaScenes.RunLua(_state.Scenes, _state.CurrentScene, "FrameRenderer");
        }
    }
}
