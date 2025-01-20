using System;

namespace Talos.Utility
{
    internal class RandomUtils
    {
        internal static Random _random = new Random();

        internal static int Random() => _random.Next();

        internal static int Random(int maxValue) => _random.Next(maxValue);

        internal static int Random(int minValue, int maxValue) => _random.Next(minValue, maxValue);

        internal static T RandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }
    }
}
