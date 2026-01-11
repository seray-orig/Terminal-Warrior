/*
 * CONSOLE VARIABLE
 * 
 * Глобальные переменные - ещё один мост между Lua и ядром C#
 * Что создано в окружении Lua можно использовать как
 * и далее в скриптах, так и здесь в C#.
 * 
 * Из config.lua используется в Game как значение "фпс",
 * как имя для консольного окна,
 * в FrameLuaRenderer как отладочный символ,
 * и другие переменные в других местах кода.
 * 
 * Все эти значения можно менять на ходу
 * без перезапуска консольного приложения.
*/

using NLua;

namespace Terminal_Warrior.Engine
{
    public class ConVar : IDisposable
    {
        // 1 - Имя
        // 2 - Значение
        // 3 / 4 - Пределы min, max значения (2) если оно int, если нет, то необязательно
        private Dictionary<string, (dynamic, (int?, int?))> _convars = new();

        public dynamic this[string index]
        {
            get
            {
                if (_convars.ContainsKey(index))
                    return _convars[index].Item1;
                return null!;
            }

            set
            {
                if (string.IsNullOrEmpty(index) || !(value is LuaTable) || value[2] == null)
                    return;

                // Если несуществующей переменной присваивается значение,
                // то создаём эту переменную
                if (!_convars.ContainsKey(index))
                {
                    CreateConVar(index, value);
                }
                else
                {
                    SetConVar(index, value);
                }
            }
        }

        private void CreateConVar(string index, LuaTable value)
        {
            (int?, int?) limits = new();
            if (value[3] != null && value[3] is long) // min
            {
                if (value[4] != null && value[4] is long) // max
                    limits = (Convert.ToInt32(value[3]), Convert.ToInt32(value[4]));
                else
                    limits = (Convert.ToInt32(value[3]), null);
            }

            SetConVar(index, value, limits);
        }

        private void SetConVar(string index, LuaTable value, (int?, int?) limits = new())
        {
            switch (value[2])
            {
                // В Lua целое число - это тип Int64
                case long l:
                    int intValue = Convert.ToInt32(l);
                    int? min, max;
                    if (this[index] != null)
                    {
                        min = _convars[index].Item2.Item1;
                        max = _convars[index].Item2.Item2;
                    }
                    else
                    {
                        min = limits.Item1;
                        max = limits.Item2;
                    }

                    if (max != null && intValue > max)
                    {
                        _convars[index] = (max, (min, max));
                        return;
                    }
                    if (min != null && intValue < min)
                    {
                        _convars[index] = (min, (min, max));
                        return;
                    }

                    _convars[index] = (intValue, (min, max));
                    break;

                case string s:
                    if (s.Length == 1)
                        _convars[index] = (Convert.ToChar(s), (null, null));
                    else
                        _convars[index] = (s, (null, null));
                    break;

                default:
                    _convars[index] = (value[2], (null, null));
                    break;
            }
        }

        public void Dispose()
        {
            _convars.Clear();
        }
    }
}
