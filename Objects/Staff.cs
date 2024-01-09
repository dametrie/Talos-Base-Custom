using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Talos.Enumerations;

namespace Talos.Objects
{
    internal class Staff
    {
        private TemuairClass _temuairClass;

        internal string Name { get; set; }
        internal int AbilityRequired { get; set; }
        internal int InsightRequired { get; set; }
        internal bool MasterRequired { get; set; }
        internal Dictionary<string, byte> CastLines { get; private set; }
        internal Staff()
        {
            Name = "";
            AbilityRequired = 0;
            InsightRequired = 0;
            MasterRequired = false;
            _temuairClass = TemuairClass.Peasant;
            CastLines = new Dictionary<string, byte>();
        }
        internal Staff(string name, Dictionary<string, byte> castLines, int abilityRequired, int insightRequired, bool masterRequired, TemuairClass temuairClass = TemuairClass.Peasant)
        {
            Name = name;
            CastLines = castLines;
            AbilityRequired = abilityRequired;
            InsightRequired = insightRequired;
            MasterRequired = masterRequired;
            _temuairClass = temuairClass;
        }
    }
}
