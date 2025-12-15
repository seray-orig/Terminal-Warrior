using NLua;

namespace Terminal_Warrior.game
{
    public class Npc : Entity
    {
        public readonly static Dictionary<Guid, Npc> NpcDictionary = new();

        public int HealthPoints { get; protected set; }

        public Npc(string name, int healthPoints, (uint, uint) coordinates, string texture) : base(name, texture, coordinates)
        {
            HealthPoints = healthPoints;

            NpcDictionary.Add(base.Id, this);
        }

        public virtual void Move(uint Left, uint Top)
        {
            base.Position = (base.Left + Left, base.Top + Top);
        }
        public virtual void MoveTo(uint Left, uint Top)
        {
            base.Position = (Left, Top);
        }

        public override string ToString()
        {
            return $"Object: {base.ToString()} Name: {Name}";
        }
    }
}
