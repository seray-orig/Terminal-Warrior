using NLua;

namespace Terminal_Warrior.game
{
    // Забавно, игрок наследуется от неигрового персонажа...
    public sealed class Player : Npc
    {
        public static Player? CurrentPlayer;
        public Player(string Name, int HP, (uint, uint) SpawnPoint, string Texture) : base(Name, HP, SpawnPoint, Texture)
        {
            CurrentPlayer = this;
        }
    }
}
