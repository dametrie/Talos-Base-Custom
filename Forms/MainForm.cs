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
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using Newtonsoft.Json;
using Talos.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using Talos.Options;
using Talos.Structs;
using System.Text;
using Talos.Objects;
using Talos.Definitions;
using System.Xml.Linq;


namespace Talos
{
    public partial class MainForm : Form
    {

        internal Server Server { get; private set; }
        internal int ThreadID { get; set; }

        internal Dictionary<int, int> SpriteOverrides = new Dictionary<int, int>();

        internal System.Windows.Forms.Timer killTimer = new System.Windows.Forms.Timer();
        internal int hours;
        internal int minutes;
        internal int seconds;
        private Rect _daClient;
        private Rect _daWindow;
        private int _titleHeight;
        private int _borderWidth;

        private Dictionary<Client, TabPage> _clientTabs = new Dictionary<Client, TabPage>();
        internal static Dictionary<string, DATArchive> khanFiles = new Dictionary<string, DATArchive>();
        internal List<JObject> AutoAscendDataList = new List<JObject>();
        private IntPtr _hWnd;
        private List<Keys> _mods = new List<Keys>
        {
            Keys.Alt,
            Keys.Control,
            Keys.Shift,
            Keys.LWin,
            Keys.RWin
        };

        public MainForm()
        {
            InitializeComponent();
            LoadKhans();
            Application.EnableVisualStyles();
            GenerateDefaultHotKeys();

            ThreadID = Thread.CurrentThread.ManagedThreadId;
            CheckForIllegalCrossThreadCalls = false;

            Opacity = Settings.Default.BotOpacity / 100.0;
        }

        private void killTimer_Tick(object sender, EventArgs e)
        {
            if (hours == 0 && minutes == 0 && seconds == 0)
            {
                killTimer.Stop();
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (seconds == 0)
            {
                seconds = 59;
                if (minutes == 0)
                {
                    minutes = 59;
                    if (hours > 0)
                        hours--;
                }
                else
                {
                    minutes--;
                }
            }
            else
            {
                seconds--;
            }

            Text = $"Talos - {hours}:{minutes:D2}:{seconds:D2} time left until shutdown!";
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

            LoadCharLoginData();
            LoadAutoAscendData();

            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                int num1 = (int)MessageDialog.Show(this, "Make sure to run the bot as Admin or you will lose functionality.");

                Application.Exit();
            }
        }

        internal void LoadCharLoginData()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "loginCon");

            if (!File.Exists(filePath))
                return;

