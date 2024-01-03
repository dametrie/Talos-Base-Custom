using System;

namespace Talos
{
    internal class Utility
    {
        private static Random _random = new Random();

        public static int Random() => _random.Next();

        public static int Random(int maxValue) => _random.Next(maxValue);

        public static int Random(int minValue, int maxValue) => _random.Next(minValue, maxValue);
    }
}
