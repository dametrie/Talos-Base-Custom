using System;

namespace Talos
{
    internal class Utility
    {
        internal static Random _random = new Random();

        internal static int Random() => _random.Next();

        internal static int Random(int maxValue) => _random.Next(maxValue);

        internal static int Random(int minValue, int maxValue) => _random.Next(minValue, maxValue);

        internal static uint CalculateFNV(string hash)
        {
            uint num = default(uint);
            if (hash != null)
            {
                num = 2166136261u;
                for (int i = 0; i < hash.Length; i++)
                {
                    num = (hash[i] ^ num) * 16777619;
                }
            }
            return num;
        }
    }
}
