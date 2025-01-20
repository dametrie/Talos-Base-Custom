using System;

namespace Talos.PInvoke
{
    internal struct StartupInfo
    {
        internal int Size { get; set; }

        internal string Reserved { get; set; }

        internal string Desktop { get; set; }

        internal string Title { get; set; }

        internal int X { get; set; }

        internal int Y { get; set; }

        internal int Width { get; set; }

        internal int Height { get; set; }

        internal int XCountChars { get; set; }

        internal int YCountChars { get; set; }

        internal int FillAttribute { get; set; }

        internal int Flags { get; set; }

        internal short ShowWindow { get; set; }

        internal short Reserved2 { get; set; }

        internal IntPtr Reserved3 { get; set; }

        internal IntPtr StdInputHandle { get; set; }

        internal IntPtr StdOutputHandle { get; set; }

        internal IntPtr StdErrorHandle { get; set; }
    }
}
