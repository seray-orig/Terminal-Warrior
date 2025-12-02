

namespace Terminal_Warrior.Engine.Core
{
    public interface IFactory<T>
    {
        T Create();
    }
}
