

namespace Terminal_Warrior.Engine.Core
{
    public struct LayerAction
    {
        public Action Action;
        public int Layer;
    }

    // Костыль на слои рендера в консоли.
    // Если вызывать Write или консольный логгер до рендера,
    // то их сообщения стираются очисткой FrameLuaRenderer.
    // Смысла в невидимых сообщениях об ошибках нету,
    // поэтому такой костыль вполне неплох, оно работает!
    public static class ConsoleExtended
    {
        public static readonly List<LayerAction> Actions = new();

        public static void AddLayer(Action action, int layer)
        {
            Actions.Add(new LayerAction { Action = action, Layer = layer });
        }
    }
}
