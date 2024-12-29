using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Talos.Forms;
using Talos.PInvoke;
using Talos.Properties;
using MapsCacheEditor;
using Talos.Base;
using Talos.Definitions;
using Talos.Capricorn.IO;
using Talos.Forms.UI;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using Newtonsoft.Json;
using Talos.Utility;

namespace Talos
{
    public partial class MainForm : Form
    {

        internal Server Server { get; private set; }
        internal int ThreadID { get; set; }
        private Dictionary<Client, TabPage> _clientTabs = new Dictionary<Client, TabPage>();
        internal Dictionary<string, DateTime> _consecutiveLogin = new Dictionary<string, DateTime>();
        internal static Dictionary<string, DATArchive> khanFiles = new Dictionary<string, DATArchive>();
        internal List<JObject> AutoAscendDataList = new List<JObject>();
        private IntPtr _hWnd;
        public MainForm()
        {
            InitializeComponent();
            LoadKhans();
            ThreadID = Thread.CurrentThread.ManagedThreadId;

            CheckForIllegalCrossThreadCalls = false;
        }

        private void LoadKhans()
        {
            if (khanFiles.Count == 0)
            {
                try
                {
                    string text = Settings.Default.DarkAgesPath.Replace("Darkages.exe", "");
                    //string text = "C:\\Program Files (x86)\\KRU\\Dark Ages";
                    if (!Directory.Exists(text))
                    {
                        throw new Exception();
                    }
                    string[] files = Directory.GetFiles(text, "khan*", SearchOption.TopDirectoryOnly);
                    foreach (string fileName in files)
                    {
                        string key = fileName.Replace(text, "");
                        khanFiles.Add(key, DATArchive.FromFile(fileName));
                    }
                }
                catch
                {
                    MessageDialog.Show(this, "Failed to load .dats, please check Dark Ages path.");
                    return;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Server = new Server(this);

            LoadAutoAscendData();
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                int num1 = (int)MessageDialog.Show(this, "Make sure to run the bot as Admin or you will lose functionality.");
            }
        }

        private void LoadAutoAscendData()
        {
            lock (AutoAscendDataList)
            {
                // 1. Resolve the path and ensure the directory exists
                string autoAscendPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "data",
                    "autoascend"
                );
                Directory.CreateDirectory(autoAscendPath);

                // 2. Grab all JSON files. If none, exit.
                string[] files = Directory.GetFiles(autoAscendPath, "*.json");
                if (files.Length == 0) return;

                // 3. Clear the list before reloading
                AutoAscendDataList.Clear();

                // 4. Prepare the serializer settings once
                var serializerSettings = new JsonSerializerSettings
                {
                    Converters = { new LocationConverter() }
                };

                // 5. Load each file, add to AutoAscendDataList
                foreach (string file in files)
                {
                    try
                    {
                        string content = File.ReadAllText(file);
                        var obj = JsonConvert.DeserializeObject<JObject>(content, serializerSettings);
                        AutoAscendDataList.Add(obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading data from {file}: {ex.Message}");
                    }
                }
            }
        }

        internal void CheckBelt(Client client)
        {
            client.IsCheckingBelt = true;
            client.UseSkill("Sense");
            while (client.IsCheckingBelt)
            {
                Thread.Sleep(5);
            }
        }
        internal void RemoveClientTab(Client client)
        {
            if (InvokeRequired) { Invoke(new Action(() => { RemoveClientTab(client); })); return; }

            if (_clientTabs.ContainsKey(client))
            {
                clientTabControl.Controls.Remove(_clientTabs[client]);
                _clientTabs.Remove(client);
                client.ClientTab.Dispose();
                client.Remove();
            }

        }

        internal void AddClientTab(Client client)
        {
            if (InvokeRequired) { Invoke(new Action(() => { AddClientTab(client); })); return; }

            ClientTab clientTab = new ClientTab(client)
            {
                Dock = DockStyle.Fill
            };

            TabPage tabPage = new TabPage(client.Name);
            tabPage.Controls.Add(clientTab);
            clientTabControl.TabPages.Add(tabPage);

            _clientTabs.Add(client, tabPage);

            foreach (Client otherClient in client.Server.ClientList)
            {
                Bot bot = otherClient.Bot;
                if (bot != null && bot.AllyPage != null && otherClient != client)
                {
                    if (bot.IsAllyAlreadyListed(client.Name))
                    {
                        otherClient.ClientTab.aislingTabControl.TabPages[client.Name]?.Dispose();
                        bot.RemoveAlly(client.Name);
                    }

                    Ally ally = new Ally(client.Name)
                    {
                        AllyPage = bot.AllyPage
                    };
                    bot.AddAlly(ally);
                }
            }
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
                ProcessAccessFlags.VmOperation | ProcessAccessFlags.VmRead | ProcessAccessFlags.VmWrite))
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
            try
            {
                string dawnd = Settings.Default.DarkAgesPath.Replace("Darkages.exe", "dawnd.dll");
                if (!File.Exists(dawnd))
                    File.WriteAllBytes(dawnd, Resources.dawnd);
            }
            catch
            {
                int num = (int)MessageDialog.Show(this, "Failed to write dawnd.dll, please check Dark Ages path.");
                return;
            }
            if (khanFiles.Count == 0)
            {
                try
                {
                    string str = Properties.Settings.Default.DarkAgesPath.Replace("Darkages.exe", "");
                    foreach (string file in Directory.GetFiles(str, "khan*", 0))
                        khanFiles.Add(file.Replace(str, ""), DATArchive.FromFile(file));
                }
                catch
                {
                    int num = (int)MessageDialog.Show(this, "Failed to load .dats, please check Dark Ages path.");
                    return;
                }
            }
            ThreadPool.QueueUserWorkItem(z => LaunchDarkages());

        }

        private void mapCacheMenuItem_Click(object sender, EventArgs e)
        {
            if (!NativeMethods.IsWindow(_hWnd))
            {
                using (MemoryStream memoryStream = new MemoryStream(Resources.MapsCacheEditor))
                {
                    MapsCacheViewer mapsCacheViewer = (MapsCacheViewer)(object)new MapsCacheViewer(memoryStream);
                    _hWnd = ((Control)(object)mapsCacheViewer).Handle;
                    if (mapsCacheViewer.InvokeRequired)
                    {
                        mapsCacheViewer.Invoke(new MethodInvoker(delegate
                        {
                            ((Form)(object)mapsCacheViewer).ShowDialog();
                        }));
                    }
                    else
                    {
                        ((Form)(object)mapsCacheViewer).ShowDialog();
                    }
                }
            }
        }
    }
}


