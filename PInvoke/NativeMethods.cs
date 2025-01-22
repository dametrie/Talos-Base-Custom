using System;
using System.Runtime.InteropServices;
using System.Text;
using Talos.Structs;
using Talos.Definitions;
using System.Diagnostics;

namespace Talos.PInvoke
{
    internal class NativeMethods
    {
        #region Kernel32
        [DllImport("kernel32.dll")]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        internal static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

        [DllImport("kernel32")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        internal static extern IntPtr OpenProcess_1(uint uint_0, int int_0, int int_1);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        internal static extern bool ReadProcessMemory_1(IntPtr hProcess, int int_0, [Out] byte[] byte_0, int int_1, byte byte_1);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern WaitEventResult WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        internal static extern bool WriteProcessMemory_1(IntPtr hProcess, int addressOffset, byte[] lpBuffer, int nSize, byte flag);
   
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        internal static extern bool WriteProcessMemory_2(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        internal static bool ReadMemoryFromProcess(Process process, int memoryAddress, byte[] buffer) =>
        ReadProcessMemory_1(process.Handle, memoryAddress, buffer, buffer.Length, 0);

        internal static bool WriteMemoryToProcess(Process process, int memoryAddress, byte[] data) =>
        WriteProcessMemory_1(process.Handle, memoryAddress, data, data.Length, 0);


        #endregion

        #region User32

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        internal static extern int MapVirtualKey(int uCode, int uMapType);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowRect(IntPtr hwnd, ref Rect lpRect);

        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        internal static extern bool GetWindowRect_1(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hDc, int nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(IntPtr hWnd, string clientName);

        [DllImport("user32.dll")]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        internal static extern IntPtr PostMessage_1(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FlashWindowEx(ref Interop.FLASHWINFO flashwinfo);

        [DllImport("User32.dll")]
        internal static extern int SetForegroundWindow(int handle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        internal static extern bool IsWindow(IntPtr hWnd);

        internal static Interop.FLASHWINFO fInfo(IntPtr hWnd, uint dwFlags, uint uCount, uint dwTimeout)
        {
            Interop.FLASHWINFO flashwinfo = default;
            flashwinfo._cbSize = Convert.ToUInt32(Marshal.SizeOf(flashwinfo));
            flashwinfo._hWnd = hWnd;
            flashwinfo._dwFlags = dwFlags;
            flashwinfo._uCount = uCount;
            flashwinfo._dwTimeout = dwTimeout;
            return flashwinfo;
        }

        internal static void CloseWindowByWindowName(string clientName)
        {
            IntPtr intPtr = FindWindow(IntPtr.Zero, clientName);
            if (!(intPtr == IntPtr.Zero))
            {
                SendMessage(intPtr, 16u, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion
    }


}
