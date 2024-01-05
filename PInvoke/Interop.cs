using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.PInvoke
{
    internal static class Interop
    {
        internal struct FLASHWINFO
        {
            internal uint _cbSize;

            internal IntPtr _hWnd;

            internal uint _dwFlags;

            internal uint _uCount;

            internal uint _dwTimeout;
        }

        internal struct DWM_THUMBNAIL_PROPERTIES
        {
            internal int _dwFlags;

            internal RECT _rcDestination;

            internal RECT _rcSource;

            internal byte _opacity;

            internal bool _fVisible;

            internal bool _fSourceClientAreaOnly;
        }

        internal struct RECT
        {
            internal int _left;

            internal int _top;

            internal int _right;

            internal int _bottom;

            internal RECT(int left, int top, int right, int bottom)
            {
                _left = left;
                _top = top;
                _right = right;
                _bottom = bottom;
            }

        }

        internal struct PSIZE
        {
            internal int x;

            internal int y;
        }
    }
}
