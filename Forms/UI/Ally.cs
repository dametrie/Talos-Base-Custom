using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Forms.UI
{
    internal class Ally
    {
        internal string Name { get; set; }
        internal AllyPage AllyPage { get; set; }
        internal Ally(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }   

    }
}
