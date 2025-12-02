using Terminal_Warrior.Engine.Core;

namespace Terminal_Warrior.Logger.Util
{
    public class MessageValidator : IValidator
    {
        public bool IsValid(object[] obj)
        {
            if (obj == null) return false;
            
            string message;
            foreach (var item in obj)
            {
                if (item == null) return false;
                try { message = Convert.ToString(item)!; }
                catch { return false; }
                if (string.IsNullOrEmpty(message)) return false;
            }

            return true;
        }
    }
}