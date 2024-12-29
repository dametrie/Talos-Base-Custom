using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Utility
{
    internal class HashingUtils
    {
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
