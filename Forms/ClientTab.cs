using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Talos.Networking;
using Talos.Enumerations;
using Talos.Structs;
using Talos.Objects;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using Talos.Base;
using System.Collections.Generic;
using System.Drawing;
using Talos.Forms.UI;
using Talos.Definitions;
using Talos.Helper;
using Talos.Forms.User_Controls;
using System.Diagnostics;
using Talos.Maps;
using Talos.Capricorn.Drawing;
using Talos.Capricorn.IO;
using Talos.Properties;
using Talos.PInvoke;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;


namespace Talos.Forms
{
    public partial class ClientTab : UserControl
    {
        private ResourceBar healthBar;
        private ResourceBar manaBar;

        private Client _client;
        private bool _isBotRunning = false;
        private string textMaptext = string.Empty;
        private string textXtext = string.Empty;
        private string textYtext = string.Empty;
        private string LastLoadedProfile = string.Empty;
        private short textMap;
        private short textX;
        private short testY;
        internal WayForm WayForm { get; set; } = null;

        internal string _lastWhispered;
        internal int _whispIndex;
        private uint _abilityExp;
        private uint _gold;
        internal ulong _sessionExperience;
        internal uint _sessionAbility;
        internal uint _sessionGold;

        internal bool shouldContinue = true;

        private Stopwatch _sessionExperienceStopWatch = new Stopwatch();
        private Stopwatch _sessionAbilityStopWatch = new Stopwatch();
        private Stopwatch _sessionGoldStopWatch = new Stopwatch();


        internal DateTime _lastUpdate;
        internal DateTime _lastStatusUpdate;

        internal List<string> botProfiles = new List<string>();

        private List<string> _chatPanelList = new List<string> { "", "", "", "", "" };
        internal List<string> _recentWhispers = new List<string> { "", "", "", "", "" };

        internal List<string> _bashingSkillList = new List<string>();
        internal List<string> _unmaxedSkills = new List<string>();
        internal List<string> _unmaxedSpells = new List<string>();

        internal BindingList<string> _trashToDrop = new BindingList<string>
        {
            "fior sal",
            "fior creag",
            "fior srad",
            "fior athar",
            "Purple Potion",
            "Blue Potion",
            "Light Belt",
            "Passion Flower",
            "Gold Jade Necklace",
            "Bone Necklace",
            "Amber Necklace",
            "Half Talisman",
            "Iron Greaves",
            "Goblin Helmet",
            "Cordovan Boots",
            "Shagreen Boots",
            "Magma Boots",
            "Hy-brasyl Bracer",
            "Hy-brasyl Gauntlet",
            "Hy-brasyl Belt",
            "Magus Apollo",
            "Holy Apollo",
            "Magus Diana",
            "Holy Diana",
            "Magus Gaea",
            "Holy Gaea"
        };

        internal BindingList<string> _wayFormProfiles = new BindingList<string>();


        private readonly object _lock = new object();
        private string profilePath;
        internal DATArchive setoaArchive;
        internal EPFImage skillImageArchive;
        internal EPFImage spellImageArchive;
        internal Palette256 iconPalette;

        internal bool _isLoading;
        private string waypointsPath;
        private System.Windows.Forms.Timer mushroomBonusCooldownTimer;

        internal bool IsBashing
        {
            get
            {
                return btnBashingNew.Text == "Stop Bashing";
            }
        }

        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            WayForm = new WayForm(_client);
            UIHelper.Initialize(_client);
            InitializeComponent();
            InitializeCustomResourceBars();

            WayForm.savedWaysLBox.DataSource = _wayFormProfiles;
            worldObjectListBox.DataSource = client.WorldObjectBindingList;
            creatureHashListBox.DataSource = client.CreatureBindingList;
            strangerList.DataSource = client.StrangerBindingList;
            friendList.DataSource = client.FriendBindingList;
            trashList.DataSource = _trashToDrop;

            profilePath = Settings.Default.DarkAgesPath.Replace("Darkages.exe", client.Name + "\\Talos");
            waypointsPath = AppDomain.CurrentDomain.BaseDirectory + "waypoints";
            setoaArchive = DATArchive.FromFile(Settings.Default.DarkAgesPath.Replace("Darkages.exe", "setoa.dat"));
            spellImageArchive = EPFImage.FromArchive("spell001.epf", setoaArchive);
            skillImageArchive = EPFImage.FromArchive("skill001.epf", setoaArchive);
            iconPalette = Palette256.FromArchive("gui06.pal", setoaArchive);

            toggleLogRecvBtn.Checked = Settings.Default.LogOnStartup;
            toggleLogSendBtn.Checked = Settings.Default.LogOnStartup;

            OnlyDisplaySpellsWeHave();
            AddClientToFriends();
            UpdateFriendList();
            SetupInitialClientHacks();
        }

        private void InitializeCustomResourceBars()
        {
            healthBar = new ResourceBar("healthBar")
            {
                BackColor = Color.White,
                ForeColor = Color.Crimson,
                Location = new System.Drawing.Point(6, 282),
                MaximumSize = new Size(50, 196),
                Name = "healthBar",
                Size = new Size(50, 196),
                Style = ProgressBarStyle.Continuous
            };

            manaBar = new ResourceBar("manaBar")
            {
                BackColor = Color.White,
                ForeColor = Color.MidnightBlue,
                Location = new System.Drawing.Point(65, 282),
                MaximumSize = new Size(50, 196),
                Name = "manaBar",
                Size = new Size(50, 196),
                Style = ProgressBarStyle.Continuous
            };

            mainCoverTab.Controls.Add(healthBar);
            mainCoverTab.Controls.Add(manaBar);
        }

        private void ClientTab_Load(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke((Action)delegate { ClientTab_Load(sender, e); }); return; }

            ThreadPool.QueueUserWorkItem(delegate
            {
                _client.CheckNetStat();
            });


            _client.SpellTimer.Start();

