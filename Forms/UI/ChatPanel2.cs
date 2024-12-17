using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace Talos.Forms.UI
{
    public class ChatPanel2 : RichTextBox
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct ScrollInfo
        {
            public uint cbSize;
            public uint fMask;
            public uint nMin;
            public uint nMax;
            public uint nPage;
            public uint nPos;
            public uint nTrackPos;
        }

        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const int EM_GETSCROLLPOS = 0x0400 + 221;
        private const int EM_SETSCROLLPOS = 0x0400 + 222;
        System.Drawing.Point emptyPoint = System.Drawing.Point.Empty;

        public bool AutoDetectUrls
        {
            get { return DetectUrls; }
            set { DetectUrls = value; }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref System.Drawing.Point lParam);

        [DllImport("user32.dll")]
        private static extern int GetScrollInfo(IntPtr hwnd, int fnBar, ref ScrollInfo lpsi);

        public ChatPanel2()
        {
            AutoDetectUrls = false;
        }

        public void AppendTextAtPosition(string text, int position)
        {
            if (position < 0 || position > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            SelectionStart = position;
            base.SelectedText = text;
            Select(position, text.Length);
            ScrollToCaret();
            Select(position + text.Length, 0);
        }

        public void AppendRtfAtPosition(string rtfText, string plainText, int position)
        {
            if (position < 0 || position > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            SelectionStart = position;
            SelectedRtf = $"{{\\rtf1\\ansi {rtfText}\\v #{plainText}\\v0}}";
            Select(position, rtfText.Length + plainText.Length + 1);
            ScrollToCaret();
            Select(position + rtfText.Length + plainText.Length + 1, 0);
        }

        public void ScrollToEnd(bool smoothScroll)
        {
            if (smoothScroll)
                SmoothScrollToEnd();
            else
                SelectionStart = base.Text.Length;
        }

        public int GetScrollPosition()
        {
            return GetScrollBarPosition(SB_VERT);
        }

        private void SmoothScrollToEnd()
        {
            SendMessage(Handle, EM_SETSCROLLPOS, 0, ref emptyPoint);
        }

        private int GetScrollBarPosition(uint sbType)
        {
            ScrollInfo scrollInfo = new ScrollInfo
            {
                cbSize = (uint)Marshal.SizeOf(typeof(ScrollInfo)),
                fMask = 0x10 | 0x01, // SIF_RANGE | SIF_POS
                nMin = 0,
                nMax = 0,
                nPage = 0,
                nPos = 0,
                nTrackPos = 0
            };

            GetScrollInfo(Handle, (int)sbType, ref scrollInfo);
            return (int)scrollInfo.nPos;
        }

        public bool IsScrollBarAtBottom()
        {
            int min, max;
            GetScrollRange(Handle, SB_VERT, out min, out max);
            System.Drawing.Point pt = System.Drawing.Point.Empty;
            SendMessage(Handle, EM_GETSCROLLPOS, 0, ref pt);
            return pt.Y + ClientSize.Height >= max;
        }

        private void GetScrollRange(IntPtr hwnd, uint fnBar, out int minPos, out int maxPos)
        {
            minPos = 0;
            maxPos = 0;

            ScrollInfo si = new ScrollInfo();
            si.cbSize = (uint)Marshal.SizeOf(si);
            si.fMask = 0x10; // SIF_RANGE

            if (GetScrollInfo(hwnd, (int)fnBar, ref si) != 0)
            {
                minPos = (int)si.nMin;
                maxPos = (int)si.nMax;
            }
        }
    }
}
