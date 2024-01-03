using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Talos.Forms;
using Talos.Properties;

namespace Talos
{
    public partial class MainForm : Form
    {

        internal Server Server { get; private set; }
        internal int ThreadID { get; set; }
        internal Dictionary<Client, TabPage> _clientDictionary;
        public MainForm()
        {
            InitializeComponent();
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            _clientDictionary = new Dictionary<Client, TabPage>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Server = new Server(this);
        }
        internal void RemoveClient(Client client)
        {
            if (InvokeRequired)
                Invoke((Action)delegate { RemoveClient(client); });
            else
            {
                if (_clientDictionary.ContainsKey(client))
                {
                    clientTabControl.Controls.Remove(_clientDictionary[client]);
                    _clientDictionary.Remove(client);
                    client.ClientTab.Dispose();
                    client.Remove();
                }
            }
        }

        internal void AddClient(Client client)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { AddClient(client); });
                return;
            }

            ClientTab clientTab = new ClientTab(client)
            {
                Dock = DockStyle.Fill
            };
            TabPage tabPage = new TabPage(client.Name);
            tabPage.Controls.Add(clientTab);
            clientTabControl.TabPages.Add(tabPage);
            _clientDictionary.Add(client, tabPage);
            Server._clientList.Add(client);
        }


        #region Launch Darkages
        internal Process LaunchDarkages(bool noWalls = false)
        {
            if (!Directory.Exists(Settings.Default.DataPath))
            {
                Directory.CreateDirectory(Settings.Default.DataPath);
            }
            string darkAgesPath = Settings.Default.DarkAgesPath;
            StartupInfo startupInfo = default;
            startupInfo.Size = Marshal.SizeOf(startupInfo);
            ProcessInformation information = default;
            NativeMethods.CreateProcess(darkAgesPath, null, IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.Suspended, IntPtr.Zero, null, ref startupInfo, out information);

            Process processById = Process.GetProcessById(information.ProcessId);
            IntPtr intptr_ = NativeMethods.OpenProcess_1(2035711u, 1, processById.Id);

            if (Settings.Default.useDawnd)
            {
                InjectDLL(intptr_, darkAgesPath.Replace("Darkages.exe", "dawnd.dll"));
            }
            using (ProcMemoryStream stream = new ProcMemoryStream(information.ProcessId,
                ProcessAccess.VmOperation | ProcessAccess.VmRead | ProcessAccess.VmWrite))
            {
                if (noWalls)
                {
                    stream.Position = 6281349L;
                    stream.WriteByte(144);
                    stream.WriteByte(144);
                    stream.WriteByte(144);
                    stream.WriteByte(144);
                    stream.WriteByte(144);
                }
                //force "socket" - call for direct ip
                stream.Position = 4404130L;
                stream.WriteByte(235);

                //edit the direct ip to the server loopback ip
                stream.Position = 4404162L;
                stream.WriteByte(106);
                stream.WriteByte(1);
                stream.WriteByte(106);
                stream.WriteByte(0);
                stream.WriteByte(106);
                stream.WriteByte(0);
                stream.WriteByte(106);
                stream.WriteByte(127);

                //change port
                stream.Position = 275684L;
                stream.WriteByte(50);
                stream.WriteByte(10);

                //skip intro
                stream.Position = 4384287L;
                stream.WriteByte(144);
                stream.WriteByte(144);
                stream.WriteByte(144);
                stream.WriteByte(144);
                stream.WriteByte(144);
                stream.WriteByte(144);
                if (!Settings.Default.useDawnd)
                {
                    //Allow multiple instances
                    stream.Position = 5744601L;
                    stream.WriteByte(235);
                }
                stream.Position = 7290020L;
                NativeMethods.ResumeThread(information.ThreadHandle);
            }
            return processById;
        }

        internal void InjectDLL(IntPtr hProcess, string buffer)
        {
            int num = buffer.Length + 1;
            IntPtr intPtr = NativeMethods.VirtualAllocEx(hProcess, (IntPtr)null, (uint)num, 4096u, 64u);
            NativeMethods.WriteProcessMemory_2(hProcess, intPtr, buffer, (UIntPtr)(ulong)num, out IntPtr intptr_2);
            UIntPtr procAddress = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (procAddress == UIntPtr.Zero)
            {
                MessageDialog.Show(this, "Error 1\n");
                return;
            }
            IntPtr value = NativeMethods.CreateRemoteThread(hProcess, (IntPtr)null, 0u, procAddress, intPtr, 0u, out intptr_2);
            if (value == IntPtr.Zero)
            {
                MessageDialog.Show(this, "Error 2 Try again..");
                return;
            }
            uint num2 = (uint)NativeMethods.WaitForSingleObject(value, 10000);
            if (num2 != 128L && num2 != 258L && num2 != uint.MaxValue)
            {
                NativeMethods.VirtualFreeEx(hProcess, intPtr, (UIntPtr)0uL, 32768u);
                if (value != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(value);
                }
            }
            else
            {
                MessageDialog.Show(this, " Error 3 Try again...");
                if (value != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(value);
                }
            }
        }
        #endregion

        private void launchDA_Click(object sender, EventArgs e)
        {
            LaunchDarkages();
        }

    }
}