            WayForm.DesktopLocation = Location;
            if (_client.Server.MedWalk.ContainsKey(_client.Name) && _client.Server.MedWalk.ContainsKey(_client.Name))
            {
                walkMapCombox.Text = _client.Server.MedWalk[_client.Name];
                walkSpeedSldr.Value = _client.Server.MedWalkSpeed[_client.Name];
                walkSpeedSldr_Scroll(_client.ClientTab.walkSpeedSldr, new EventArgs());
                walkBtn.Text = "Stop";
                startStrip_Click(new object(), new EventArgs());
                _client.Server.MedWalk.Remove(_client.Name);
                _client.Server.MedWalkSpeed.Remove(_client.Name);
            }
            else if (_client.Server.MedTask.ContainsKey(_client.Name) && _client.Server.MedWalkSpeed.ContainsKey(_client.Name))
            {
                if (_client.Server.MedTask[_client.Name] == "bugEvent")
                {
                    toggleBugBtn.Text = "Disable";
                }
                if (_client.Server.MedTask[_client.Name] == "vDayEvent")
                {
                    toggleSeaonalDblBtn.Text = "Disable";
                }
                else
                {
                    followText.Text = _client.Server.MedTask[_client.Name];
                    followCbox.Checked = true;
                }
                walkSpeedSldr.Value = _client.Server.MedWalkSpeed[_client.Name];
                walkSpeedSldr_Scroll(_client.ClientTab.walkSpeedSldr, new EventArgs());
                startStrip_Click(new object(), new EventArgs());
                _client.Server.MedTask.Remove(_client.Name);
                _client.Server.MedWalkSpeed.Remove(_client.Name);
            }

            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }
            if (!Directory.Exists(waypointsPath))
            {
                Directory.CreateDirectory(waypointsPath);
            }

        }

        internal void HandleFiles()
        {
            string text = AppDomain.CurrentDomain.BaseDirectory + "\\inventory\\" + _client.Name.ToLower();
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            else
            {
                try
                {
                    File.WriteAllLines(text + "\\inventory.txt", _client.Inventory.Select(item => $"{item.Name}: {item.Quantity}"));
                }
                catch
                {
                }
            }
            if (profilePath == null)
            {
                Thread.Sleep(100);
            }
            foreach (string item in Directory.GetFiles(profilePath, "*.xml").Select(Path.GetFileNameWithoutExtension))
            {
                List<string> list = botProfiles;
                if (list != null && !list.Contains(item))
                {
                    botProfiles.Add(item);
                }
            }
            foreach (string itemToAdd in Directory.GetFiles(waypointsPath).Select(Path.GetFileName))
            {
                BindingList<string> bindingList = _wayFormProfiles;
                if (bindingList != null && !bindingList.Contains(itemToAdd))
                {
                    UpdateBindingList(_wayFormProfiles, WayForm.savedWaysLBox, itemToAdd);
                }
            }
        }

        private void SaveFriendList()
        {
            // Get the path to the friend list file
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            string filePath = Path.Combine(dataDirectory, "FriendList.xml");

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(dataDirectory);

            // Setup XML writer settings
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };

            try
            {
                using (FileStream fileStream = File.Create(filePath))
                using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Friends");


                    foreach (string friend in _client.FriendBindingList)
                    {
                        writer.WriteElementString("friend", friend);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving friend list: {ex.Message}");
            }
        }
        internal void UpdateFriendList()
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "FriendList.xml");

            if (!File.Exists(dataPath))
                return;

            // Example regex: Matches one or more word characters (letters, digits, underscore)
            // Adjust this pattern to suit your exact needs.
            Regex wordPattern = new Regex(@"^\w+$", RegexOptions.Compiled);

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(dataPath))
                {
                    xmlReader.MoveToContent();
                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name == "friend" && xmlReader.Read())
                        {
                            string friendName = xmlReader.Value;

                            // Check that friendName matches the regex and that a case-insensitive match does not already exist
                            if (wordPattern.IsMatch(friendName) &&
                                !_client.FriendBindingList.Any(f => f.Equals(friendName, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                UpdateBindingList(_client.FriendBindingList, friendList, friendName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating friend list: {ex.Message}");
            }
        }

        private void AddClientToFriends()
        {
            if (!_client.FriendBindingList.Contains(_client.Name))
            {
                UpdateBindingList(_client.FriendBindingList, friendList, _client.Name);
                _client.StrangerBindingList.Remove(_client.Name);
            }
        }

        private void OnlyDisplaySpellsWeHave()
        {
            UIHelper.SetupComboBox(healCombox, new[] { "Leigheas", "nuadhaich", "ard ioc", "mor ioc", "ioc", "beag ioc", "Cold Blood", "Spirit Essence" }, healCbox, healPctNum);
            UIHelper.SetupComboBox(dionCombox, new[] { "mor dion", "Iron Skin", "Wings of Protection", "Draco Stance", "dion", "Stone Skin", "Glowing Stone" }, dionCbox, dionPctNum, dionWhenCombox, aoSithCbox);
            UIHelper.SetupComboBox(fasCombox, new[] { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" }, fasCbox);
            UIHelper.SetupComboBox(aiteCombox, new[] { "ard naomh aite", "mor naomh aite", "naomh aite", "beag naomh aite" }, aiteCbox);

            UIHelper.SetupCheckbox(deireasFaileasCbox, "deireas faileas");
            UIHelper.SetupCheckbox(aoSuainCbox, "ao suain", "Leafhopper Chirp");
            UIHelper.SetupCheckbox(aoCurseCbox, "ao beag cradh", "ao cradh", "ao mor cradh", "ao ard cradh");
            UIHelper.SetupCheckbox(aoPoisonCbox, "ao puinsein");
            UIHelper.SetupCheckbox(bubbleBlockCbox, "Bubble Block", "Bubble Shield");
            UIHelper.SetupCheckbox(spamBubbleCbox, "Bubble Block", "Bubble Shield");
            UIHelper.SetupCheckbox(fungusExtractCbox, "Fungus Beetle Extract");
            UIHelper.SetupCheckbox(mantidScentCbox, "Mantid Scent", "Potent Mantid Scent");
        }


        internal void SetClassSpecificSpells()
        {
            SetVisibilityBasedOnMedeniaClass();
            SetVisibilityBasedOnTemuairClass();
            SetVisibilityBasedOnPreviousClass();
        }

        private void SetVisibilityBasedOnMedeniaClass()
        {
            string medeniaClass = _client.MedeniaClassFlag.ToString();

            switch (medeniaClass)
            {
                case "Archer":
                    SetControlVisibility(nerveStimulantCbox, "Nerve Stimulant");
                    SetPureClassItems(vanishingElixirCbox, "Vanishing Elixir");
                    break;
                case "Gladiator":
                    SetControlVisibility(muscleStimulantCbox, "Muscle Stimulant");
                    SetPureClassSpells(aegisSphereCbox, "Aegis Sphere");
                    SetPureClassItems(monsterCallCbox, "Monster Call");
                    break;
                case "Summoner":
                    SetControlVisibility(dragonScaleCbox, "Dragon's Scale", false);
                    SetPureClassSpells(new[] { vineyardCbox, disenchanterCbox, manaWardCbox }, new[] { "Lyliac Vineyard", "Disenchanter", "Mana Ward" });
                    SetPureClassItems(dragonsFireCbox, "Dragon's Fire");
                    SetVineyardVisibility();
                    break;
                case "Bard":
                    SetRegenerationVisibility();
                    SetControlVisibility(wakeScrollCbox, "Wake Scroll");
                    break;
                case "Druid":
                    SetDruidFormVisibility();
                    break;
            }
        }

        private void SetVisibilityBasedOnTemuairClass()
        {
            string temuairClass = _client.TemuairClassFlag.ToString();

            switch (temuairClass)
            {
                case "Rogue":
                    SetControlVisibility(hideCbox, "Hide", spellBased: true);
                    break;
                case "Warrior":
                    if (_client.PreviousClassFlag.ToString() == "Pure")
                    {
                        SetPureClassSpells(asgallCbox, "asgall faileas");
                        SetPureClassSkills(perfectDefenseCbox, "Perfect Defense");
                    }
                    break;
                case "Wizard":
                    SetControlVisibility(fasSpioradCbox, "fas spiorad", spellBased: true);
                    fasSpioradCbox.VisibleChanged += (sender, e) =>
                    {
                        fasSpioradText.Visible = fasSpioradCbox.Visible;
                    };
                    break;
                case "Priest":
                    SetControlVisibility(armachdCbox, "armachd", spellBased: true);
                    SetControlVisibility(beagCradhCbox, "beag cradh", spellBased: true);
                    break;
                case "Monk":
                    SetControlVisibility(mistCbox, "Mist", spellBased: true);
                    SetControlVisibility(hideCbox, "White Bat Stance", spellBased: true);
                    break;
            }
        }
        private void SetVisibilityBasedOnPreviousClass()
        {
            string previousClass = _client.PreviousClassFlag.ToString();

            switch (previousClass)
            {
                case "Rogue":
                    SetControlVisibility(hideCbox, "Hide", spellBased: true);
                    break;
                case "Warrior":
                    SetControlVisibility(asgallCbox, "Asgall Faileas", spellBased: true);
                    SetControlVisibility(perfectDefenseCbox, "Perfect Defense", spellBased: true);
                    break;
                case "Wizard":
                    SetControlVisibility(fasSpioradCbox, "fas spiorad", spellBased: true);
                    fasSpioradCbox.VisibleChanged += (sender, e) =>
                    {
                        fasSpioradText.Visible = fasSpioradCbox.Visible;
                    };
                    break;
                case "Priest":
                    SetControlVisibility(armachdCbox, "armachd", spellBased: true);
                    SetControlVisibility(beagCradhCbox, "beag cradh", spellBased: true);
                    break;
                case "Monk":
                    SetControlVisibility(mistCbox, "Mist", spellBased: true);
                    SetControlVisibility(hideCbox, "White Bat Stance", spellBased: true);
                    break;
            }
        }


        private void SetControlVisibility(Control control, string name, bool spellBased = false)
        {
            bool isVisible = spellBased ? _client.Spellbook[name] != null : _client.HasItem(name);
            control.Visible = isVisible;
        }

        private void SetPureClassSkills(Control control, string skillName)
        {
            if (_client.PreviousClassFlag.ToString() == "Pure" && _client.Skillbook[skillName] != null)
                control.Visible = true;
        }
        private void SetPureClassSpells(Control control, string spellName)
        {
            if (_client.PreviousClassFlag.ToString() == "Pure" && _client.Spellbook[spellName] != null)
                control.Visible = true;
        }
        private void SetPureClassItems(Control control, string itemName)
        {
            if (_client.PreviousClassFlag.ToString() == "Pure" && _client.HasItem(itemName))
                control.Visible = true;
        }

        private void SetPureClassSpells(Control[] controls, string[] spellNames)
        {
            if (_client.PreviousClassFlag.ToString() == "Pure")
            {
                for (int i = 0; i < spellNames.Length; i++)
                {
                    if (_client.Spellbook[spellNames[i]] != null)
                        controls[i].Visible = true;
                }
            }
        }


        private void SetRegenerationVisibility()
        {
            if (_client.Spellbook["Regeneration"] != null || _client.Spellbook["Increased Regeneration"] != null)
                regenerationCbox.Visible = true;
        }

        private void SetDruidFormVisibility()
        {
            bool hasSpellWithForm = _client.Spellbook.Any(spell => spell.Name.Contains("Form"));
            if (hasSpellWithForm)
                druidFormCbox.Visible = true;
        }

        private void SetVineyardVisibility()
        {
            if (_client.PreviousClassFlag.ToString() == "Pure" && _client.Spellbook["Lyliac Vineyard"] != null)
            {
                vineyardCbox.Visible = true;
                vineCombox.Visible = true;
                vineText.Visible = true;
            }
        }


        internal void DisplayHPMP()
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { DisplayHPMP(); })); return; }

            hpLbl.Text = _client.Stats.CurrentHP.ToString();
            mpLbl.Text = _client.Stats.CurrentMP.ToString();
            healthBar.Value = (int)_client.HealthPct;
            manaBar.Value = (int)_client.ManaPct;

        }

        internal void RemoveClient()
        {
            _client = null;
        }


        internal void LogPackets(Packet p)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { LogPackets(p); })); return; }

            packetList.Items.Add(p);
        }

        internal void SpellToUse(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.Name;
            if (button.BackColor == Color.DodgerBlue)
            {
                button.BackColor = Color.White;
                _unmaxedSpells.Remove(name);
            }
            else
            {
                button.BackColor = Color.DodgerBlue;
                _unmaxedSpells.Add(name);
            }
        }

        internal void SkillToUse(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.Name;
            if (button.BackColor == Color.DodgerBlue)
            {
                button.BackColor = Color.White;
                _unmaxedSkills.Remove(name);
            }
            else
            {
                button.BackColor = Color.DodgerBlue;
                _unmaxedSkills.Add(name);
            }
        }

        internal void RenderUnmaxedSkills(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(skillImageArchive[index], iconPalette);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name,
                Name = name + "whatever"
            };
            textBox.Width = (int)(TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
            textBox.Location = new System.Drawing.Point(point.X + 25, point.Y + 25);
            button.Location = new System.Drawing.Point(point.X, point.Y);
            button.FlatStyle = FlatStyle.Flat;
            button.Size = new Size(40, 40);
            button.Image = image;
            button.BackColor = Color.White;
            button.Name = name;
            button.Padding = Padding.Empty;
            button.Margin = Padding.Empty;
            button.MouseEnter += ShowAndBringToFrontTextBox;
            button.MouseLeave += HideTextBox;
            button.Click += SkillToUse;
            unmaxedSkillsGroup.Controls.Add(button);
            unmaxedSkillsGroup.Controls.Add(textBox);
        }
        internal void RenderUnmaxedSpells(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(spellImageArchive[index], iconPalette);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name
            };
            textBox.Width = (int)(TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
            textBox.Name = name + "whatever";
            textBox.Location = new System.Drawing.Point(point.X + 25, point.Y + 25);
            button.Location = new System.Drawing.Point(point.X, point.Y);
            button.FlatStyle = FlatStyle.Flat;
            button.Size = new Size(40, 40);
            button.Image = image;
            button.Name = name;
            button.Padding = Padding.Empty;
            button.Margin = Padding.Empty;
            button.MouseEnter += ShowAndBringToFrontTextBox;
            button.MouseLeave += HideTextBox;
            button.Click += SpellToUse;
            unmaxedSpellsGroup.Controls.Add(button);
            unmaxedSpellsGroup.Controls.Add(textBox);
        }
        internal void RenderBashingSkills(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(skillImageArchive[index], iconPalette);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name,
                Name = name + "whatever"
            };
            textBox.Width = (int)(TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
            textBox.Location = new System.Drawing.Point(point.X + 25, point.Y + 25);
            button.Location = new System.Drawing.Point(point.X, point.Y);
            button.FlatStyle = FlatStyle.Flat;
            button.Size = new Size(40, 40);
            button.Image = image;
            button.BackColor = Color.White;
            button.Name = name;
            button.Padding = Padding.Empty;
            button.Margin = Padding.Empty;
            button.MouseEnter += ShowAndBringToFrontTextBox;
            button.MouseLeave += HideTextBox;
            button.Click += BashSkillToUse;
            bashingSkillsToUseGrp.Controls.Add(button);
            bashingSkillsToUseGrp.Controls.Add(textBox);
        }
        internal void BashSkillToUse(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.Name;
            if (button.BackColor == Color.DodgerBlue)
            {
                button.BackColor = Color.White;
                _bashingSkillList.Remove(name);
            }
            else
            {
                button.BackColor = Color.DodgerBlue;
                _bashingSkillList.Add(name);
            }
        }


        private void HideTextBox(object sender, EventArgs e)
        {
            Button button = sender as Button;
            TextBox textBox = button.Parent.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == button.Name + "whatever");
            textBox?.Hide();
        }

        private void ShowAndBringToFrontTextBox(object sender, EventArgs e)
        {
            Button button = sender as Button;
            TextBox textBox = button.Parent.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == button.Name + "whatever");
            textBox?.Show();
            textBox?.BringToFront();
        }


        internal void UpdateBindingList(BindingList<string> bindingList, ListBox listBox, string name)
        {
            if (listBox == null || string.IsNullOrEmpty(name))
            {
                return;
            }

            // Check if we're on the UI thread
            if (listBox.InvokeRequired)
            {
                listBox.Invoke(new Action(() => UpdateBindingList(bindingList, listBox, name)));
            }
            else
            {
                if (bindingList.Count != 0)
                {
                    bindingList.Add(name);
                }
                else
                {
                    bindingList.Add(name);
                    listBox.DataSource = null;
                    listBox.DataSource = bindingList;
                }
            }
        }

        internal void AddMessageToChatPanel(System.Drawing.Color color, string message)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    AddMessageToChatPanel(color, message);
                });
                return;
            }
            if (chatPanel2.TextLength != 0)
            {
                message = Environment.NewLine + message;
            }
            MatchCollection matchCollection;
            if ((matchCollection = Regex.Matches(message, "({=[a-zA-Z])")).Count > 0)
            {
                foreach (Match item in matchCollection)
                {
                    if (item.Success)
                    {
                        foreach (Group group in item.Groups)
                        {
                            message = message.Replace(group.Value, "");
                        }
                    }
                }
            }
            _ = chatPanel2.Text.Length;
            bool atBottom = chatPanel2.IsScrollBarAtBottom();
            SetChatPanelTextColor(color);
            if (!string.IsNullOrWhiteSpace(message))
            {
                chatPanel2.AppendText(message);
            }
            SetChatPanelTextColor(chatPanel2.ForeColor);
            if (atBottom)
            {
                chatPanel2.ScrollToCaret();
            }
        }

        internal void UpdateChatPanelMaxLength(Client client)
        {
            chatPanel2.MaxLength = 65 - client.Name.Length;
        }
        internal void UpdateChatPanel(string input)
        {
            var match = Regex.Match(input, "(\\[!|<!|[a-zA-Z]+)");
            if (!match.Success)
                return;

            string str = match.Groups[1].Value;
            if (str == "[!")
                str = "!!";
            if (str == "<!")
                str = "!";

            // Remove existing occurrence if any
            if (_recentWhispers.Contains(str))
            {
                _recentWhispers.Remove(str);
            }

            // Insert at the front
            _recentWhispers.Insert(0, str);

            // Ensure the list doesn't grow beyond 5 items
            if (_recentWhispers.Count > 5)
            {
                _recentWhispers
                    .RemoveAt(5);
            }
        }

        private void ShiftListItemsDown(int endIndex)
        {
            for (int i = endIndex; i > 0; i--)
            {
                _chatPanelList[i] = _chatPanelList[i - 1];
            }
        }

        internal void SetChatPanelTextColor(System.Drawing.Color color)
        {
            chatPanel2.SelectionStart = chatPanel2.TextLength;
            chatPanel2.SelectionLength = 0;
            chatPanel2.SelectionColor = color;
        }
        private void unifiedGuildChatCbox_CheckedChanged(object sender, EventArgs e)
        {
            _client.UnifiedGuildChat = unifiedGuildChatCbox.Checked;
        }


        private void effectBtn_Click(object sender, EventArgs e)
        {
            ServerPacket serverPacket = new ServerPacket(41);
            serverPacket.WriteUInt32(_client.PlayerID);
            serverPacket.WriteUInt32(_client.PlayerID);
            serverPacket.WriteUInt16((ushort)effectNum.Value);
            serverPacket.WriteUInt16((ushort)effectNum.Value);
            serverPacket.WriteUInt16(90);
            _client.Enqueue(serverPacket);
        }

        internal void LogPackets(ClientPacket clientPacket)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { LogPackets(clientPacket); })); return; }

            if (toggleLogSendBtn.Checked)
                packetList.Items.Add(clientPacket.Copy());
        }

        internal void LogPackets(ServerPacket serverPacket)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { LogPackets(serverPacket); })); return; }

            if (toggleLogRecvBtn.Checked)
                packetList.Items.Add(serverPacket.Copy());
        }

        private void toggleDialogBtn_Click(object sender, EventArgs e)
        {
            _client.BlockDialogs = toggleDialogBtn.Checked;
        }
        private void packetList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && packetList.SelectedItem != null)
            {
                Clipboard.SetText(((Packet)packetList.SelectedItem).GetHexString());
            }
        }

        private void packetList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && packetList.SelectedItem != null)
            {
                Clipboard.SetText(((Packet)packetList.SelectedItem).GetHexString());
            }
        }

        private void packetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packetList.SelectedItem == null)
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            Packet packet = packetList.SelectedItem as Packet;
            packetHexText.Text = packet.GetHexString();
            string text = packet.GetAsciiString();
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i % 39 == 0)
                {
                    stringBuilder.AppendLine();
                }
                stringBuilder.Append(text[i]);
            }
            packetCharText.Text = stringBuilder.ToString();
        }

        private void sendPacketToServerBtn_Click(object sender, EventArgs e)
        {
            string[] lines = packetInputText.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                string text = Regex.Replace(lines[i], "\"(.*?)\"([012]?)", delegate (Match match_0)
                {
                    int prefixLength = 1;
                    if (!string.IsNullOrEmpty(match_0.Groups[2].Value))
                    {
                        prefixLength = int.Parse(match_0.Groups[2].Value);
                    }
                    byte[] contentBytes = Encoding.GetEncoding(949).GetBytes(match_0.Groups[1].Value.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t"));
                    if (prefixLength != 0)
                    {
                        int contentLength = contentBytes.Length;
                        Array.Resize(ref contentBytes, contentLength + prefixLength);
                        Buffer.BlockCopy(contentBytes, 0, contentBytes, prefixLength, contentLength);
                        if (prefixLength == 1)
                        {
                            contentBytes[0] = (byte)contentLength;
                        }
                        else
                        {
                            contentBytes[0] = (byte)(contentLength >> 8);
                            contentBytes[1] = (byte)contentLength;
                        }
                    }
                    return BitConverter.ToString(contentBytes);
                }).Replace("-", string.Empty).Replace(" ", string.Empty);
                if (text.Length < 2 || text.Length % 2 != 0 || !Regex.IsMatch(text, "^[a-f0-9]+$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                ClientPacket clientPacket = new ClientPacket(byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber));
                if (text.Length > 2)
                {
                    int num = (text.Length - 2) / 2;
                    byte[] toSend = new byte[num];
                    for (int j = 0; j < num; j++)
                    {
                        int startIndex = 2 + j * 2;
                        toSend[j] = byte.Parse(text.Substring(startIndex, 2), NumberStyles.HexNumber);
                    }
                    clientPacket.Write(toSend);
                }
                _client.Enqueue(clientPacket);
            }
        }

        private void sendPacketToClientBtn_Click(object sender, EventArgs e)
        {
            string[] lines = packetInputText.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                string text = Regex.Replace(lines[i], "\"(.*?)\"([012]?)", delegate (Match match_0)
                {
                    int prefixLength = 1;
                    if (!string.IsNullOrEmpty(match_0.Groups[2].Value))
                    {
                        prefixLength = int.Parse(match_0.Groups[2].Value);
                    }
                    byte[] contentBytes = Encoding.GetEncoding(949).GetBytes(match_0.Groups[1].Value.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t"));
                    if (prefixLength != 0)
                    {
                        int contentLength = contentBytes.Length;
                        Array.Resize(ref contentBytes, contentLength + prefixLength);
                        Buffer.BlockCopy(contentBytes, 0, contentBytes, prefixLength, contentLength);
                        if (prefixLength == 1)
                        {
                            contentBytes[0] = (byte)contentLength;
                        }
                        else
                        {
                            contentBytes[0] = (byte)(contentLength >> 8);
                            contentBytes[1] = (byte)contentLength;
                        }
                    }
                    return BitConverter.ToString(contentBytes);
                }).Replace("-", string.Empty).Replace(" ", string.Empty);
                if (text.Length < 2 || text.Length % 2 != 0 || !Regex.IsMatch(text, "^[a-f0-9]+$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                ServerPacket serverPacket = new ServerPacket(byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber));
                if (text.Length > 2)
                {
                    int num = (text.Length - 2) / 2;
                    byte[] toSend = new byte[num];
                    for (int j = 0; j < num; j++)
                    {
                        int startIndex = 2 + j * 2;
                        toSend[j] = byte.Parse(text.Substring(startIndex, 2), NumberStyles.HexNumber);
                    }
                    serverPacket.Write(toSend);
                }
                _client.Enqueue(serverPacket);
            }
        }

        private void clearPacketLogBtn_Click(object sender, EventArgs e)
        {
            packetList.Items.Clear();
            packetCharText.Clear();
            packetHexText.Clear();
        }


        private void bonusCooldownTimer_Tick(object sender, EventArgs e)
        {
            // Calculate the total elapsed time since the last bonus was applied
            TimeSpan elapsedTimeSinceLastBonus = DateTime.Now - _client.Bot._lastExpBonusAppliedTime;

            // Accumulate the total elapsed time with any previously stored elapsed time
            TimeSpan totalElapsedTime = elapsedTimeSinceLastBonus + _client.Bot._expBonusElapsedTime;

            // Update the label to show the total elapsed time in a readable format
            doublesLbl.Text = $"Time Elapsed: {totalElapsedTime.Hours}h {totalElapsedTime.Minutes}m {totalElapsedTime.Seconds}s";
        }

        internal void UpdateNpcInfo(string npcDialog, ushort dialogID, ushort pursuitID)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UpdateNpcInfo(npcDialog, dialogID, pursuitID))); return; }

            npcText.Text = npcDialog;
            dialogIdLbl.Text = "Dialog ID: " + dialogID.ToString();
            pursuitLbl.Text = "Pursuit ID: " + pursuitID.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.North);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.East);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.West);
        }


        private void button5_Click(object sender, EventArgs e)
        {


        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            //take in contents of textbox and store to variable of short type
            textMaptext = textBox6.Text;
            short.TryParse(textMaptext, out textMap);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textXtext = textBox5.Text;
            short.TryParse(textXtext, out textX);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textYtext = textBox4.Text;
            short.TryParse(textYtext, out testY);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (_client.IsRefreshingData == 1)
            {
                button7.Text = "Refresh";
                _client.IsRefreshingData = 0;
            }
            else
            {
                button7.Text = "Stop Refreshing";
                _client.IsRefreshingData = 1;
            }
        }

        private void formCbox_CheckedChanged(object sender, EventArgs e)
        {
            _client.SpriteOverrideEnabled = (sender as CheckBox).Checked;
            _client.DisplayAisling(_client.Player);
        }
        private void formNum_ValueChanged(object sender, EventArgs e)
        {
            _client.SpriteOverride = (ushort)(sender as NumericUpDown).Value;
            if (formCbox.Checked)
            {
                _client.DisplayAisling(_client.Player);
            }
        }

        private void deformCbox_CheckedChanged(object sender, EventArgs e)
        {

            if (deformCbox.Checked)
                _client.DeformNearStrangers = true;
            else
                _client.DeformNearStrangers = false;

            _client.DisplayAisling(_client.Player);
        }

        internal void DisplayObjectIDs(WorldObject worldObject)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { DisplayObjectIDs(worldObject); })); return; }

            lastClickedIDLbl.Text = "ID: " + worldObject.ID;
            lastClickedNameLbl.Text = (worldObject.Name ?? " ");


            if (worldObject is VisibleObject)
            {
                lastClickedSpriteLbl.Text = "Sprite: " + (worldObject as VisibleObject).SpriteID;

            }
        }

        internal void DisplayMapInfoOnCover(Map map)
        {
            mapInfoInfoLbl.Text = "Pt:" + _client.ServerLocation.Point.ToString() + "  Size: " + map.Width + "x" + map.Height + "  ID: " + map.MapID;
        }


        internal void SetMonsterForm(bool isChecked, ushort monsterID)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { SetMonsterForm(isChecked, monsterID); })); return; }

            formCbox.Checked = isChecked;
            formNum.Value = monsterID;
        }

        internal void ClearNearbyEnemies()
        {
            DateTime startTime = DateTime.UtcNow;
            List<Control> controlsToDispose = new List<Control>();

            for (int i = 0; i < nearbyEnemyTable.Controls.Count; i++)
            {
                NearbyEnemy enemy = nearbyEnemyTable.Controls[i] as NearbyEnemy;
                if (enemy != null && enemy._isLoaded)
                {
                    controlsToDispose.Add(nearbyEnemyTable.Controls[i]);
                }
            }

            foreach (Control control in controlsToDispose)
            {
                control.Dispose();
            }

            nearbyEnemyTable.Controls.Clear();
        }

        internal void ClearNearbyAllies()
        {
            DateTime startTime = DateTime.UtcNow;
            List<NearbyAlly> alliesToDispose = new List<NearbyAlly>();

            foreach (NearbyAlly ally in nearbyAllyTable.Controls)
            {
                if (ally.isLoaded)
                {
                    alliesToDispose.Add(ally);
                }
            }

            foreach (NearbyAlly ally in alliesToDispose)
            {
                ally.Dispose();
            }

            nearbyAllyTable.Controls.Clear();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string spellName = this.spellName.Text;


            _client.UseSpell(spellName, _client.Player, true, false);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox6 != null && textBox5 != null && textBox4 != null)
            {
                Location targetLocation = new Location(textMap, new Structs.Point(textX, testY));
                //_client.RouteFinder.FindRoute(_client._clientLocation, targetLocation);
                //_client.RouteFind(targetLocation);
                // Start the walking process on a new thread
                Task.Run(() =>
                {
                    while (_client.ClientLocation != targetLocation)
                    {
                        // Safely update the UI or check conditions that involve UI elements

                        Invoke((MethodInvoker)delegate
                        {
                            // Example of checking a condition or updating UI
                            shouldContinue = (_client.ClientLocation != targetLocation);
                        });

                        if (!shouldContinue)
                            break;

                        _client.Routefind(targetLocation);


                        // Optionally add a small delay to prevent tight looping
                        Thread.Sleep(100);
                    }
                });

            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (textBox6 != null && textBox5 != null && textBox4 != null)
            {
                Location targetLocation = new Location(textMap, new Structs.Point(textX, testY));
                _client.Routefind(targetLocation);

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _client.RemoveShield();
        }

        private void walkSpeedSldr_Scroll(object sender, EventArgs e)
        {
            walkSpeedLbl.Text = (sender as TrackBar).Value.ToString();
            _client.WalkSpeed = walkSpeedSldr.Value;
        }

        internal void DelayedUpdateStrangerList()
        {
            Thread.Sleep(1600);
            UpdateStrangerList();
        }

        internal void UpdateStrangerList()
        {
            if (Monitor.TryEnter(_lock, 150))
            {
                string currentMap = _client.Map.Name;
                bool flag = false;

                if (!string.IsNullOrEmpty(currentMap))
                {
                    flag = CONSTANTS.DONT_UPDATE_STRANGERS.Contains(currentMap);
                }

                if (flag)
                {
                    return;
                }

                try
                {
                    bool isChargeSkillUsedRecently = _client.HasSkill("Charge") && (DateTime.UtcNow - _client.Skillbook["Charge"].LastUsed).TotalMilliseconds < 1500.0;
                    bool isSprintPotionUsedRecently = _client.HasItem("Sprint Potion") && (DateTime.UtcNow - _client.Inventory.Find("Sprint Potion").LastUsed).TotalMilliseconds < 1500.0;

                    if (!isChargeSkillUsedRecently && !isSprintPotionUsedRecently)
                    {
                        HashSet<string> nearbyPlayers = new HashSet<string>(_client.GetNearbyPlayers().Select(player => player.Name), StringComparer.CurrentCultureIgnoreCase);
                        HashSet<string> nonStrangers = new HashSet<string>(_client.GroupedPlayers.Concat(_client.FriendBindingList), StringComparer.CurrentCultureIgnoreCase);

                        foreach (string name in new List<string>(_client.StrangerBindingList))
                        {
                            if (nonStrangers.Contains(name, StringComparer.CurrentCultureIgnoreCase) || !nearbyPlayers.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                            {
                                _client.StrangerBindingList.Remove(name);
                            }
                        }
                        foreach (string name in nearbyPlayers)
                        {
                            if (!nonStrangers.Contains(name, StringComparer.CurrentCultureIgnoreCase) && !_client.StrangerBindingList.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                            {
                                UpdateBindingList(_client.StrangerBindingList, strangerList, name);
                            }
                        }
                        foreach (string name in new List<string>(_client.StrangerBindingList))
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                _client.DictLastSeen[name] = DateTime.UtcNow;
                            }
                        }
                        _client.DictLastSeen = _client.DictLastSeen.OrderByDescending((KeyValuePair<string, DateTime> kvp) => kvp.Value).Take(5).ToDictionary((KeyValuePair<string, DateTime> kvp) => kvp.Key, (KeyValuePair<string, DateTime> kvp) => kvp.Value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught trying to update stranger list {ex.ToString()}");
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        private void dojoRefreshBtn_Click(object sender, EventArgs e)
        {
            _client.ClientTab.StopBot();
            try
            {
                _client.ClientTab._isLoading = true;
                _client.Staffs.Clear();
                _client.LoadStavesAndBows();
                _unmaxedSkills.Clear();
                _unmaxedSpells.Clear();
                setoaArchive = DATArchive.FromFile(Settings.Default.DarkAgesPath.Replace("Darkages.exe", "setoa.dat"));
                spellImageArchive = EPFImage.FromArchive("spell001.epf", setoaArchive);
                skillImageArchive = EPFImage.FromArchive("skill001.epf", setoaArchive);
                iconPalette = Palette256.FromArchive("gui06.pal", setoaArchive);
                while (unmaxedSkillsGroup.Controls.Count > 0)
                {
                    unmaxedSkillsGroup.Controls[0].Dispose();
                }
                while (unmaxedSpellsGroup.Controls.Count > 0)
                {
                    unmaxedSpellsGroup.Controls[0].Dispose();
                }
                _client.LoadUnmaxedSkills();
                _client.LoadUnmaxedSpells();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (_client.ClientTab.startStrip.Text == "Stop")
                {
                    _client.BotBase.Start();
                }
                _client.ClientTab._isLoading = false;

                OnlyDisplaySpellsWeHave();
                SetClassSpecificSpells();
            }
        }

        private void addAislingBtn_Click(object sender, EventArgs e)
        {
            string name = addAislingText.Text;
            Console.WriteLine($"[Debug] Adding ally: {name}");
            if (ParseAllyTextBox(name))
            {
                AddAllyPage(name);
                Console.WriteLine($"[Debug] Ally {name} added successfully.");
                addAislingText.Clear();
                if (!_isLoading && MessageDialog.Show(_client.Server.MainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
                {
                    aislingTabControl.SelectTab(aislingTabControl.TabPages.IndexOfKey(name));
                    clientTabControl.SelectTab(1);
                }
            }
        }

        private bool ParseAllyTextBox(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageDialog.Show(_client.Server.MainForm, "Your ally target cannot be empty.");
                return false;
            }
            if (!name.All(char.IsLetter))
            {
                MessageDialog.Show(_client.Server.MainForm, "Your ally target cannot contain noncharacters.");
                return false;
            }
            if (_client.Bot.ContainsAlly(name))
            {
                MessageDialog.Show(_client.Server.MainForm, "Ally already in list.");
                return false;
            }
            if (name.Equals(_client.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageDialog.Show(_client.Server.MainForm, "Cannot add yourself to ally list.");
                return false;
            }
            return true;
        }

        internal void AddAllyPage(string name, Image image = null)
        {
            name = name.Trim().ToLowerInvariant(); // Normalize before using

            if (_client.Bot.ContainsAlly(name))
            {
                return;
            }

            Ally ally = new Ally(name);
            ally.Page = new AllyPage(ally, _client);

            TabPage tabPage = new TabPage(ally.ToString())
            {
                Name = name
            };
            tabPage.Controls.Add(ally.Page);
            ally.Page.allypictureCharacter.Image = image;

            aislingTabControl.TabPages.Add(tabPage);
            UpdateNearbyAllyTable(name);
            RefreshNearbyAllyTable();
            _client.Bot.AddAlly(ally);

        }

        internal void AddEnemyPage(ushort sprite)
        {
            if (!_client.Bot.IsEnemyAlreadyListed(sprite))
            {
                Enemy enemy = new Enemy(sprite);
                enemy.EnemyPage = new EnemyPage(enemy, _client);
                TabPage tabPage = new TabPage(sprite.ToString())
                {
                    Name = sprite.ToString()
                };
                tabPage.Controls.Add(enemy.EnemyPage);
                monsterTabControl.TabPages.Add(tabPage);
                UpdateNearbyEnemyTable(sprite);
                RefreshNearbyEnemyTable();
                _client.Bot.UpdateEnemyList(enemy);
            }
        }

        internal void AddNearbyEnemy(Creature npc)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    AddNearbyEnemy(npc);
                });
            }
            else if (_client.Bot.AllMonsters == null && !_client.Bot._enemyListID.Contains(npc.SpriteID) && !nearbyEnemyTable.Controls.ContainsKey(npc.SpriteID.ToString()))
            {
                NearbyEnemy control = new NearbyEnemy(npc, _client)
                {
                    Name = npc.SpriteID.ToString()
                };
                nearbyEnemyTable.Controls.Add(control, -1, -1);
            }
        }

        internal void AddNearbyAlly(Player player)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    AddNearbyAlly(player);
                });
            }
            else if (!_client.Bot._allyListName.Contains(player.Name) && !nearbyAllyTable.Controls.ContainsKey(player.Name))
            {
                NearbyAlly control = new NearbyAlly(player, _client)
                {
                    Name = player.Name
                };
                nearbyAllyTable.Controls.Add(control, -1, -1);
            }
        }

        internal void UpdateNearbyEnemyTable(ushort sprite)
        {
            // Exit early if the EnemyPage is not null or the control does not exist.
            if (_client.Bot.AllMonsters != null || !nearbyEnemyTable.Controls.ContainsKey(sprite.ToString()))
            {
                return;
            }

            // Define the timeout duration and the interval for checks.
            TimeSpan timeout = TimeSpan.FromMilliseconds(250);
            TimeSpan checkInterval = TimeSpan.FromMilliseconds(25);

            DateTime startTime = DateTime.UtcNow;

            // Try to get the NearbyEnemy control.
            if (nearbyEnemyTable.Controls[sprite.ToString()] is NearbyEnemy nearbyEnemy)
            {
                // Loop until the timeout is reached or the control is loaded.
                while (DateTime.UtcNow - startTime < timeout)
                {
                    if (nearbyEnemy._isLoaded)
                    {
                        // If loaded, dispose the control and exit.
                        nearbyEnemy.Dispose();
                        return;
                    }

                    // Sleep for the check interval before checking again.
                    Thread.Sleep(checkInterval);
                }
            }
        }

        private void RefreshNearbyEnemyTable()
        {
            List<NearbyEnemy> list = nearbyEnemyTable.Controls.OfType<NearbyEnemy>().ToList();
            nearbyEnemyTable.Controls.Clear();
            foreach (NearbyEnemy item in list)
            {
                nearbyEnemyTable.Controls.Add(item, -1, -1);
            }
        }

        internal void UpdateGroupList()
        {
            groupList.DataSource = null;
            _client.GroupBindingList = new BindingList<string>(_client.GroupedPlayers.ToList());
            groupList.DataSource = _client.GroupBindingList;
        }

        internal void UpdateNearbyAllyTable(string name)
        {
            if (!nearbyAllyTable.Controls.ContainsKey(name))
            {
                return;
            }

            Control allyControl = nearbyAllyTable.Controls[name];

            WaitForCondition(allyControl, control => ((NearbyAlly)control).isLoaded);

            allyControl.Dispose();
        }

        private void RefreshNearbyAllyTable()
        {
            List<NearbyAlly> allies = nearbyAllyTable.Controls.OfType<NearbyAlly>().ToList();
            nearbyAllyTable.Controls.Clear();
            foreach (NearbyAlly ally in allies)
            {
                nearbyAllyTable.Controls.Add(ally);
            }
        }

        private void WaitForCondition(Control control, Func<Control, bool> condition)
        {
            DateTime utcNow = DateTime.UtcNow;
            while (true)
            {
                if (DateTime.UtcNow.Subtract(utcNow).TotalMilliseconds < 250.0)
                {
                    if (condition(control))
                    {
                        break;
                    }
                    Thread.Sleep(25);
                    continue;
                }
                return;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.South);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.North);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.East);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            _client.Walk(Direction.West);
        }



        internal async void startStrip_Click(object sender, EventArgs e)
        {
            if (startStrip.Text == "Start")
            {
                startStrip.Text = "Stop";
                if (_client.BotBase != _client.Bot)
                {
                    _client.BotBase = _client.Bot;
                    _client.BotBase.Client = _client;
                    _client.BotBase.Server = _client.Server;
                    _client.CurrentWaypoint = 0;
                }

                // asynchronously to avoid freezing the UI?
                await Task.Run(() =>
                {
                    _client.BotBase.Start();
                });

                if (!_client.ClientTab.safeScreenCbox.Checked)
                {
                    _client.ServerMessage((byte)ServerMessageType.OrangeBar1, "Bot Started");
                }
            }
            else if (startStrip.Text == "Stop")
            {
                startStrip.Text = "Start";

                await Task.Run(() =>
                {
                    _client.BotBase.Stop();
                });

                if (!_client.ClientTab.safeScreenCbox.Checked)
                {
                    _client.ServerMessage((byte)ServerMessageType.OrangeBar1, "Bot Stopped");
                }
                _client.IsWalking = false;
                _client.IsCasting = false;
                _client.Bot._dontWalk = false;
                _client.Bot._dontCast = false;
                _client.ExchangeOpen = false;
            }
        }


        private void lastSeenBtn_Click(object sender, EventArgs e)
        {
            _client.ServerMessage((byte)ServerMessageType.Whisper, "- Last 5 strangers sighted -");

            // Take only the top 5 most recently seen strangers
            var lastSeenStrangers = _client.DictLastSeen
                .OrderByDescending(kvp => kvp.Value)
                .Take(5);

            foreach (var item in lastSeenStrangers)
            {
                string message = $"{item.Key}: {item.Value.ToLocalTime():t}";
                _client.ServerMessage((byte)ServerMessageType.Whisper, message);
            }
        }

        private void friendStrangerBtn_Click(object sender, EventArgs e)
        {
            foreach (string selectedItem in strangerList.SelectedItems.OfType<string>().ToList())
            {
                UpdateBindingList(_client.FriendBindingList, friendList, selectedItem);
                _client.StrangerBindingList.Remove(selectedItem);
            }

            SaveFriendList();
            UpdateFriendList();

        }

        private void removeFriendBtn_Click(object sender, EventArgs e)
        {
            List<Player> source = _client.GetNearbyPlayers();

            foreach (string seletedItem in friendList.SelectedItems.OfType<string>().ToList())
            {
                if (source.Select((Player player) => player.Name).Contains(seletedItem, StringComparer.CurrentCultureIgnoreCase))
                {
                    UpdateBindingList(_client.StrangerBindingList, strangerList, seletedItem);
                }

                _client.FriendBindingList.Remove(seletedItem);

                UpdateStrangerList();
                SaveFriendList();
                UpdateFriendList();
            }
        }

        private void groupStrangerBtn_Click(object sender, EventArgs e)
        {
            foreach (string selectedItem in strangerList.SelectedItems.OfType<string>().ToList())
            {
                _client.RequestGroup(selectedItem);
            }
        }

        private void groupFriendBtn_Click(object sender, EventArgs e)
        {
            foreach (string selectedItem in friendList.SelectedItems.OfType<string>().ToList())
            {
                _client.RequestGroup(selectedItem);
            }
        }

        private void groupAltsBtn_Click(object sender, EventArgs e)
        {
            List<Client> clientListCopy;
            lock (_client.Server._clientListLock)
            {
                clientListCopy = _client.Server.ClientList.ToList(); // Create a copy to iterate over
            }

            foreach (Client client in clientListCopy)
            {
                if (!_client.GroupedPlayers.Contains(client.Name) && _client != client)
                {
                    _client.RequestGroup(client.Name);
                }
            }
        }

        private void friendGroupBtn_Click(object sender, EventArgs e)
        {
            foreach (string selectedItem in groupList.SelectedItems.OfType<string>().ToList())
            {
                if (!_client.FriendBindingList.Contains(selectedItem))
                {
                    UpdateBindingList(_client.FriendBindingList, friendList, selectedItem);
                }
            }
            SaveFriendList();
            UpdateFriendList();
        }

        private void kickGroupedBtn_Click(object sender, EventArgs e)
        {
            foreach (string selectedItem in groupList.SelectedItems.OfType<string>().ToList())
            {
                _client.RequestGroup(selectedItem);
            }

        }

        private void friendAltsBtn_Click(object sender, EventArgs e)
        {
            BindingList<string> friends = _client.FriendBindingList;


            List<Client> clientListCopy;
            lock (_client.Server._clientListLock)
            {
                clientListCopy = _client.Server.ClientList.ToList(); // Create a copy to iterate over
            }

            foreach (Client client in clientListCopy)
            {
                if (!friends.Contains(client.Name) && _client != client)
                {
                    UpdateBindingList(friends, friendList, client.Name);
                }
                if (_client.StrangerBindingList.Contains(client.Name))
                {
                    _client.StrangerBindingList.Remove(client.Name);
                }
            }

            SaveFriendList();
            UpdateFriendList();

        }

        internal void UpdateGroupTargets()
        {
            foreach (string name in _client.GroupedPlayers)
            {
                if (_client.Bot.Alts == null || _client.Server.GetClient(name) == null)
                {
                    if (_client.Bot.ContainsAlly(name))
                    {
                        if (aislingTabControl.TabPages.ContainsKey(name))
                        {
                            aislingTabControl.TabPages[name].Dispose();
                        }
                        _client.Bot.RemoveAlly(name);
                    }
                    Ally ally = new Ally(name)
                    {
                        Page = _client.Bot.Group
                    };
                    _client.Bot.AddAlly(ally);
                }
            }
        }

        private void addGroupBtn_Click(object sender, EventArgs e)
        {
            if (!_isLoading && _client.Bot.Group != null)
            {
                MessageDialog.Show(_client.Server.MainForm, "You are already targeting the group.");
                return;
            }

            _client.Bot.Group = new AllyPage(new Ally("group"), _client);
            _client.Bot.Group.allyMDCRbtn.Visible = true;
            _client.Bot.Group.allyMDCSpamRbtn.Visible = true;
            _client.Bot.Group.allyMICSpamRbtn.Visible = true;
            _client.Bot.Group.allyNormalRbtn.Visible = true;
            TabPage tabPage = new TabPage("group");
            tabPage.Controls.Add(_client.Bot.Group);
            aislingTabControl.TabPages.Add(tabPage);
            foreach (string name in _client.GroupedPlayers)
            {
                if (_client.Bot.Alts == null || _client.Server.GetClient(name) == null)
                {
                    if (_client.Bot.ContainsAlly(name))
                    {
                        aislingTabControl.TabPages[name]?.Dispose();
                        _client.Bot.RemoveAlly(name);
                    }
                    Ally ally = new Ally(name)
                    {
                        Page = _client.Bot.Group
                    };
                    _client.Bot.AddAlly(ally);
                }
            }
            if (!_isLoading && MessageDialog.Show(_client.Server.MainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
            {
                aislingTabControl.SelectTab(tabPage);
                clientTabControl.SelectTab(1);
            }
        }

        private void addAltsBtn_Click(object sender, EventArgs e)
        {
            if (!_isLoading && _client.Bot.Alts != null)
            {
                MessageDialog.Show(_client.Server.MainForm, "You already targeting alts.");
                return;
            }
            _client.Bot.Alts = new AllyPage(new Ally("alts"), _client);
            TabPage tabPage = new TabPage("alts");
            tabPage.Controls.Add(_client.Bot.Alts);
            aislingTabControl.TabPages.Add(tabPage);

            List<Client> clientListCopy;
            lock (_client.Server._clientListLock)
            {
                clientListCopy = _client.Server.ClientList.ToList(); // Create a copy to iterate over
            }
            foreach (Client client in clientListCopy.Where((Client c) => c.Identifier != _client.Identifier))
            {
                if (_client.Bot.ContainsAlly(client.Name))
                {
                    aislingTabControl.TabPages[client.Name]?.Dispose();
                    _client.Bot.RemoveAlly(client.Name);

                }
                Ally ally = new Ally(client.Name)
                {
                    Page = _client.Bot.Alts
                };
                _client.Bot.AddAlly(ally);
            }
            if (!_isLoading && MessageDialog.Show(_client.Server.MainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
            {
                aislingTabControl.SelectTab(tabPage);
                clientTabControl.SelectTab(1);
            }
        }

        private void addAllMonstersBtn_Click(object sender, EventArgs e)
        {
            if (!_isLoading && _client.Bot.AllMonsters != null)
            {
                MessageDialog.Show(_client.Server.MainForm, "Enemy already in list");
                return;
            }
            EnemyPage enemyPage = new EnemyPage(new Enemy("all monsters"), _client);
            enemyPage.enemyPicture.Visible = false;
            enemyPage.priorityGroup.Visible = true;
            enemyPage.mpndDioned.Visible = true;
            enemyPage.ignoreGroup.Visible = true;
            TabPage tabPage = new TabPage("all monsters");
            tabPage.Controls.Add(enemyPage);
            ClearNearbyEnemies();
            monsterTabControl.TabPages.Remove(nearbyEnemyTab);
            while (monsterTabControl.TabPages.Count > 0)
            {
                monsterTabControl.TabPages[0]?.Dispose();
            }
            foreach (Enemy enemy in _client.Bot.ReturnEnemyList())
            {
                _client.Bot.ClearEnemyLists(enemy.ToString());
            }
            monsterTabControl.TabPages.Add(tabPage);
            foreach (Creature npc in _client.GetNearbyValidCreatures())
            {
                if (!_client.Bot.IsEnemyAlreadyListed(npc.SpriteID))
                {
                    Enemy enemy = new Enemy(npc.SpriteID)
                    {
                        EnemyPage = enemyPage
                    };
                    _client.Bot.UpdateEnemyList(enemy);
                }
            }
            if (!_isLoading && MessageDialog.Show(_client.Server.MainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
            {
                clientTabControl.SelectTab(2);
                monsterTabControl.SelectTab(tabPage);
            }
            _client.Bot.AllMonsters = enemyPage;
        }

        private void addMonsterBtn_Click(object sender, EventArgs e)
        {
            if ((ushort.TryParse(addMonsterText.Text, out ushort result)) || validateMonsterInput(result))
            {
                AddEnemyPage(result);
                addMonsterText.Clear();
                if (!_isLoading && MessageDialog.Show(_client.Server.MainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
                {
                    monsterTabControl.SelectTab(monsterTabControl.TabPages.IndexOfKey(result.ToString()));
                    clientTabControl.SelectTab(2);
                }
            }
        }

        private bool validateMonsterInput(ushort sprite)
        {
            if (sprite >= 1 && sprite <= 1000)
            {
                if (!_client.Bot.IsEnemyAlreadyListed(sprite) && _client.Bot.AllMonsters == null)
                {
                    return true;
                }
                MessageDialog.Show(_client.Server.MainForm, "Enemy already in list");
                addMonsterText.Clear();
                return false;
            }
            MessageDialog.Show(_client.Server.MainForm, "Your sprite must be a number between 1-1000");
            addMonsterText.Clear();
            return false;
        }

        private void SaveProfile(string savePath)
        {
            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }

            // Ensure savePath does not contain invalid characters
            if (Regex.Match(savePath, "[\\/\\*\\\"\\[\\]\\\\\\:\\?\\|\\<\\>\\.]").Success)
            {
                MessageDialog.Show(_client.Server.MainForm, "Cannot use characters /*\"[]\\:?|<>.");
                return;
            }

            // Build the directory path for the profile
            string profileDirectory = Settings.Default.DarkAgesPath.Replace("Darkages.exe", _client.Name + "\\Talos\\");
            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            // Create the full profile path (including filename)
            string profilePath = Path.Combine(profileDirectory, savePath + ".xml");

            // Create an instance of FormStateHelper and populate it with the form's control values
            FormStateHelper formState = new FormStateHelper
            {
                // ComboBox page
                ComboBoxPage = new ComboBoxPageState
                {
                    Combo1ListItems = new List<string>(combo1List.Lines),
                    Combo2ListItems = new List<string>(combo2List.Lines),
                    Combo3ListItems = new List<string>(combo3List.Lines),
                    Combo4ListItems = new List<string>(combo4List.Lines),
                    Combo1BtnText = combo1Btn.Text,
                    Combo2BtnText = combo2Btn.Text,
                    Combo3BtnText = combo3Btn.Text,
                    Combo4BtnText = combo4Btn.Text,
                    DontCbox1Checked = dontCbox1.Checked,
                    DontCBox2Checked = dontCBox2.Checked,
                    DontCBox3Checked = dontCBox3.Checked,
                    DontCBox4Checked = dontCBox4.Checked
                },

                // Aisling page
                AislingPage = new AislingPageState
                {
                    FollowText = followText.Text,
                    DoublesComboxText = doublesCombox.Text,
                    AutoDoubleCboxChecked = autoDoubleCbox.Checked,
                    ExpGemsComboxText = expGemsCombox.Text,
                    AutoGemCboxChecked = autoGemCbox.Checked,
                    AutoStaffCboxChecked = autoStaffCbox.Checked,
                    HideLinesCboxChecked = hideLinesCbox.Checked,
                    DionComboxText = dionCombox.Text,
                    DionWhenComboxText = dionWhenCombox.Text,
                    AiteComboxText = aiteCombox.Text,
                    HealComboxText = healCombox.Text,
                    FasComboxText = fasCombox.Text,
                    VineComboxText = vineCombox.Text,
                    OptionsSkullCboxChecked = optionsSkullCbox.Checked,
                    OptionsSkullSurrboxChecked = optionsSkullSurrbox.Checked,
                    OneLineWalkCboxChecked = oneLineWalkCbox.Checked,
                    DionCboxChecked = dionCbox.Checked,
                    HealCboxChecked = healCbox.Checked,
                    DeireasFaileasCboxChecked = deireasFaileasCbox.Checked,
                    AoSithCboxChecked = aoSithCbox.Checked,
                    AlertStrangerCboxChecked = alertStrangerCbox.Checked,
                    AlertRangerCboxChecked = alertRangerCbox.Checked,
                    AlertDuraCboxChecked = alertDuraCbox.Checked,
                    AlertSkulledCboxChecked = alertSkulledCbox.Checked,
                    AlertEXPCboxChecked = alertEXPCbox.Checked,
                    AlertItemCapCboxChecked = alertItemCapCbox.Checked,
                    AiteCboxChecked = aiteCbox.Checked,
                    FasCboxChecked = fasCbox.Checked,
                    DisenchanterCboxChecked = disenchanterCbox.Checked,
                    WakeScrollCboxChecked = wakeScrollCbox.Checked,
                    HideCboxChecked = hideCbox.Checked,
                    DruidFormCboxChecked = druidFormCbox.Checked,
                    MistCboxChecked = mistCbox.Checked,
                    ArmachdCboxChecked = armachdCbox.Checked,
                    FasSpioradCboxChecked = fasSpioradCbox.Checked,
                    BeagCradhCboxChecked = beagCradhCbox.Checked,
                    AegisSphereCboxChecked = aegisSphereCbox.Checked,
                    DragonScaleCboxChecked = dragonScaleCbox.Checked,
                    ManaWardCboxChecked = manaWardCbox.Checked,
                    RegenerationCboxChecked = regenerationCbox.Checked,
                    PerfectDefenseCboxChecked = perfectDefenseCbox.Checked,
                    DragonsFireCboxChecked = dragonsFireCbox.Checked,
                    AsgallCboxChecked = asgallCbox.Checked,
                    MuscleStimulantCboxChecked = muscleStimulantCbox.Checked,
                    NerveStimulantCboxChecked = nerveStimulantCbox.Checked,
                    MonsterCallCboxChecked = monsterCallCbox.Checked,
                    VanishingElixirCboxChecked = vanishingElixirCbox.Checked,
                    VineyardCboxChecked = vineyardCbox.Checked,
                    AutoRedCboxChecked = autoRedCbox.Checked,
                    FungusExtractCboxChecked = fungusExtractCbox.Checked,
                    MantidScentCboxChecked = mantidScentCbox.Checked,
                    AoSuainCboxChecked = aoSuainCbox.Checked,
                    AoCurseCboxChecked = aoCurseCbox.Checked,
                    AoPoisonCboxChecked = aoPoisonCbox.Checked,
                    FollowCboxChecked = followCbox.Checked,
                    BubbleBlockCboxChecked = bubbleBlockCbox.Checked,
                    SpamBubbleCboxChecked = spamBubbleCbox.Checked,
                    RangerStopCboxChecked = rangerStopCbox.Checked,
                    PickupGoldCboxChecked = pickupGoldCbox.Checked,
                    PickupItemsCboxChecked = pickupItemsCbox.Checked,
                    DropTrashCboxChecked = dropTrashCbox.Checked,
                    TrashList = new List<string>(_trashToDrop),
                    OverrideList = new List<string>(overrideList.Items.Cast<string>()),
                    SafeFSCboxChecked = safeFSCbox.Checked,
                    EquipmentRepairCboxChecked = equipmentrepairCbox.Checked,
                    RangerLogCboxChecked = rangerLogCbox.Checked,
                    ChkLastStepF5Checked = chkLastStepF5.Checked,
                    DionPctNumValue = dionPctNum.Value,
                    HealPctNumValue = healPctNum.Value,
                    ChkSpeedStrangersChecked = chkSpeedStrangers.Checked,
                    LockstepCboxChecked = lockstepCbox.Checked,
                    FollowDistanceNumValue = followDistanceNum.Value,
                    WalkSpeedSldrValue = walkSpeedSldr.Value,
                    NumLastStepTimeValue = numLastStepTime.Value,
                    WalkSpeedTlbl = walkSpeedLbl.Text,
                    FasSpioradText = fasSpioradText.Text,
                    VineText = vineText.Text,
                    //GMLogCBoxChecked = gmLogCBox.Checked,
                    //ChkGMSoundsChecked = chkGMSounds.Checked,
                    //ChkAltLoginChecked = chkAltLogin.Checked,
                },

                // Tools page
                ToolsPage = new ToolsPageState
                {
                    FormCboxChecked = formCbox.Checked,
                    FormNumValue = formNum.Value,
                    NoBlindCboxChecked = noBlindCbox.Checked,
                    MapZoomCboxChecked = mapZoomCbox.Checked,
                    SeeHiddenCboxChecked = seeHiddenCbox.Checked,
                    GhostHackCboxChecked = ghostHackCbox.Checked,
                    IgnoreCollisionCboxChecked = ignoreCollisionCbox.Checked,
                    HideForegroundCboxChecked = hideForegroundCbox.Checked,
                    MapFlagsEnableCboxChecked = mapFlagsEnableCbox.Checked,
                    MapSnowCboxChecked = mapSnowCbox.Checked,
                    MapTabsCboxChecked = mapTabsCbox.Checked,
                    MapSnowTileCboxChecked = mapSnowTileCbox.Checked,
                    UnifiedGuildChatCboxChecked = unifiedGuildChatCbox.Checked,
                    ToggleOverrideCboxChecked = toggleOverrideCbox.Checked,
                    DeformCBoxChecked = deformCbox.Checked
                },

                // Bashing Page
                BashingPage = new BashingPageState
                {
                    ChkWaitForFasChecked = chkWaitForFas.Checked,
                    ChkWaitForCradhChecked = chkWaitForCradh.Checked,
                    ChkFrostStrikeChecked = chkFrostStrike.Checked,
                    ChkUseSkillsFromRangeChecked = chkUseSkillsFromRange.Checked,
                    ChkChargeToTargetCbxChecked = ChargeToTargetCbx.Checked,
                    ChkAssistBasherChecked = assistBasherChk.Checked,
                    NumOverrideDistanceValue = overrideDistanceNum.Value,
                    NumAssitantStrayValue = numAssitantStray.Value,
                    NumPFCounterValue = numPFCounter.Value,
                    TextLeadBasherValue = leadBasherTxt.Text,
                    NumCrasherHealthValue = numCrasherHealth.Value,
                    NumExHealValue = numExHeal.Value,
                    NumBashSkillDelayValue = numBashSkillDelay.Value,
                    NumSkillIntValue = numSkillInt.Value,
                    NumPingCompensation1Value = pingCompensationNum1.Value,
                    NumMonsterWalkInterval1Value = monsterWalkIntervalNum1.Value,
                    NumAtkRangeValue = atkRangeNum.Value,
                    NumEngageRangeValue = engageRangeNum.Value,
                    //ChkTavWallHacksChecked = chkTavWallHacks.Checked,
                    ChkTavWallStrangerChecked = chkTavWallStranger.Checked,
                    RbtnLeaderTargetChecked = radioLeaderTarget.Checked,
                    RbtnAssistantStrayChecked = radioAssitantStray.Checked,
                    ChkAssailsChecked = chkBashAssails.Checked,
                    ChkExkuraChecked = chkExkuranum.Checked,
                    ChkProtect1CboxChecked = Protect1Cbx.Checked,
                    ChkProtect2CboxChecked = Protect2Cbx.Checked,
                    ChkBashAsgallChecked = chkBashAsgall.Checked,
                    ChkIgnoreWalledInChecked = ignoreCollisionCbox.Checked,
                    ChkRiskySkillsChecked = riskySkillsCbox.Checked,
                    ChkRiskySkillsDionChecked = riskySkillsDionCbox.Checked,
                    ChkCrasherCboxChecked = crasherCbox.Checked,
                    ChkUseCrashersChecked = chkCrasher.Checked,
                    ChkCrasherOnlyAsgallChecked = chkCrasherOnlyAsgall.Checked,
                    ChkCrasherAboveHPChecked = chkCrasherAboveHP.Checked,
                    ChkPrioritizeChecked = priorityCbox.Checked,
                    ChkPriorityOnlyChecked = priorityOnlyCbox.Checked,
                    TextProtect1Value = Protect1Cbx.Text,
                    TextProtect2Value = Protect2Cbx.Text,
                    PriorityLboxItems = new List<string>(priorityLBox.Items.Cast<string>()),
                }
            };

            // Save each AllyPage and EnemyPage
            foreach (TabPage tabPage in aislingTabControl.TabPages)
            {
                if (tabPage.Controls.OfType<AllyPage>().FirstOrDefault() is AllyPage allyPage
                    && tabPage.Name != "selfTab" && tabPage.Name != "nearbyAllyTab")
                {
                    formState.AllyPages.Add(new AllyPageState
                    {
                        AllyPageName = tabPage.Text,
                        DbAiteCboxChecked = allyPage.dbAiteCbox.Checked,
                        DbAiteComboxText = allyPage.dbAiteCombox.Text,
                        DbFasCboxChecked = allyPage.dbFasCbox.Checked,
                        DbFasComboxText = allyPage.dbFasCombox.Text,
                        DbIocCboxChecked = allyPage.dbIocCbox.Checked,
                        DbIocComboxText = allyPage.dbIocCombox.Text,
                        DbIocNumPctValue = allyPage.dbIocNumPct.Value,
                        DbArmachdCboxChecked = allyPage.dbArmachdCbox.Checked,
                        DbBCCboxChecked = allyPage.dbBCCbox.Checked,
                        DbRegenCboxChecked = allyPage.dbRegenCbox.Checked,
                        DispelSuainCboxChecked = allyPage.dispelSuainCbox.Checked,
                        DispelCurseCboxChecked = allyPage.dispelCurseCbox.Checked,
                        DispelPoisonCboxChecked = allyPage.dispelPoisonCbox.Checked,
                        MiscLyliacCboxChecked = allyPage.miscLyliacCbox.Checked,
                        MiscLyliacTboxText = allyPage.miscLyliacTbox.Text,
                        AllyNormalRbtnChecked = allyPage.allyNormalRbtn.Checked,
                        AllyMDCRbtnChecked = allyPage.allyMDCRbtn.Checked,
                        AllyMDCSpamRbtnChecked = allyPage.allyMDCSpamRbtn.Checked,
                        AllyMICSpamRbtnChecked = allyPage.allyMICSpamRbtn.Checked
                    });
                }
            }

            foreach (TabPage tabPage in monsterTabControl.TabPages)
            {
                if (tabPage.Controls.OfType<EnemyPage>().FirstOrDefault() is EnemyPage enemyPage
                    && tabPage.Name != "nearbyEnemyTab")
                {
                    formState.EnemyPages.Add(new EnemyPageState
                    {
                        EnemyPageName = tabPage.Text,
                        NearestFirstCbxChecked = enemyPage.NearestFirstCbx.Checked,
                        FarthestFirstCbxChecked = enemyPage.FarthestFirstCbx.Checked,
                        SpellsCurseCboxChecked = enemyPage.spellsCurseCbox.Checked,
                        SpellsCurseComboxText = enemyPage.spellsCurseCombox.Text,
                        SpellsFasCboxChecked = enemyPage.spellsFasCbox.Checked,
                        SpellsFasComboxText = enemyPage.spellsFasCombox.Text,
                        SpellsControlCboxChecked = enemyPage.spellsControlCbox.Checked,
                        SpellsControlComboxText = enemyPage.spellsControlCombox.Text,
                        TargetCboxChecked = enemyPage.targetCbox.Checked,
                        TargetComboxText = enemyPage.targetCombox.Text,
                        TargetCursedCboxChecked = enemyPage.targetCursedCbox.Checked,
                        TargetFassedCboxChecked = enemyPage.targetFassedCbox.Checked,
                        AttackCboxOneChecked = enemyPage.attackCboxOne.Checked,
                        AttackComboxOneText = enemyPage.attackComboxOne.Text,
                        AttackCboxTwoChecked = enemyPage.attackCboxTwo.Checked,
                        AttackComboxTwoText = enemyPage.attackComboxTwo.Text,
                        SpellFirstRbtnChecked = enemyPage.spellFirstRbtn.Checked,
                        PramhFirstRbtnChecked = enemyPage.pramhFirstRbtn.Checked,
                        CurseFirstRbtnChecked = enemyPage.curseFirstRbtn.Checked,
                        FasFirstRbtnChecked = enemyPage.fasFirstRbtn.Checked,
                        SpellAllRbtnChecked = enemyPage.spellAllRbtn.Checked,
                        SpellOneRbtnChecked = enemyPage.spellOneRbtn.Checked,
                        PriorityCboxChecked = enemyPage.priorityCbox.Checked,
                        PriorityOnlyCboxChecked = enemyPage.priorityOnlyCbox.Checked,
                        PriorityList = enemyPage.priorityLbox.Items.Cast<string>().ToList(),
                        ExpectedHitsNumValue = enemyPage.expectedHitsNum.Value
                    });
                }
            }

            // Now serialize the FormStateHelper object to XML and save it to the file
            XmlSerializer serializer = new XmlSerializer(typeof(FormStateHelper));
            using (FileStream fileStream = File.Create(profilePath))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true, // Makes the XML file more human-readable
                    NewLineOnAttributes = true
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings))
                {
                    serializer.Serialize(xmlWriter, formState);
                }
            }

            // Optionally add the saved profile to a list of profiles
            if (!botProfiles.Contains(savePath, StringComparer.CurrentCultureIgnoreCase))
            {
                botProfiles.Add(savePath);
            }

            // Store the last loaded profile (optional)
            LastLoadedProfile = savePath;
        }

        internal readonly SemaphoreSlim _loadLock = new SemaphoreSlim(1, 1);

        internal async Task LoadProfileAsync(string profilePath)
        {

            if (string.IsNullOrEmpty(profilePath))
            {
                return;
            }

            // Ensure only one load operation at a time
            await _loadLock.WaitAsync();


            try
            {
                _isLoading = true;
                // Set the last loaded profile
                LastLoadedProfile = profilePath;

                // Clear any existing options
                ClearOptions();

                // Load and deserialize the profile asynchronously
                string profileFullPath = Path.Combine(Settings.Default.DataPath, _client.Name, "Talos", profilePath + ".xml");

                if (!File.Exists(profileFullPath))
                {
                    _client.ServerMessage(0, "Profile does not exist.");
                    return;
                }

                FormStateHelper formState;

                // Asynchronously load the XML file
                using (FileStream fileStream = File.OpenRead(profileFullPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FormStateHelper));
                    formState = (FormStateHelper)serializer.Deserialize(fileStream);
                }

                // Update UI controls asynchronously
                await Task.Run(() =>
                {
                    Invoke(new Action(() =>
                    {
                        // Apply saved AllyPages
                        foreach (var allyState in formState.AllyPages)
                        {
                            addAislingText.Text = allyState.AllyPageName;

                            // Add the ally tab based on its name
                            if (allyState.AllyPageName == "group")
                            {
                                addGroupBtn_Click(null, EventArgs.Empty);
                            }
                            else if (allyState.AllyPageName == "alts")
                            {
                                addAltsBtn_Click(null, EventArgs.Empty);
                            }
                            else
                            {
                                addAislingBtn_Click(null, EventArgs.Empty);
                            }

                            // Find the newly added AllyPage tab and apply the saved state
                            foreach (TabPage tabPage in aislingTabControl.TabPages)
                            {
                                if (tabPage.Text == allyState.AllyPageName)
                                {
                                    var allyPage = tabPage.Controls.OfType<AllyPage>().FirstOrDefault();
                                    if (allyPage != null)
                                    {
                                        // Apply the AllyPageState values
                                        allyPage.dbAiteCbox.Checked = allyState.DbAiteCboxChecked;
                                        allyPage.dbAiteCombox.Text = allyState.DbAiteComboxText;
                                        allyPage.dbFasCbox.Checked = allyState.DbFasCboxChecked;
                                        allyPage.dbFasCombox.Text = allyState.DbFasComboxText;
                                        allyPage.dbIocCbox.Checked = allyState.DbIocCboxChecked;
                                        allyPage.dbIocCombox.Text = allyState.DbIocComboxText;
                                        allyPage.dbIocNumPct.Value = allyState.DbIocNumPctValue;
                                        allyPage.miscLyliacTbox.Text = allyState.MiscLyliacTboxText;

                                        // Apply RadioButton states
                                        allyPage.allyMICSpamRbtn.Checked = allyState.AllyMICSpamRbtnChecked;
                                        allyPage.allyMDCSpamRbtn.Checked = allyState.AllyMDCSpamRbtnChecked;
                                        allyPage.allyMDCRbtn.Checked = allyState.AllyMDCRbtnChecked;
                                        allyPage.allyNormalRbtn.Checked = allyState.AllyNormalRbtnChecked;

                                        // Apply other CheckBox states
                                        allyPage.dispelCurseCbox.Checked = allyState.DispelCurseCboxChecked;
                                        allyPage.dispelSuainCbox.Checked = allyState.DispelSuainCboxChecked;
                                        allyPage.dispelPoisonCbox.Checked = allyState.DispelPoisonCboxChecked;
                                    }
                                }
                            }
                        }

                        // Apply saved EnemyPages
                        foreach (var enemyState in formState.EnemyPages)
                        {
                            addMonsterText.Text = enemyState.EnemyPageName;

                            // Add the enemy tab based on its name
                            if (enemyState.EnemyPageName == "all monsters")
                            {
                                addAllMonstersBtn_Click(null, EventArgs.Empty);
                            }
                            else
                            {
                                addMonsterBtn_Click(null, EventArgs.Empty);
                            }

                            // Find the newly added EnemyPage tab and apply the saved state
                            foreach (TabPage tabPage in monsterTabControl.TabPages)
                            {
                                if (tabPage.Text == enemyState.EnemyPageName)
                                {
                                    var enemyPage = tabPage.Controls.OfType<EnemyPage>().FirstOrDefault();
                                    if (enemyPage != null)
                                    {
                                        // Apply CheckBox values
                                        enemyPage.NearestFirstCbx.Checked = enemyState.NearestFirstCbxChecked;
                                        enemyPage.FarthestFirstCbx.Checked = enemyState.FarthestFirstCbxChecked;
                                        enemyPage.spellsCurseCbox.Checked = enemyState.SpellsCurseCboxChecked;
                                        enemyPage.spellsFasCbox.Checked = enemyState.SpellsFasCboxChecked;
                                        enemyPage.spellsControlCbox.Checked = enemyState.SpellsControlCboxChecked;
                                        enemyPage.targetCbox.Checked = enemyState.TargetCboxChecked;
                                        enemyPage.targetCursedCbox.Checked = enemyState.TargetCursedCboxChecked;
                                        enemyPage.targetFassedCbox.Checked = enemyState.TargetFassedCboxChecked;
                                        enemyPage.attackCboxOne.Checked = enemyState.AttackCboxOneChecked;
                                        enemyPage.attackCboxTwo.Checked = enemyState.AttackCboxTwoChecked;
                                        enemyPage.mpndSilenced.Checked = enemyState.MpndSilencedChecked;
                                        enemyPage.mspgSilenced.Checked = enemyState.MspgSilencedChecked;
                                        enemyPage.mpndDioned.Checked = enemyState.MpndDionedChecked;
                                        enemyPage.mspgPct.Checked = enemyState.MspgPctChecked;
                                        enemyPage.NearestFirstCbx.Checked = enemyState.NearestFirstCbxChecked;

                                        // Apply ComboBox values
                                        enemyPage.spellsCurseCombox.Text = enemyState.SpellsCurseComboxText;
                                        enemyPage.spellsFasCombox.Text = enemyState.SpellsFasComboxText;
                                        enemyPage.spellsControlCombox.Text = enemyState.SpellsControlComboxText;
                                        enemyPage.targetCombox.Text = enemyState.TargetComboxText;
                                        enemyPage.attackComboxOne.Text = enemyState.AttackComboxOneText;
                                        enemyPage.attackComboxTwo.Text = enemyState.AttackComboxTwoText;

                                        // Apply NumericUpDown values
                                        enemyPage.expectedHitsNum.Value = enemyState.ExpectedHitsNumValue;

                                        // Apply ListBox items for priority
                                        enemyPage.priorityLbox.Items.Clear();
                                        foreach (var item in enemyState.PriorityLboxItems)
                                        {
                                            enemyPage.priorityLbox.Items.Add(item);
                                        }
                                    }
                                }
                            }
                        }


                        // ComboBoxPage controls
                        combo1List.Lines = formState.ComboBoxPage.Combo1ListItems.ToArray();
                        combo2List.Lines = formState.ComboBoxPage.Combo2ListItems.ToArray();
                        combo3List.Lines = formState.ComboBoxPage.Combo3ListItems.ToArray();
                        combo4List.Lines = formState.ComboBoxPage.Combo4ListItems.ToArray();

                        combo1Btn.Text = formState.ComboBoxPage.Combo1BtnText;
                        combo2Btn.Text = formState.ComboBoxPage.Combo2BtnText;
                        combo3Btn.Text = formState.ComboBoxPage.Combo3BtnText;
                        combo4Btn.Text = formState.ComboBoxPage.Combo4BtnText;

                        dontCbox1.Checked = formState.ComboBoxPage.DontCbox1Checked;
                        dontCBox2.Checked = formState.ComboBoxPage.DontCBox2Checked;
                        dontCBox3.Checked = formState.ComboBoxPage.DontCBox3Checked;
                        dontCBox4.Checked = formState.ComboBoxPage.DontCBox4Checked;

                        // AislingPage controls
                        followDistanceNum.Value = formState.AislingPage.FollowDistanceNumValue;
                        followText.Text = formState.AislingPage.FollowText;
                        doublesCombox.Text = formState.AislingPage.DoublesComboxText;
                        autoDoubleCbox.Checked = formState.AislingPage.AutoDoubleCboxChecked;
                        expGemsCombox.Text = formState.AislingPage.ExpGemsComboxText;
                        autoGemCbox.Checked = formState.AislingPage.AutoGemCboxChecked;
                        autoStaffCbox.Checked = formState.AislingPage.AutoStaffCboxChecked;
                        hideLinesCbox.Checked = formState.AislingPage.HideLinesCboxChecked;

                        dionCombox.Text = formState.AislingPage.DionComboxText;
                        dionWhenCombox.Text = formState.AislingPage.DionWhenComboxText;
                        aiteCombox.Text = formState.AislingPage.AiteComboxText;
                        healCombox.Text = formState.AislingPage.HealComboxText;
                        fasCombox.Text = formState.AislingPage.FasComboxText;
                        vineCombox.Text = formState.AislingPage.VineComboxText;

                        optionsSkullCbox.Checked = formState.AislingPage.OptionsSkullCboxChecked;
                        optionsSkullSurrbox.Checked = formState.AislingPage.OptionsSkullSurrboxChecked;
                        oneLineWalkCbox.Checked = formState.AislingPage.OneLineWalkCboxChecked;
                        dionCbox.Checked = formState.AislingPage.DionCboxChecked;
                        healCbox.Checked = formState.AislingPage.HealCboxChecked;
                        deireasFaileasCbox.Checked = formState.AislingPage.DeireasFaileasCboxChecked;
                        aoSithCbox.Checked = formState.AislingPage.AoSithCboxChecked;
                        alertStrangerCbox.Checked = formState.AislingPage.AlertStrangerCboxChecked;
                        alertRangerCbox.Checked = formState.AislingPage.AlertRangerCboxChecked;
                        alertDuraCbox.Checked = formState.AislingPage.AlertDuraCboxChecked;
                        alertSkulledCbox.Checked = formState.AislingPage.AlertSkulledCboxChecked;
                        alertEXPCbox.Checked = formState.AislingPage.AlertEXPCboxChecked;
                        alertItemCapCbox.Checked = formState.AislingPage.AlertItemCapCboxChecked;
                        aiteCbox.Checked = formState.AislingPage.AiteCboxChecked;
                        fasCbox.Checked = formState.AislingPage.FasCboxChecked;
                        disenchanterCbox.Checked = formState.AislingPage.DisenchanterCboxChecked;
                        wakeScrollCbox.Checked = formState.AislingPage.WakeScrollCboxChecked;
                        hideCbox.Checked = formState.AislingPage.HideCboxChecked;
                        druidFormCbox.Checked = formState.AislingPage.DruidFormCboxChecked;
                        mistCbox.Checked = formState.AislingPage.MistCboxChecked;
                        armachdCbox.Checked = formState.AislingPage.ArmachdCboxChecked;
                        fasSpioradCbox.Checked = formState.AislingPage.FasSpioradCboxChecked;
                        beagCradhCbox.Checked = formState.AislingPage.BeagCradhCboxChecked;
                        aegisSphereCbox.Checked = formState.AislingPage.AegisSphereCboxChecked;
                        dragonScaleCbox.Checked = formState.AislingPage.DragonScaleCboxChecked;
                        manaWardCbox.Checked = formState.AislingPage.ManaWardCboxChecked;
                        regenerationCbox.Checked = formState.AislingPage.RegenerationCboxChecked;
                        perfectDefenseCbox.Checked = formState.AislingPage.PerfectDefenseCboxChecked;
                        dragonsFireCbox.Checked = formState.AislingPage.DragonsFireCboxChecked;
                        asgallCbox.Checked = formState.AislingPage.AsgallCboxChecked;
                        muscleStimulantCbox.Checked = formState.AislingPage.MuscleStimulantCboxChecked;
                        nerveStimulantCbox.Checked = formState.AislingPage.NerveStimulantCboxChecked;
                        monsterCallCbox.Checked = formState.AislingPage.MonsterCallCboxChecked;
                        vanishingElixirCbox.Checked = formState.AislingPage.VanishingElixirCboxChecked;
                        vineyardCbox.Checked = formState.AislingPage.VineyardCboxChecked;
                        autoRedCbox.Checked = formState.AislingPage.AutoRedCboxChecked;
                        fungusExtractCbox.Checked = formState.AislingPage.FungusExtractCboxChecked;
                        mantidScentCbox.Checked = formState.AislingPage.MantidScentCboxChecked;
                        aoSuainCbox.Checked = formState.AislingPage.AoSuainCboxChecked;
                        aoCurseCbox.Checked = formState.AislingPage.AoCurseCboxChecked;
                        aoPoisonCbox.Checked = formState.AislingPage.AoPoisonCboxChecked;
                        followCbox.Checked = formState.AislingPage.FollowCboxChecked;
                        bubbleBlockCbox.Checked = formState.AislingPage.BubbleBlockCboxChecked;
                        spamBubbleCbox.Checked = formState.AislingPage.SpamBubbleCboxChecked;
                        rangerStopCbox.Checked = formState.AislingPage.RangerStopCboxChecked;
                        pickupGoldCbox.Checked = formState.AislingPage.PickupGoldCboxChecked;
                        pickupItemsCbox.Checked = formState.AislingPage.PickupItemsCboxChecked;
                        dropTrashCbox.Checked = formState.AislingPage.DropTrashCboxChecked;
                        //GMLogCBoxChecked = formState.AislingPage.gmLogCBox.Checked,
                        //ChkGMSoundsChecked = formState.AislingPage.chkGMSounds.Checked,
                        //ChkAltLoginChecked = formState.AislingPage.chkAltLogin.Checked,

                        // ListBox items
                        _trashToDrop.Clear();
                        foreach (string item in formState.AislingPage.TrashList)
                        {
                            UpdateBindingList(_trashToDrop, trashList, item);
                        }
                        overrideList.Items.Clear();
                        foreach (string item in formState.AislingPage.OverrideList)
                        {
                            overrideList.Items.Add(item);
                        }

                        // ToolsPage controls
                        formCbox.Checked = formState.ToolsPage.FormCboxChecked;
                        formNum.Value = formState.ToolsPage.FormNumValue;
                        noBlindCbox.Checked = formState.ToolsPage.NoBlindCboxChecked;
                        mapZoomCbox.Checked = formState.ToolsPage.MapZoomCboxChecked;
                        seeHiddenCbox.Checked = formState.ToolsPage.SeeHiddenCboxChecked;
                        ghostHackCbox.Checked = formState.ToolsPage.GhostHackCboxChecked;
                        ignoreCollisionCbox.Checked = formState.ToolsPage.IgnoreCollisionCboxChecked;
                        hideForegroundCbox.Checked = formState.ToolsPage.HideForegroundCboxChecked;
                        mapFlagsEnableCbox.Checked = formState.ToolsPage.MapFlagsEnableCboxChecked;
                        mapSnowCbox.Checked = formState.ToolsPage.MapSnowCboxChecked;
                        mapTabsCbox.Checked = formState.ToolsPage.MapTabsCboxChecked;
                        mapSnowTileCbox.Checked = formState.ToolsPage.MapSnowTileCboxChecked;
                        unifiedGuildChatCbox.Checked = formState.ToolsPage.UnifiedGuildChatCboxChecked;
                        toggleOverrideCbox.Checked = formState.ToolsPage.ToggleOverrideCboxChecked;
                        deformCbox.Checked = formState.ToolsPage.DeformCBoxChecked;

                        // BashingPage controls
                        chkWaitForFas.Checked = formState.BashingPage.ChkWaitForFasChecked;
                        chkWaitForCradh.Checked = formState.BashingPage.ChkWaitForCradhChecked;
                        chkFrostStrike.Checked = formState.BashingPage.ChkFrostStrikeChecked;
                        chkUseSkillsFromRange.Checked = formState.BashingPage.ChkUseSkillsFromRangeChecked;
                        ChargeToTargetCbx.Checked = formState.BashingPage.ChkChargeToTargetCbxChecked;
                        assistBasherChk.Checked = formState.BashingPage.ChkAssistBasherChecked;
                        overrideDistanceNum.Value = formState.BashingPage.NumOverrideDistanceValue;
                        numAssitantStray.Value = formState.BashingPage.NumAssitantStrayValue;
                        numPFCounter.Value = formState.BashingPage.NumPFCounterValue;
                        leadBasherTxt.Text = formState.BashingPage.TextLeadBasherValue;
                        numCrasherHealth.Value = formState.BashingPage.NumCrasherHealthValue;
                        numExHeal.Value = formState.BashingPage.NumExHealValue;
                        numBashSkillDelay.Value = formState.BashingPage.NumBashSkillDelayValue;
                        numSkillInt.Value = formState.BashingPage.NumSkillIntValue;
                        pingCompensationNum1.Value = formState.BashingPage.NumPingCompensation1Value;
                        monsterWalkIntervalNum1.Value = formState.BashingPage.NumMonsterWalkInterval1Value;
                        atkRangeNum.Value = formState.BashingPage.NumAtkRangeValue;
                        engageRangeNum.Value = formState.BashingPage.NumEngageRangeValue;
                        chkTavWallHacks.Checked = formState.BashingPage.ChkTavWallHacksChecked;
                        chkTavWallStranger.Checked = formState.BashingPage.ChkTavWallStrangerChecked;
                        radioLeaderTarget.Checked = formState.BashingPage.RbtnLeaderTargetChecked;
                        radioAssitantStray.Checked = formState.BashingPage.RbtnAssistantStrayChecked;
                        chkBashAssails.Checked = formState.BashingPage.ChkAssailsChecked;
                        chkExkuranum.Checked = formState.BashingPage.ChkExkuraChecked;
                        Protect1Cbx.Checked = formState.BashingPage.ChkProtect1CboxChecked;
                        Protect2Cbx.Checked = formState.BashingPage.ChkProtect2CboxChecked;
                        chkBashAsgall.Checked = formState.BashingPage.ChkBashAsgallChecked;
                        ignoreCollisionCbox.Checked = formState.BashingPage.ChkIgnoreWalledInChecked;
                        riskySkillsCbox.Checked = formState.BashingPage.ChkRiskySkillsChecked;
                        riskySkillsDionCbox.Checked = formState.BashingPage.ChkRiskySkillsDionChecked;
                        crasherCbox.Checked = formState.BashingPage.ChkCrasherCboxChecked;
                        chkCrasher.Checked = formState.BashingPage.ChkUseCrashersChecked;
                        chkCrasherOnlyAsgall.Checked = formState.BashingPage.ChkCrasherOnlyAsgallChecked;
                        chkCrasherAboveHP.Checked = formState.BashingPage.ChkCrasherAboveHPChecked;
                        Protect1Cbx.Text = formState.BashingPage.TextProtect1Value;
                        Protect2Cbx.Text = formState.BashingPage.TextProtect2Value;
                        priorityCbox.Checked = formState.BashingPage.ChkPrioritizeChecked;
                        priorityOnlyCbox.Checked = formState.BashingPage.ChkPriorityOnlyChecked;
                        priorityLBox.Items.Clear();
                        foreach (string item in formState.BashingPage.PriorityLboxItems)
                        {
                            priorityLBox.Items.Add(item);
                        }

                    }));
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file access issues, deserialization errors)
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"Error loading profile: {ex.Message}");
            }
            finally
            {
                // Reset loading flag
                _isLoading = false;

                // Release the semaphore
                _loadLock.Release();
            }
        }



        private void loadStrip_Enter(object sender, EventArgs e)
        {
            while (loadStrip.DropDownItems.Count > 0)
            {
                loadStrip.DropDownItems[0].Dispose();
            }
            foreach (string item in botProfiles)
            {
                loadStrip.DropDownItems.Add(new ToolStripMenuItem(item, null, LoadMenuItem_Click, item));
            }
            loadStrip.DropDownClosed += deleteStrip_DropDownClosed;
        }
        private async void LoadMenuItem_Click(object sender, EventArgs e)
        {
            string profileName = (sender as ToolStripMenuItem)?.Text;

            if (!string.IsNullOrEmpty(profileName))
            {
                await LoadProfileAsync(profileName);
            }
        }
        internal void deleteStrip_DropDownClosed(object sender, EventArgs e)
        {
            (sender as ToolStripDropDownItem).DropDown.Close();
        }
        internal void ClearOptions()
        {
            // Preserve states
            bool toggleDmuChecked = toggleDmuCbox.Checked;
            bool toggleGenderChecked = toggleGenderCbox.Checked;

            // Clear controls recursively
            ClearAllControls(Controls);

            // Restore specific states
            toggleDmuCbox.Checked = toggleDmuChecked;
            toggleGenderCbox.Checked = toggleGenderChecked;
            ResetSpecificOptions();
        }

        private void ClearAllControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.Items.Count > 0)
                        comboBox.Text = comboBox.Items[0].ToString();
                    else
                        comboBox.Text = string.Empty;
                }
                else if (control.Controls.Count > 0)
                {
                    // Recursively clear child controls
                    ClearAllControls(control.Controls);
                }
            }
        }

        private void ResetSpecificOptions()
        {
            rangerStopCbox.Checked = false;
            renameCombox.Text = "Select Staff Type";
            autoStaffCbox.Checked = true;
            seeHiddenCbox.Checked = true;
            noBlindCbox.Checked = true;
            alertRangerCbox.Checked = true;
            rangerStopCbox.Checked = true;

            // Reset trash list with BindingList
            ResetTrashList();

            // Reset form-specific controls
            formNum.Value = 1M;
            followDistanceNum.Value = 1M;
            walkSpeedSldr.Value = 150;
            dionPctNum.Value = 80M;
            healPctNum.Value = 80M;
            fasSpioradText.Text = "5000";
            walkBtn.Text = "Walk";
            btnBashingNew.Text = "Start Bashing";
            //btnBashingNew.Text = "Start Bashing";

            // Clear combo lists
            combo1List.Clear();
            combo2List.Clear();
            combo3List.Clear();
            combo4List.Clear();

            _client.WalkSpeed = 150;
            walkSpeedLbl.Text = "150";

            // Clear ally and enemy pages
            ClearAllyAndEnemyPages();
        }

        private void ResetTrashList()
        {
            trashList.DataSource = null; // Clear the current binding

            BindingList<string> bindingList = new BindingList<string>
            {
                "fior sal", "fior creag", "fior srad", "fior athar", "Purple Potion",
                "Blue Potion", "Light Belt", "Passion Flower", "Gold Jade Necklace",
                "Bone Necklace", "Amber Necklace", "Half Talisman", "Iron Greaves",
                "Goblin Helmet", "Cordovan Boots", "Shagreen Boots", "Magma Boots",
                "Hy-brasyl Bracer", "Hy-brasyl Gauntlet", "Hy-brasyl Belt", "Magus Apollo",
                "Holy Apollo", "Magus Diana", "Holy Diana", "Magus Gaea", "Holy Gaea"
            };

            _trashToDrop = bindingList;
            trashList.DataSource = _trashToDrop;
        }

        private void ClearAllyAndEnemyPages()
        {
            // Clear ally pages
            foreach (TabPage tabPage in aislingTabControl.TabPages)
            {
                if (tabPage != selfTab && tabPage.Controls.OfType<AllyPage>().Any())
                {
                    var allyPage = tabPage.Controls.OfType<AllyPage>().First();
                    allyPage.allyRemoveBtn_Click(this, EventArgs.Empty);
                }
            }

            // Clear enemy pages
            foreach (TabPage tabPage in monsterTabControl.TabPages)
            {
                if (tabPage != nearbyEnemyTab && tabPage.Controls.OfType<EnemyPage>().Any())
                {
                    var enemyPage = tabPage.Controls.OfType<EnemyPage>().First();
                    enemyPage.enemyRemoveBtn_Click(this, EventArgs.Empty);
                }
            }
        }




        private void addMonsterText_Enter(object sender, EventArgs e)
        {

        }

        private void addMonsterText_Leave(object sender, EventArgs e)
        {

        }

        private void addWarpBtn_Click(object sender, EventArgs e)
        {

        }

        private void removeWarpBtn_Click(object sender, EventArgs e)
        {

        }

        private void chkMappingToggle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void effectBarIdsBtn_Click(object sender, EventArgs e)
        {
            if (!_client.EffectsBar.Any())
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, "You currently have no effects");
            }
            else
            {
                foreach (var spell in _client.EffectsBar)
                {
                    _client.ServerMessage((byte)ServerMessageType.Whisper, spell.ToString());
                }
            }
        }

        private void mapNodeIdsBtn_Click(object sender, EventArgs e)
        {
            if (_client.WorldMap == null)
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, "Please be in the world map before clicking!");
            }
            else
            {
                var messages = _client.WorldMap.Nodes
                    .Select(node => $"{node.Name}: {node.MapID}  {node.Location}")
                    .ToArray();

                foreach (var message in messages)
                {
                    _client.ServerMessage((byte)ServerMessageType.Whisper, message);
                }
            }
        }

        private void classDetectorBtn_Click(object sender, EventArgs e)
        {
            _client.ServerMessage((byte)ServerMessageType.Whisper, "Prev Class: " + _client.PreviousClassFlag.ToString());
            _client.ServerMessage((byte)ServerMessageType.Whisper, "Current Class: " + _client.TemuairClassFlag.ToString());
            _client.ServerMessage((byte)ServerMessageType.Whisper, "Med Class: " + _client.MedeniaClassFlag.ToString());
        }

        private void pursuitIdsBtn_Click(object sender, EventArgs e)
        {
            if (!_client.Server.PursuitIDs.Any())
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, "No pursuits recorded yet, please click an NPC");
                return;
            }

            var messages = _client.Server.PursuitIDs
                .Select(p => $"{p.Value}: {p.Key}   ")
                .ToList();

            foreach (var message in messages)
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, message);
            }
        }

        private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toggleHideToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void walkToMenu_MouseEnter(object sender, EventArgs e)
        {

        }

        private void waypointsMenu_Click(object sender, EventArgs e)
        {

        }

        private void waypointsMenu_MouseEnter(object sender, EventArgs e)
        {

        }

        private void deleteStrip_MouseEnter(object sender, EventArgs e)
        {
            while (deleteStrip.DropDownItems.Count > 0)
            {
                deleteStrip.DropDownItems[0].Dispose();
            }
            foreach (string item in botProfiles)
            {
                deleteStrip.DropDownItems.Add(new ToolStripMenuItem(item, null, deleteMenuItem_Click, item));
            }
            deleteStrip.DropDownClosed += deleteStrip_DropDownClosed;
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            string text = (sender as ToolStripMenuItem).Text;
            if (MessageDialog.Show(_client.Server.MainForm, "ARE YOU SURE YOU WANT TO DELETE " + text + "?") == DialogResult.OK)
            {
                DeleteProfile(text);
            }
        }
        private void DeleteProfile(string filename)
        {
            try
            {
                string path = Settings.Default.DarkAgesPath.Replace("Darkages.exe", _client.Name + "\\Talos\\" + filename + ".xml");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (botProfiles.Contains(filename, StringComparer.CurrentCultureIgnoreCase))
                {
                    botProfiles.Remove(filename);
                }
            }
            catch
            {
            }
        }

        private void saveStrip_Click(object sender, EventArgs e)
        {
            string input = string.Empty;
            if (!string.IsNullOrEmpty(LastLoadedProfile) && MessageDialog.Show(_client.Server.MainForm, "Save this profile as " + LastLoadedProfile + "?") == DialogResult.OK)
            {
                SaveProfile(LastLoadedProfile);
            }
            else if (InputDialog.Show(this, "Please enter a name for your new profile to be saved as.", out input) == DialogResult.OK)
            {
                SaveProfile(input);
            }
        }

        private void loadStrip_MouseEnter(object sender, EventArgs e)
        {

        }

        private void clearStrip_Click(object sender, EventArgs e) => ClearOptions();

        private void numComboImgSelect_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSkillCombo_Click(object sender, EventArgs e)
        {

        }


        private void faceNum_ValueChanged(object sender, EventArgs e)
        {

        }

        private void removeRenameBtn_Click(object sender, EventArgs e)
        {

        }

        private void addRenameBtn_Click(object sender, EventArgs e)
        {

        }

        private void btnCheckLoginTime_Click(object sender, EventArgs e)
        {
            if (_client.Server.ConsecutiveLogin.ContainsKey(_client.Name.ToUpper()))
            {
                TimeSpan timeSpan = _client.Server.ConsecutiveLogin[_client.Name.ToUpper()].Subtract(DateTime.UtcNow);
                _client.ServerMessage((byte)ServerMessageType.Whisper, timeSpan.ToString("hh\\:mm\\:ss") + " since you've used Consecutive Login.");
            }
        }

        private void btnConLogin_Click(object sender, EventArgs e)
        {
            if (btnConLogin.Text == "Start")
            {
                btnConLogin.Text = "Stop";
            }
            else
            {
                btnConLogin.Text = "Start";
            }
        }

        private void btnFood_Click(object sender, EventArgs e)
        {

        }

        private void TailorBtn_Click(object sender, EventArgs e)
        {

        }

        private void ascendAllBtn_Click(object sender, EventArgs e)
        {

        }

        private void prayerAssistTbox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void togglePrayingBtn_Click(object sender, EventArgs e)
        {

        }

        private void btnItemRemove_Click(object sender, EventArgs e)
        {

        }

        private void btnItemAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadItemList_Click(object sender, EventArgs e)
        {

        }

        private void btnItemFinder_Click(object sender, EventArgs e)
        {

        }

        private void voteForAllBtn_Click(object sender, EventArgs e)
        {

        }

        private void voteText_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void toggleVotingBtn_Click(object sender, EventArgs e)
        {

        }

        private void fishingCombox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toggleFishingBtn_Click(object sender, EventArgs e)
        {

        }

        private void setNearestBankBtn_Click(object sender, EventArgs e)
        {

        }

        private void returnFarmBtn_Click(object sender, EventArgs e)
        {

        }

        private void setCustomBankBtn_Click(object sender, EventArgs e)
        {

        }

        private void toggleFarmBtn_Click(object sender, EventArgs e)
        {

        }

        private void finishedWhiteBtn_Click(object sender, EventArgs e)
        {

        }

        private void teacherText_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void killedCrabBtn_Click(object sender, EventArgs e)
        {

        }

        private void killedBatBtn_Click(object sender, EventArgs e)
        {

        }

        private void toggleHubaeBtn_Click(object sender, EventArgs e)
        {

        }

        private void laborNameText_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void laborBtn_Click(object sender, EventArgs e)
        {

        }

        private void polishBtn_Click(object sender, EventArgs e)
        {

        }

        private void assistText_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void toggleAscensionBtn_Click(object sender, EventArgs e)
        {

        }

        internal bool ComboChecker(int comboNum)
        {
            if (!(comboGroup.Controls["dontCbox" + comboNum] as CheckBox)?.Checked ?? false)
                return true;

            Location targetLocation = _client.ClientLocation.Offsetter(_client.ClientDirection);

            List<WorldObject> nearbyObjects = _client.GetNearbyObjects();

            if (nearbyObjects == null || !nearbyObjects.OfType<Creature>().Any())
                return false;

            return nearbyObjects
                .OfType<Creature>()
                .Any(creature => creature.Location == targetLocation);
        }


        private void useDoubleBtn_Click(object sender, EventArgs e)
        {
            var itemMappings = new Dictionary<string, string>
            {
                { "Kruna 50%", "50 Percent EXP/AP Bonus" },
                { "Kruna 100%", "Double EXP/AP Bonus" },
                { "Xmas 50%", "XMas Bonus Exp-Ap" },
                { "Star 100%", "Double Bonus Exp-Ap" },
                { "Vday 100%", "VDay Bonus Exp-Ap" }
            };

            if (doublesCombox.Text == "Xmas 100%")
            {
                var itemText = _client.HasItem("Christmas Double Exp-Ap") ? "Christmas Double Exp-Ap" : "XMas Double Exp-Ap";
                UseDouble(itemText);
            }
            else if (itemMappings.TryGetValue(doublesCombox.Text, out var itemText))
            {
                UseDouble(itemText);
            }

            UpdateExpBonusTimer();
        }

        internal void UseDouble(string doubleName)
        {
            if (!_client.UseItem(doubleName))
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"You do not own any {doubleName}.");
                autoDoubleCbox.Checked = false;
            }
        }

        internal void CastMushroom()
        {
            var itemMappings = new Dictionary<string, string>
            {
                { "Double", "Double Experience Mushroom" },
                { "50 Percent", "50 Percent Experience Mushroom" },
                { "Greatest", "Greatest Experience Mushroom" },
                { "Greater", "Greater Experience Mushroom" },
                { "Great", "Great Experience Mushroom" },
                { "Experience Mushroom", "Experience Mushroom" }
            };

            if (mushroomCombox.Text == "Best Available")
            {
                var mushrooom = FindBestMushroomInInventory(_client);
                UseMushroom(mushrooom);
            }
            else if (itemMappings.TryGetValue(mushroomCombox.Text, out var mushroom))
            {
                UseMushroom(mushroom);
            }

            UpdateMushroomBonusTimer();
        }

        internal void UseMushroom(string mushroomName)
        {
            if (!_client.UseItem(mushroomName))
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"You do not own any {mushroomName}.");
                autoMushroomCbox.Checked = false;
            }
        }

        internal string FindBestMushroomInInventory(Client client)
        {
            List<string> mushroomNames = new List<string>
            {
                "Double Experience Mushroom",
                "50 Percent Experience Mushroom",
                "Greatest Experience Mushroom",
                "Greater Experience Mushroom",
                "Great Experience Mushroom",
                "Experience Mushroom"
            };

            foreach (string mushroomName in mushroomNames)
            {
                if (client.HasItem(mushroomName))
                    return mushroomName;
            }

            return null;
        }

        internal void UpdateExpBonusTimer()
        {
            _client.Bot._lastExpBonusAppliedTime = DateTime.Now;
            _client.Bot._expBonusElapsedTime = _client.Bot._expBonusElapsedTime;

            if (!expBonusCooldownTimer.Enabled)
            {
                expBonusCooldownTimer.Enabled = true;
                expBonusCooldownTimer.Start();
            }
        }

        internal void UpdateMushroomBonusTimer()
        {
            _client.Bot._lastMushroomBonusAppliedTime = DateTime.Now;
            _client.Bot._mushroomBonusElapsedTime = _client.Bot._mushroomBonusElapsedTime;

            if (!mushroomBonusCooldownTimer.Enabled)
            {
                mushroomBonusCooldownTimer.Enabled = true;
                mushroomBonusCooldownTimer.Start();
            }
        }





        internal void StopBot()
        {
            if (startStrip.Text == "Stop")
            {
                _client.BotBase.Stop();
            }
        }

        private void useGemBtn_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                UseExpGems();
            });
        }

        private void UseExpGems()
        {
            StopBot();

            if (!_client.UseItem("Experience Gem"))
            {
                _client.ServerMessage((byte)ServerMessageType.OrangeBar1, "You do not have any experience gems.");
                return;
            }
            while (_client.Dialog == null)
            {
                Thread.Sleep(25);
            }
            byte objectType = _client.Dialog.ObjectType;
            int objectID = _client.Dialog.ObjectID;
            ushort pursuitID = _client.Dialog.PursuitID;
            ushort dialogID = _client.Dialog.DialogID;
            byte choice = (byte)((expGemsCombox.Text == "Ascend HP") ? 1 : 2);
            _client.Dialog.DialogNext();
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1));
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1), choice);
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1));
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1));
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1));
            Thread.Sleep(1000);
            while (_client.Dialog == null)
            {
                Thread.Sleep(25);
            }
            objectType = _client.Dialog.ObjectType;
            objectID = _client.Dialog.ObjectID;
            pursuitID = _client.Dialog.PursuitID;
            dialogID = _client.Dialog.DialogID;
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1));
            _client.ReplyDialog(objectType, objectID, pursuitID, (ushort)(dialogID + 1), 2);
            _client.ReplyDialog(objectType, objectID, pursuitID, dialogID);
            if (startStrip.Text == "Stop")
            {
                _client.BotBase.Start();
            }
        }

        private void calcResetBtn_Click(object sender, EventArgs e)
        {
            expSessionLbl.Text = "Session";
            expHourLbl.Text = "EXP/hr";
            _sessionExperience = 0uL;
            _sessionAbility = 0u;
            _sessionGold = 0u;
            _sessionExperienceStopWatch.Reset();
            _sessionAbilityStopWatch.Reset();
            _sessionGoldStopWatch.Reset();
        }

        internal void DisplaySessionStats()
        {
            expBoxedLbl.ForeColor = ((_client.Experience > 4250000000u) ? Color.Red : Color.Black);
            expBoxedLbl.Text = "Boxed: " + Math.Truncate((double)_client.Experience).ToString("N0");
            expSessionLbl.Text = "Session: " + Math.Truncate((double)_sessionExperience).ToString("N0");
            if (_sessionExperienceStopWatch.IsRunning)
            {
                expHourLbl.Text = "EXP/hr: " + Math.Truncate(_sessionExperience / _sessionExperienceStopWatch.Elapsed.TotalHours).ToString("N0");
            }
            else if (_sessionExperience > 0L)
            {
                _sessionExperienceStopWatch.Start();
            }
            if (_client.AbilityExperience < _abilityExp)
            {
                _abilityExp = _client.AbilityExperience;
                _sessionAbility = 0u;
                _sessionAbilityStopWatch.Reset();
            }
            else if (_sessionAbility == 0)
            {
                _sessionAbility = _client.AbilityExperience;
                _sessionAbilityStopWatch.Start();
            }
            else if (_sessionAbilityStopWatch.IsRunning)
            {
                apHourLbl.Text = "AP/hr: " + Math.Truncate((_client.AbilityExperience - _sessionAbility) / _sessionAbilityStopWatch.Elapsed.TotalHours).ToString("N0");
            }
            if (_client.Gold < _gold)
            {
                _gold = _client.Gold;
                _sessionGold = 0u;
                _sessionGoldStopWatch.Reset();
            }
            else if (_sessionGold == 0)
            {
                _sessionGold = _client.Gold;
                _sessionGoldStopWatch.Start();
            }
            else if (_sessionGoldStopWatch.IsRunning)
            {
                goldHourLbl.Text = "Gold/hr: " + Math.Truncate((_client.Gold - _sessionGold) / _sessionGoldStopWatch.Elapsed.TotalHours).ToString("N0");
            }
        }

        private void safeScreenCbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                _client.SafeScreen = true;
                noBlindCbox.Checked = false;
                seeHiddenCbox.Checked = false;
                mapZoomCbox.Checked = false;
                ghostHackCbox.Checked = false;
                mapFlagsEnableCbox.Checked = false;
                viewDMUCbox.Checked = false;
                toggleDmuCbox.Checked = false;
                //_client._server._mainForm.dressupDictionary.Clear();
                _client.RefreshRequest(false);
            }
            else
            {
                _client.SafeScreen = false;
            }
        }

        private void noBlindCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (noBlindCbox.Checked)
            {
                _client.EnableCheats(Cheats.NoBlind);
            }
            else
            {
                _client.EnableCheats(Cheats.NoBlind);
            }
            _client.SetStatUpdateFlags(StatUpdateFlags.Secondary);
        }

        private void seeHiddenCbox_CheckedChanged(object sender, EventArgs e)
        {

            if (seeHiddenCbox.Checked)
            {
                _client.EnableCheats(Cheats.SeeHidden);
            }
            else
            {
                _client.DisableCheats(Cheats.SeeHidden);
            }
            foreach (Player player in _client.GetNearbyPlayers())
            {
                _client.DisplayAisling(player);
            }

        }

        private void mapZoomCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mapZoomCbox.Checked)
            {
                _client.EnableCheats(Cheats.ZoomableMap);
            }
            else
            {
                _client.DisableCheats(Cheats.ZoomableMap);
            }
            _client.EnableMapZoom();
        }

        private void hideForegroundCbox_CheckedChanged(object sender, EventArgs e)
        {
            byte[] array = new byte[5];
            NativeMethods.ReadMemoryFromProcess(Process.GetProcessById(_client.processId), _client.BASE_ADDRESS, array);
            if (array[0] == _client.ADDRESS_BUFFER[0])
            {
                NativeMethods.WriteMemoryToProcess(Process.GetProcessById(_client.processId), _client.BASE_ADDRESS, _client.PROCESS_DATA);
            }
            else
            {
                NativeMethods.WriteMemoryToProcess(Process.GetProcessById(_client.processId), _client.BASE_ADDRESS, _client.ADDRESS_BUFFER);
            }
            _client.RefreshRequest();
        }

        private void ignoreCollisionCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreCollisionCbox.Checked)
            {
                _client.EnableCheats(Cheats.GmMode);
            }
            else
            {
                _client.DisableCheats(Cheats.GmMode);
            }
            _client.SetStatUpdateFlags(StatUpdateFlags.None);
        }

        private void ghostHackCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ghostHackCbox.Checked)
            {
                _client.EnableCheats(Cheats.SeeGhosts);
                foreach (Player value in _client.NearbyGhosts.Values)
                {
                    _client.SeeGhosts(value);
                }
            }
            else
            {
                _client.DisableCheats(Cheats.SeeGhosts);
                foreach (Player value2 in _client.NearbyGhosts.Values)
                {
                    _client.RemoveObject(value2.ID);
                }
            }
        }
        private void SetupInitialClientHacks()
        {
            _client.EnableCheats(noBlindCbox.Checked ? Cheats.NoBlind : Cheats.None);
            _client.EnableCheats(seeHiddenCbox.Checked ? Cheats.SeeHidden : Cheats.None);
            _client.EnableCheats(ignoreCollisionCbox.Checked ? Cheats.GmMode : Cheats.None);
            _client.EnableCheats(ghostHackCbox.Checked ? Cheats.SeeGhosts : Cheats.None);

            // Update stat flags
            _client.SetStatUpdateFlags(StatUpdateFlags.Secondary);

        }

        internal void AutoLoginHandlers()
        {
            //Adam
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // Iterate through Exits on the current map and print them to the console
            foreach (KeyValuePair<Structs.Point, Warp> exit in _client.Map.Exits)
            {
                Structs.Point exitPoint = exit.Key; // The point where the warp exists on the map
                Warp warp = exit.Value;     // The warp object itself
                Console.WriteLine($"Exit at {exitPoint}: {warp}");
            }

            foreach (KeyValuePair<Structs.Point, WorldMap> worldMapLinks in _client.Map.WorldMaps)
            {
                Structs.Point exitPoint = worldMapLinks.Key;
                WorldMap worldMap = worldMapLinks.Value;
                Console.WriteLine($"World Map Link at {exitPoint}: Field: {worldMap.Field}");
            }

        }

        private void walkBtn_Click(object sender, EventArgs e)
        {
            if (walkBtn.Text == "Walk")
            {
                walkBtn.Text = "Stop";
                return;
            }
            walkBtn.Text = "Walk";
            _client.IsWalking = false;
        }

        private void waypointsMenu_Click_1(object sender, EventArgs e)
        {
            if (WayForm == null || WayForm.IsDisposed)
            {
                WayForm = new WayForm(_client);
                WayForm.Show();
            }
            else
            {
                // Bring the existing form to the front
                WayForm.Show();
            }
        }

        private void removeTrashBtn_Click(object sender, EventArgs e)
        {
            if (trashList.SelectedItem != null)
            {
                _trashToDrop.Remove(trashList.SelectedItem.ToString());
            }
        }

        private void addTrashBtn_Click(object sender, EventArgs e)
        {
            if (!_trashToDrop.Contains(addTrashText.Text, StringComparer.CurrentCultureIgnoreCase))
            {
                UpdateBindingList(_trashToDrop, trashList, addTrashText.Text);
                addTrashText.Text = "";
            }
        }

        private void getSpriteBtn_Click(object sender, EventArgs e)
        {
            if (_client.Inventory[1] != null)
            {
                _client.ServerMessage(0, "Your " + _client.Inventory[1].Name + " has a Sprite ID of " + (_client.Inventory[1].Sprite - CONSTANTS.ITEM_SPRITE_OFFSET) + ".");
            }
        }

        private void removeOverrideBtn_Click(object sender, EventArgs e)
        {
            if (overrideList.SelectedItem != null)
            {
                overrideList.Items.Remove(overrideList.SelectedItem);
            }
        }

        private void addOverrideBtn_Click(object sender, EventArgs e)
        {
            if (!overrideList.Items.Contains(overrideText.Text))
            {
                overrideList.Items.Add(overrideText.Text);
                overrideText.Clear();
            }
        }

        private void walkAllClientsBtn_Click(object sender, EventArgs e)
        {
            List<Client> clientListCopy;
            lock (_client.Server._clientListLock)
            {
                clientListCopy = _client.Server.ClientList.ToList(); // Create a copy to iterate over
            }

            foreach (Client client in clientListCopy)
            {
                if (client != _client)
                {
                    client.ClientTab.walkMapCombox.Text = walkMapCombox.Text;
                    client.ClientTab.walkSpeedSldr.Value = walkSpeedSldr.Value;
                    client.ClientTab.walkSpeedSldr_Scroll(client.ClientTab.walkSpeedSldr, new EventArgs());
                    client.ClientTab.walkBtn.Text = "Stop";
                }
            }
        }

        private void StopWalk(object sender, EventArgs e) => walkBtn.Text = "Walk";
        private void SetWalk(object sender, EventArgs e)
        {
            walkMapCombox.Text = (sender as ToolStripMenuItem).Text;
            walkBtn.Text = "Stop";
        }
        private void WalkToMenu_Click(object sender, EventArgs e)
        {
            if (walkToMenu.DropDownItems.Count > 0)
            {
                walkToMenu.DropDownItems.Clear();
            }

            AddMenuItem(walkToMenu, "STOP", StopWalk);

            foreach (string item in walkMapCombox.Items)
            {
                AddMenuItem(walkToMenu, item, SetWalk);
            }

            walkToMenu.DropDownClosed += new EventHandler(DropClosed);
        }
        internal void DropClosed(object sender, EventArgs e)
        {
            (sender as ToolStripDropDownItem).DropDown.Close();
        }

        // Helper method to add a menu item
        private void AddMenuItem(ToolStripMenuItem parentMenu, string text, EventHandler clickHandler)
        {
            var menuItem = new ToolStripMenuItem(text, null, clickHandler);
            parentMenu.DropDownItems.Add(menuItem);
        }

        private void toggleBugBtn_Click(object sender, EventArgs e)
        {
            toggleBugBtn.Text = toggleBugBtn.Text == "Enable" ? "Disable" : "Enable";
        }

        private void toggleDojoBtn_Click(object sender, EventArgs e)
        {
            toggleDojoBtn.Text = toggleDojoBtn.Text == "Enable" ? "Disable" : "Enable";
        }

        private void toggleYuleBtn_Click(object sender, EventArgs e)
        {
            toggleYuleBtn.Text = toggleYuleBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void togglePigChaseBtn_Click(object sender, EventArgs e)
        {
            togglePigChaseBtn.Text = togglePigChaseBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleFowlBtn_Click(object sender, EventArgs e)
        {
            toggleFowlBtn.Text = toggleFowlBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleMAWBtn_Click(object sender, EventArgs e)
        {
            toggleMAWBtn.Text = toggleMAWBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleCatchLeprechaunBtn_Click(object sender, EventArgs e)
        {
            toggleCatchLeprechaunBtn.Text = toggleCatchLeprechaunBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleSeasonalDblBtn_Click(object sender, EventArgs e)
        {
            toggleSeaonalDblBtn.Text = toggleSeaonalDblBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleScavengerHuntBtn_Click(object sender, EventArgs e)
        {
            toggleScavengerHuntBtn.Text = toggleScavengerHuntBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleParchmentMaskBtn_Click(object sender, EventArgs e)
        {
            toggleParchmentMaskBtn.Text = toggleParchmentMaskBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void toggleCandyTreatsBtn_Click(object sender, EventArgs e)
        {
            toggleCandyTreatsBtn.Text = toggleCandyTreatsBtn.Text == "Enable" ? "Disable" : "Enable";

        }

        private void btnResetAllStatus_Click(object sender, EventArgs e)
        {
            _client.IsRefreshingData = 0;
            _client.NeedsToRepairHammer = false;
            _client.IsCasting = false;
            _client.IsWalking = false;
            _client.IsBashing = false;
            _client.HasWalked = false;
            _client.Map.CanUseSpells = true;
            _client.SpellHistory.Clear();
        }

        private void EscapeSay(object sender, KeyPressEventArgs k)
        {
            if (k.KeyChar == '\u001B')
            {
                chatBox.Text = "";
                chatBox.MaxLength = 65 - _client.Name.Length;
                k.Handled = true;
            }
            else
            {
                if (k.KeyChar != '\r')
                    return;
                _client.PublicMessage(0, chatBox.Text);
                chatBox.MaxLength = 65 - _client.Name.Length;
                chatBox.Text = "";
            }
        }

        private void WhispShout(object sender, KeyEventArgs k)
        {
            if (k.KeyCode == Keys.OemQuotes && k.Shift)
            {
                chatBox.KeyPress -= new KeyPressEventHandler(EscapeSay);
                chatBox.KeyDown -= new KeyEventHandler(WhispShout);
                if (!string.IsNullOrWhiteSpace(chatBox.Text))
                    return;
                chatBox.MaxLength = 67;
                chatBox.ForeColor = Color.Magenta;
                if (!string.IsNullOrWhiteSpace(_recentWhispers[0]))
                {
                    chatBox.Text = "to [" + _recentWhispers[0] + "]? ";
                    chatBox.KeyDown += new KeyEventHandler(WhispUpDown);
                    _whispIndex = 0;
                    k.Handled = true;
                    k.SuppressKeyPress = true;
                }
                else
                    chatBox.Text = "to []? ";
                k.Handled = true;
                k.SuppressKeyPress = true;
                chatBox.KeyPress += new KeyPressEventHandler(WhispTargetEnter);
                chatBox.SelectionStart = chatBox.Text.Length;
            }
            else
            {
                if (k.KeyCode != Keys.D1 || !k.Shift)
                    return;
                chatBox.KeyPress -= new KeyPressEventHandler(EscapeSay);
                chatBox.ForeColor = Color.Red;
                chatBox.MaxLength = 67;
                chatBox.Text = _client.Name + "! ";
                k.Handled = true;
                k.SuppressKeyPress = true;
                chatBox.KeyDown -= new KeyEventHandler(WhispShout);
                chatBox.KeyPress += new KeyPressEventHandler(ShoutEnter);
                chatBox.SelectionStart = chatBox.Text.Length;
            }
        }

        private void ShoutEnter(object sender, KeyPressEventArgs k)
        {
            if (k.KeyChar == '\u001B')
            {
                chatBox.KeyPress -= new KeyPressEventHandler(ShoutEnter);
                chatBox.MaxLength = 65 - _client.Name.Length;
                chatBox.KeyDown += new KeyEventHandler(WhispShout);
                chatBox.KeyPress += new KeyPressEventHandler(EscapeSay);
                chatBox.ForeColor = Color.Black;
                chatBox.Text = "";
                k.Handled = true;
            }
            else
            {
                if (k.KeyChar != '\r')
                    return;
                _client.PublicMessage(1, chatBox.Text.Replace(_client.Name + "! ", ""));
                chatBox.Text = "";
                chatBox.ForeColor = Color.Black;
                k.Handled = true;
                chatBox.KeyPress -= new KeyPressEventHandler(ShoutEnter);
                chatBox.KeyDown += new KeyEventHandler(WhispShout);
                chatBox.KeyPress += new KeyPressEventHandler(EscapeSay);
                chatBox.SelectionStart = chatBox.Text.Length;
            }
        }

        private void WhispUpDown(object sender, KeyEventArgs k)
        {
            if (k.KeyCode == Keys.Up)
            {
                string[] strArray = chatBox.Text.Replace("to [", "").Replace("]?", "").Split(new char[1]
                {
          ' '
                }, 2);
                while (string.IsNullOrWhiteSpace(_recentWhispers[_whispIndex]))
                {
                    ++_whispIndex;
                    if (_whispIndex > 4)
                        _whispIndex = 0;
                }
                chatBox.Text = "to [" + strArray[0] + "]? " + _recentWhispers[_whispIndex];
                ++_whispIndex;
                if (_whispIndex > 4)
                    _whispIndex = 0;
                k.Handled = true;
                k.SuppressKeyPress = true;
                chatBox.SelectionStart = chatBox.Text.Length;
            }
            else
            {
                if (k.KeyCode != Keys.Down)
                    return;
                string[] strArray = chatBox.Text.Replace("to [", "").Replace("]?", "").Split(new char[1]
                {
          ' '
                }, 2);
                while (string.IsNullOrWhiteSpace(_recentWhispers[_whispIndex]))
                {
                    --_whispIndex;
                    if (_whispIndex < 0)
                        _whispIndex = 4;
                }
                chatBox.Text = "to [" + strArray[0] + "]? " + _recentWhispers[_whispIndex];
                --_whispIndex;
                if (_whispIndex < 0)
                    _whispIndex = 4;
                k.Handled = true;
                chatBox.SelectionStart = chatBox.Text.Length;
            }
        }

        private void WhispTargetEnter(object sender, KeyPressEventArgs k)
        {
            if (k.KeyChar == '\u001B')
            {
                chatBox.KeyPress -= new KeyPressEventHandler(WhispTargetEnter);
                chatBox.KeyDown -= new KeyEventHandler(WhispUpDown);
                chatBox.MaxLength = 65 - _client.Name.Length;
                chatBox.KeyDown += new KeyEventHandler(WhispShout);
                chatBox.KeyPress += new KeyPressEventHandler(EscapeSay);
                chatBox.ForeColor = Color.Black;
                chatBox.Text = "";
                k.Handled = true;
            }
            else
            {
                if (k.KeyChar != '\r')
                    return;
                string text = chatBox.Text;
                if (chatBox.Text.Contains("to []? "))
                    _lastWhispered = text.Replace("to []? ", "");
                else if (chatBox.Text.Contains("to [" + _recentWhispers[0] + "]? ") && chatBox.Text.Length == _recentWhispers[0].Length + 7)
                {
                    _lastWhispered = text.Replace("to [", "").Replace("]? ", "");
                }
                else
                {
                    string[] strArray = text.Replace("to [", "").Replace("]?", "").Split(new char[1]
                    {
            ' '
                    }, 2);
                    _lastWhispered = (strArray[0] == strArray[1]) ? strArray[0] : strArray[1];
                }
                chatBox.Text = "- > " + _lastWhispered + ": ";
                k.Handled = true;
                chatBox.KeyDown -= new KeyEventHandler(WhispUpDown);
                chatBox.KeyPress -= new KeyPressEventHandler(WhispTargetEnter);
                chatBox.KeyPress += new KeyPressEventHandler(WhispMessageEnter);
                chatBox.SelectionStart = chatBox.Text.Length;
            }
        }

        private void WhispMessageEnter(object sender, KeyPressEventArgs k)
        {
            if (k.KeyChar == '\u001B')
            {
                chatBox.KeyPress -= new KeyPressEventHandler(WhispMessageEnter);
                chatBox.MaxLength = 65 - _client.Name.Length;
                chatBox.KeyDown += new KeyEventHandler(WhispShout);
                chatBox.KeyPress += new KeyPressEventHandler(EscapeSay);
                chatBox.ForeColor = Color.Black;
                chatBox.Text = "";
                k.Handled = true;
            }
            else
            {
                if (k.KeyChar != '\r')
                    return;
                _client.Whisper(_lastWhispered, chatBox.Text.Replace("- > " + _lastWhispered + ": ", ""));
                k.Handled = true;
                chatBox.KeyDown += new KeyEventHandler(WhispShout);
                chatBox.KeyPress -= new KeyPressEventHandler(WhispMessageEnter);
                chatBox.KeyPress += new KeyPressEventHandler(EscapeSay);
                chatBox.ForeColor = Color.Black;
                chatBox.Text = "";
            }
        }



        private void ChatBox_Enter(object sender, EventArgs e)
        {
            chatBox.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void ChatBox_Leave(object sender, EventArgs e) => chatBox.BackColor = Color.White;

        private void btnBashing_Click(object sender, EventArgs e)
        {
            if (btnBashingNew.Text == "Start Bashing")
            {
                btnBashingNew.Text = "Stop Bashing";
                btnBashingNew.Image = Resources.grumblade;
                btnBashingNew.ImageAlign = ContentAlignment.MiddleLeft;
                chkBashAsgall.Enabled = true;
            }
            else
            {
                btnBashingNew.Text = "Start Bashing";
                btnBashingNew.Image = Resources.bruneblade;
                btnBashingNew.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void priorityAddBtn_Click(object sender, EventArgs e)
        {
            if (!ushort.TryParse(priorityTBox.Text, out ushort parsedPriority) || parsedPriority < 1 || parsedPriority > 1000)
            {
                MessageDialog.Show(_client.Server.MainForm, "Your sprite must be a number between 1-1000");
                priorityTBox.Clear();
                return;
            }

            string priorityStr = parsedPriority.ToString();

            if (priorityLBox.Items.Contains(priorityStr))
            {
                MessageDialog.Show(_client.Server.MainForm, "\tEnemy already in list\t");
                priorityTBox.Clear();
                return;
            }

            priorityLBox.Items.Add(priorityStr);
            priorityTBox.Clear();
        }

        private void priorityRemoveBtn_Click(object sender, EventArgs e)
        {
            while (priorityLBox.SelectedItems.Count > 0)
                priorityLBox.Items.RemoveAt(priorityLBox.SelectedIndex);
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            _client.RefreshRequest();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            _client.RefreshRequest(false);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _client.Assail();
        }

        public void ascendBtn_Click(object sender, EventArgs e)
        {
            _client.AscendTaskDone = false;
            _client.WarBagDeposited = false;
            if (ascendBtn.Text == "Ascend")
            {
                ascendBtn.Text = "Ascending";
            }
            else
            {
                ascendBtn.Text = "Ascend";
            }
        }

        private void killerNameTbx_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(killerNameTbx.Text))
            {
                deathOptionCbx.Checked = false;
                useKillerCbx.Checked = true;
            }
            else
            {
                useKillerCbx.Checked = false;

            }
        }

        private void useKillerCbx_CheckedChanged(object sender, EventArgs e)
        {
            if (useKillerCbx.Checked)
            {
                killerNameTbx.Enabled = true;
                deathOptionCbx.Checked = false;
            }
            else
            {
                killerNameTbx.Enabled = false;
            }
        }

        private void deathOptionCbx_CheckedChanged(object sender, EventArgs e)
        {
            if (deathOptionCbx.Checked)
            {
                useKillerCbx.Checked = false;
                killerNameTbx.Enabled = false;
            }
        }

        private void ComboButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            // Determine if the combo is being enabled or disabled
            bool isEnabling = button.Text.Contains("Enable Combo");
            string comboNumber = button.Text.Split(' ').Last();

            // Update the button text
            button.Text = $"{(isEnabling ? "Disable" : "Enable")} Combo {comboNumber}";

            // Update the corresponding combo state
            switch (comboNumber)
            {
                case "Four":
                    _client.comboFour = isEnabling;
                    break;
                case "Three":
                    _client.comboThree = isEnabling;
                    break;
                case "Two":
                    _client.comboTwo = isEnabling;
                    break;
                case "One":
                    _client.comboOne = isEnabling;
                    break;
            }
        }


        private void exampleComboBtn_Click(object sender, EventArgs e)
        {
            combo1List.AppendText("Cast Gentle Touch");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Sleep 1000");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Cyclone Kick");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Necklace");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Wheel Kick 7");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Hemloch");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Crasher");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Rambutan");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Remove Weapon");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Remove Shield");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Mantis Kick");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Yowien Hatchet");
            combo1List.AppendText(Environment.NewLine);
            combo1List.AppendText("Great Yowien Shield");
        }
    }
}

