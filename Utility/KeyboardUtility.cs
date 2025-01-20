using System.Windows.Forms;

namespace Talos.Utility
{
    internal static class KeyboardUtility
    {
        internal const uint RightWindowsKeyMask = 8u;
        internal const uint LeftWindowsKeyMask = 8u;
        internal const uint ShiftKeyMask = 4u;
        internal const uint ControlKeyMask = 2u;
        internal const uint AltKeyMask = 1u;

        internal static uint GetKeyModifierMask(Keys key)
        {
            switch (key)
            {
                case Keys.RWin:
                    return RightWindowsKeyMask;
                case Keys.LWin:
                    return LeftWindowsKeyMask;

                case Keys.ShiftKey:
                    return ShiftKeyMask;
                case Keys.Shift:
                    return ShiftKeyMask;

                case Keys.ControlKey:
                    return ControlKeyMask;
                case Keys.Control:
                    return ControlKeyMask;

                case Keys.Menu:
                    return AltKeyMask;
                case Keys.Alt:
                    return AltKeyMask;

                default:
                    return 0u;
            }
        }
    }
}
