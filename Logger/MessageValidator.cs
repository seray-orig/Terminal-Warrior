

namespace Terminal_Warrior.Logger
{
    public class MessageValidator : IMessageValidator
    {
        public bool IsValid(string message) => !string.IsNullOrEmpty(message);
    }
}