            try
            {
                var document = XDocument.Load(filePath);

                var characterElement = document.Element("Character");
                if (characterElement != null)
                {
                    foreach (var element in characterElement.Elements())
                    {
                        if (DateTime.TryParse(element.Value, out DateTime loginDate))
                        {
                            Server.ConsecutiveLogin[element.Name.ToString().ToUpper()] = loginDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions appropriately (e.g., corrupted file, invalid XML)
                Console.WriteLine($"Error loading character login data: {ex.Message}");
            }
        }

        internal void SaveLoginCon()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "loginCon");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Create sanitized dictionary for saving
            var sanitizedLoginData = Server.ConsecutiveLogin
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Key) && entry.Value != default)
                .ToDictionary(
                    entry => entry.Key.ToUpper(),  // Normalize keys to uppercase
                    entry => entry.Value          // Ensure valid DateTime values
                );

            // Create the XML document with LINQ-to-XML
            var xDocument = new XDocument(
                new XElement("Character",
                    sanitizedLoginData.Select(item =>
                        new XElement(item.Key, item.Value.ToString("o")) // "o" for round-trip date/time format
                    )
                )
            );

            xDocument.Save(filePath);
        }



        internal void LoadAutoAscendData()
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
                
                //Console.WriteLine($"Number of JSON files found: {files.Length}"); // Log the count

                //foreach (var file in files)
                //{
                //    Console.WriteLine($"Found file: {file}"); // Log each file path
                //}

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
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                {
                    Invoke(new Action(() => { RemoveClientTab(client); }));
                }
                return;
            }

            if (_clientTabs.ContainsKey(client))
            {
                var tab = _clientTabs[client];
                if (tab.IsHandleCreated)
                {
                    clientTabControl.Controls.Remove(tab);
                    _clientTabs.Remove(client);
                    client.ClientTab.Dispose();
                    client.Remove();
                }
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
                if (bot != null && bot.Alts != null && otherClient != client)
                {
                    if (bot.ContainsAlly(client.Name))
                    {
                        otherClient.ClientTab.aislingTabControl.TabPages[client.Name]?.Dispose();
                        bot.RemoveAlly(client.Name);
                    }

                    Ally ally = new Ally(client.Name)
                    {
                        Page = bot.Alts
                    };
                    bot.AddAlly(ally);
                }
            }
        }


        #region Launch Darkages
        internal Process LaunchDarkages()
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
                if (Settings.Default.NoWalls)
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

                //edit the direct ip to the Server loopback ip
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

            // Wait for the main window handle to be assigned
            int retries = 10;
            while (processById.MainWindowHandle == IntPtr.Zero && retries > 0)
            {
                Thread.Sleep(500);
                processById.Refresh(); // Refresh process info to update the MainWindowHandle
                retries--;
            }

            if (processById.MainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to get the MainWindowHandle for the process.");
            }

            NativeMethods.GetClientRect(processById.MainWindowHandle, ref _daClient);
            NativeMethods.GetWindowRect(processById.MainWindowHandle, ref _daWindow);

            _titleHeight = _daWindow.Height - _daClient.Height;
            _borderWidth = _daWindow.Width - _daClient.Width;

            if(Settings.Default.useDawnd && !Settings.Default.SmallWindowOpt)
            {
                if (Settings.Default.LargeWindowOpt)
                    NativeMethods.MoveWindow(processById.MainWindowHandle, _daWindow.X, _daWindow.Y, 1280 + _borderWidth, 960 + _titleHeight, true);
                else if (Settings.Default.FullWindowOpt)
                {
                    NativeMethods.SetWindowLong(processById.MainWindowHandle, -16, 268435456); // WS_POPUP
                    NativeMethods.ShowWindowAsync(processById.MainWindowHandle, 3); // SW_MAXIMIZE
                }
            }
            NativeMethods.SetWindowLong(processById.MainWindowHandle, -20, NativeMethods.GetWindowLong(processById.MainWindowHandle, -20) | 524288);
            NativeMethods.SetLayeredWindowAttributes(processById.MainWindowHandle, 0U, (byte)Math.Truncate(byte.MaxValue / (100.0 / Settings.Default.DAOpacity)), 2U);
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

        internal void UnregisterAllHotkeys()
        {
            for (int id = 1; id <= 5; ++id)
                NativeMethods.UnregisterHotKey(Handle, id);
            for (int id = 9; id <= 12; ++id)
                NativeMethods.UnregisterHotKey(Handle, id);
        }

        internal void GenerateDefaultHotKeys()
        {
            var e = new KeyEventArgs(Keys.None);

            // Register global hotkey for refresh if enabled
            if (Properties.Settings.Default.RefreshAll)
            {
                try
                {
                    NativeMethods.RegisterHotKey(Handle, 1, 0U, (uint)Keys.F5.GetHashCode());
                }
                catch
                {
                    // Handle exception or log if needed
                }
            }

            // Define hotkeys and their corresponding names
            var hotKeys = new Dictionary<string, string>
            {
                { "toggleBot", Settings.Default.BotHotKey },
                { "toggleWalking", Settings.Default.WalkHotKey },
                { "toggleCasting", Settings.Default.CastHotKey },
                { "toggleSound", Settings.Default.SoundHotKey },
                { "combo1", Settings.Default.Combo1HotKey },
                { "combo2", Settings.Default.Combo2HotKey },
                { "combo3", Settings.Default.Combo3HotKey },
                { "combo4", Settings.Default.Combo4HotKey }
            };

            // Loop through the dictionary to register each hotkey
            foreach (var hotKey in hotKeys)
            {
                var textBox = new TextBox
                {
                    Text = hotKey.Value,
                    Name = hotKey.Key
                };

                AddHotKey(textBox, e);
            }
        }

        internal void AddHotKey(object sender, KeyEventArgs e)
        {
            try
            {
                var keysConverter = new KeysConverter();
                if (sender is not TextBox textBox || string.IsNullOrEmpty(textBox.Text))
                    return;

                var keyList = new List<Keys>();

                // Parse hotkey string into individual keys
                var match = Regex.Match(textBox.Text, @"([a-zA-Z0-9]+)(?: ([a-zA-Z0-9]+))?(?: ([a-zA-Z0-9]+))?(?: ([a-zA-Z0-9]+))?");
                foreach (Group group in match.Groups)
                {
                    if (group != match.Groups[0] && group.Success)
                    {
                        keyList.Add((Keys)keysConverter.ConvertFromString(group.Value));
                    }
                }

                // Check if the last key is already in use
                if (_mods.Contains(keyList.Last()))
                {
                    textBox.Text = string.Empty;
                    return;
                }

                // Map control names to their respective hotkey IDs
                var hotKeyMappings = new Dictionary<string, int>
                {
                    { "toggleBot", 2 },
                    { "toggleWalking", 4 },
                    { "toggleSound", 5 },
                    { "toggleCasting", 3 },
                    { "combo1", 9 },
                    { "combo2", 10 },
                    { "combo3", 11 },
                    { "combo4", 12 }
                };

                // Determine the hotkey ID based on the TextBox name
                if (!hotKeyMappings.TryGetValue(textBox.Name, out int hotKeyId))
                    return;

                // Register the hotkey based on the number of keys
                try
                {
                    uint modifiers = 0u;
                    uint mainKey = 0u;

                    if (keyList.Count > 0)
                    {
                        mainKey = (uint)keyList.Last(); // Last key is the main hotkey
                        for (int i = 0; i < keyList.Count - 1; i++) // Other keys are modifiers
                        {
                            modifiers |= KeyboardUtility.GetKeyModifierMask(keyList[i]);
                        }

                        NativeMethods.RegisterHotKey(Handle, hotKeyId, modifiers, mainKey);
                    }
                }
                catch
                {
                    // Handle errors during hotkey registration
                }
            }
            catch
            {
                MessageDialog.Show(this, "Error adding hotkeys!");
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                HandleHotKey(id);
            }

            base.WndProc(ref m); // Pass unhandled messages to the base class
        }

        private void HandleHotKey(int hotKeyId)
        {
            StringBuilder buffer = new StringBuilder(256);
            NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), buffer, 256);

            switch (hotKeyId)
            {
                case 1: // RefreshAll
                    if (Settings.Default.RefreshAll)
                    {
                        foreach (Client client in Server.ClientList)
                        {
                            client.RefreshRequest(false);
                        }
                    }
                    break;
                case 2: // Toggle Bot
                    foreach (Client client in Server.ClientList)
                    {
                        client.ClientTab?.startStrip_Click(new object(), new EventArgs());
                    }
                    break;
                case 3: // Toggle Casting
                    Server._stopCasting = !Server._stopCasting;
                    foreach (Client client in Server.ClientList)
                    {
                        if (client.ClientTab != null)
                        {
                            if (!client.ClientTab.safeScreenCbox.Checked)
                            {
                                client.ServerMessage(1, "Casting: " + (Server._stopCasting ? "OFF" : "ON"));
                            }
                            if (Server._stopCasting)
                            {
                                client.IsCasting = false;
                            }
                        }
                    }
                    break;
                case 4: // Toggle Walking
                    Server._stopWalking = !Server._stopWalking;
                    foreach (Client client in Server.ClientList)
                    {
                        if (client.ClientTab != null)
                        {
                            if (!client.ClientTab.safeScreenCbox.Checked)
                            {
                                client.ServerMessage(1, "Walking: " + (Server._stopWalking ? "OFF" : "ON"));
                            }
                            if (Server._stopWalking)
                            {
                                client.IsWalking = false;
                            }
                        }
                    }
                    break;
                case 5: // Toggle Sound
                    Server._disableSound = !Server._disableSound;
                    foreach (Client client in Server.ClientList)
                    {
                        if (client.ClientTab != null && !client.ClientTab.safeScreenCbox.Checked)
                        {
                            client.ServerMessage(1, "Sound: " + (Server._disableSound ? "OFF" : "ON"));
                        }
                    }
                    break;
                case 9: // Combo1
                case 10: // Combo2
                case 11: // Combo3
                case 12: // Combo4
                    {
                        int comboId = hotKeyId - 8; // Calculate combo index (9 -> 1, 10 -> 2, etc.)
                        Client client = Server.GetClient(buffer.ToString()); // Replace `buffer.ToString()` as needed
                        if (client != null)
                        {
                            ThreadPool.QueueUserWorkItem(_ =>
                            {
                                UseCombo(client, comboId);
                            });
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        internal void UseCombo(Client client, int comboId)
        {
            string[] validSkills = new string[]
            {
                "Assail", "Clobber", "Wallop", "Assault", "Long Strike",
                "Double Punch", "Thrash", "Triple Kick", "Midnight Slash"
            };

            // Check if the combo is enabled
            if (!IsComboEnabled(client, comboId))
                return;

            // Get the combo lines from the TextBox
            string[] comboLines = (client.ClientTab.comboGroup.Controls["combo" + comboId + "List"] as TextBox)?.Lines;
            if (comboLines == null || !client.ClientTab.ComboChecker(comboId))
                return;

            foreach (string line in comboLines)
            {
                string command = line.Trim();

                if (string.IsNullOrEmpty(command))
                    continue;

                // Process commands
                if (client.HasItem(command))
                {
                    client.UseItem(command);
                }
                else if (TryUseItemBySlot(client, command))
                {
                    continue;
                }
                else if (TryUseSkillBySlot(client, command))
                {
                    continue;
                }
                else if (TryCastSpell(client, command))
                {
                    continue;
                }
                else if (client.HasSkill(command) && !command.Equals("Assail", StringComparison.OrdinalIgnoreCase))
                {
                    client.UseSkill(command);
                }
                else if (command.StartsWith("Sleep", StringComparison.OrdinalIgnoreCase))
                {
                    HandleSleepCommand(command);
                }
                else
                {
                    HandleSpecialCommands(client, command, validSkills);
                }
            }
        }

        private bool IsComboEnabled(Client client, int comboId)
        {
            return comboId switch
            {
                1 => client.comboOne,
                2 => client.comboTwo,
                3 => client.comboThree,
                4 => client.comboFour,
                _ => false,
            };
        }

        private bool TryUseItemBySlot(Client client, string command)
        {
            Match match = Regex.Match(command, @"ItemSlot (\d+)", RegexOptions.IgnoreCase);
            if (match.Success && byte.TryParse(match.Groups[1].Value, out byte slot))
            {
                string itemName = client.Inventory[slot]?.Name;
                if (!string.IsNullOrEmpty(itemName))
                {
                    client.UseItem(itemName);
                    return true;
                }
            }
            return false;
        }

        private bool TryUseSkillBySlot(Client client, string command)
        {
            Match match = Regex.Match(command, @"SkillSlot (\d+)", RegexOptions.IgnoreCase);
            if (match.Success && byte.TryParse(match.Groups[1].Value, out byte slot))
            {
                string skillName = client.Skillbook[slot]?.Name;
                if (!string.IsNullOrEmpty(skillName))
                {
                    client.UseSkill(skillName);
                    return true;
                }
            }
            return false;
        }

        private bool TryCastSpell(Client client, string command)
        {
            Match match = Regex.Match(command, @"Cast (?:([\w ]+) on (\w+)|([\w ]+))", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string spellName = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                string targetName = match.Groups[2].Value;

                try
                {
                    if (!string.IsNullOrEmpty(targetName))
                    {
                        Creature target = client.WorldObjects.Values
                            .OfType<Creature>()
                            .FirstOrDefault(creature => string.Equals(creature.Name, targetName, StringComparison.OrdinalIgnoreCase));
                        if (target != null)
                        {
                            client.UseSpell(spellName, target, false, false);
                            return true;
                        }
                    }
                    else
                    {
                        client.UseSpell(spellName, null, false, false);
                        return true;
                    }
                }
                catch
                {
                    // Handle errors gracefully
                }
            }
            return false;
        }

        private void HandleSleepCommand(string command)
        {
            if (int.TryParse(command.Substring(6), out int sleepDuration))
            {
                Thread.Sleep(sleepDuration);
            }
        }

        private void HandleSpecialCommands(Client client, string command, string[] validSkills)
        {
            switch (command)
            {
                case "Assail":
                    foreach (var skill in client.Skillbook.SkillbookDictionary)
                    {
                        if (validSkills.Contains(skill.Key))
                        {
                            client.UseSkill(skill.Key);
                        }
                    }
                    break;
                case "Remove Staff":
                    client.RemoveStaff();
                    break;
                case "Remove Weapon":
                    client.RemoveWeapon();
                    break;
                case "Remove Shield":
                    client.RemoveShield();
                    break;
                case "Free GT":
                    UseGT(client);
                    break;
                case "Check Belt":
                    CheckBelt(client);
                    break;
                case "Use Weakness":
                    EquipBestNeck(client);
                    break;
                default:
                    break;
            }
        }

        internal void UseGT(Client client)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                client.UseSpell("Gentle Touch");
            });
        }

        internal void EquipBestNeck(Client client)
        {
            // Determine which element to use
            Element elementToUse = client.WhichtoUse(client.Server._enemyElement);

            // If the element to use matches the current attack element, do nothing
            if (elementToUse == client.AttackElement)
                return;

            // Dictionary to map elements to their respective necklace
            var necklaceMap = new Dictionary<Element, string>
            {
                { Element.Fire, "Blackstar Fire Necklace" },
                { Element.Water, "Blackstar Sea Necklace" },
                { Element.Wind, "Blackstar Wind Necklace" },
                { Element.Earth, "Blackstar Earth Necklace" },
                { Element.Holy, "Lumen Amulet" },
                { Element.Darkness, "Royal Baem Scale Pendant" }
            };

            // Check for a specific necklace for the element
            if (necklaceMap.TryGetValue(elementToUse, out string necklace))
            {
                client.UseItem(necklace);
            }
            else if (elementToUse == Element.Any && client.AttackElement == Element.None)
            {
                // Equip the default necklace if no specific element and no attack element
                client.UseItem("Royal Baem Scale Pendant");
            }
        }


        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var optionsForm = new Talos.Forms.Options(this))
            {
                optionsForm.ShowDialog();
            }
        }


    }
}


