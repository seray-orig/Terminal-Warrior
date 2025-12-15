using NLua;

namespace Terminal_Warrior.game
{
    public class Entity
    {
        public readonly static Dictionary<Guid, Entity> EntityDictionary = new();

        public readonly Guid Id;
        public string Name { get; protected set; }
        public string Texture { get; protected set; }
        public (uint, uint) Position { get; protected set; }
        public uint Left { get { return Position.Item1; } }  // Значения по коносли - слева на права, сверху вниз.
        public uint Top { get { return Position.Item2; } }

        public Entity(string name, string texture, (uint, uint) coordinates)
        {
            Id = Guid.NewGuid();
            Name = name;
            Texture = texture;
            Position = coordinates;

            EntityDictionary.Add(Id, this);
        }

        public void Draw()
        {
            Console.Write(Texture);
        }

        public void Kill()
        {
            EntityDictionary.Remove(Id);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
