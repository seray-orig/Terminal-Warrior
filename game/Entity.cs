

using Terminal_Warrior.Logger;

namespace Terminal_Warrior.game
{
    public class Entity
    {
        public readonly static Dictionary<Guid, Entity> EntityDictionary = new();

        public readonly Guid Id;
        public string Name { get; protected set; }
        public (uint, uint) Position { get; protected set; }
        public void SetPosition((uint, uint) coordinates)
        {
            Position = (Math.Max(coordinates.Item1, 1), Math.Max(coordinates.Item2, 1));
        }
        public uint Left { get { return Position.Item1; } }  // Значения по коносли - слева направо, сверху вниз.
        public uint Top { get { return Position.Item2; } }
        private DateTime _movePenalty = DateTime.Now;

        public Entity(string name, (uint, uint) coordinates)
        {
            Id = Guid.NewGuid();
            Name = name;
            SetPosition(coordinates);

            EntityDictionary.Add(Id, this);
        }

        public void Move(object left, object top, object speed)
        {
            // Некоторое подобие скорости передвижения :/
            if (Convert.ToByte(speed) > 0)
            {
                try
                {
                    TimeSpan time = DateTime.Now - _movePenalty;
                    // Делим скорость на еденицу и получаем "вермя перезарядки" ходьбы
                    if (time.TotalSeconds < 1.0 / Convert.ToByte(speed))
                        return;
                }
                catch (Exception ex) { ErrorCmdLogger.StaticLog($"Не удалось вычислить скорость энтити {Name}: {ex.Message}"); }
            }

            try
            {
                SetPosition(
                    (Convert.ToUInt32(Left + Convert.ToInt64(left)), Convert.ToUInt32(Top + Convert.ToInt64(top)))
                );
                _movePenalty = DateTime.Now;
            }
            catch (Exception ex) { ErrorCmdLogger.StaticLog($"Не удалось сдвинуть энтити {Name}: {ex.Message}"); }
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
