
namespace Terminal_Warrior.Logger.Util
{
    public static class Perebor
    {
        // Рекурсивное доставание итоговых объектов, если есть коллекции.
        // Блок кода, которым я горжусь!
        // Ни одна нейронка не могла выдать даже похожего результата,
        // а я написал! (Основано на рекурсивной функции числа Фибоначчи)
        public static string Do(dynamic obj)
        {
            if (obj is string)
                return obj;

            // Раньше была проверка на интерфейс IEnumerable,
            // но как оказалось для foreach не обязательно его явно реализовывать.
            // У foreach утиная типизация - достаточно просто иметь метод,
            // который возвращает объект IEnumerator.
            //
            // Благодаря try \ catch проверки на метод GetEnumerator
            // был убран блок switch \ case.
            //
            // Такой подход позволяет работать с legacy кодом -
            // кастомными типами данных, которые могут не реализовывать интерфейсы .NET,
            // а возвращать готовые енумераторы, например IDictionaryEnumerator,
            // как это делает LuaTable.
            try
            {
                foreach (var item in obj)
                    Do(item);
            }
            // Если метода GetEnumerator нет - объект итоговый, не коллекция.
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) { }
            // Если метод есть, но возникла ошибка.
            catch { return String.Empty; }

            return obj.ToString();
        }
    }
}
