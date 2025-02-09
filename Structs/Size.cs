using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Talos.Structs
{
    internal class Size
    {
        internal short Width { get; set; }
        internal short Height { get; set; }

        internal Size(short width, short height) {
            Width = width; Height = height;
        }

        public static bool operator ==(Size a, Size b) => a.Equals(b);

        public static bool operator !=(Size a, Size b) => !a.Equals(b);

        public virtual bool Equals(object obj)
        {
            return obj is Size size && size.Width == Width && size.Height == Height;
        }
        public virtual int GetHashCode() => (Height << 16) + Height;
        public virtual string ToString()
        {
            return string.Format("{0}x{1}", Height, Height);
        }
    }
}
