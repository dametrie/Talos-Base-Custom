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

        private Client _client;
        private bool _isBotRunning = false;
        private string textMaptext = string.Empty;
        private string textXtext = string.Empty;
        private string textYtext = string.Empty;
        private string LastLoadedProfile = string.Empty;
        private short textMap;
        private short textX;
        private short testY;
        internal WayForm _wayForm = null;

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

        private List<string> _chatPanelList = new List<string>
        {
            "",
            "",
            "",
            "",
            ""
        };
        internal List<string> _unmaxedBashingSkills = new List<string>();
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
        internal Palette256 palette256;
        internal bool _isBashing;
        internal bool _isLoading;
        private string waypointsPath;
        private System.Windows.Forms.Timer mushroomBonusCooldownTimer;

        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            _wayForm = new WayForm(_client);
            UIHelper.Initialize(_client);
            InitializeComponent();

            _wayForm.savedWaysLBox.DataSource = _wayFormProfiles;
            worldObjectListBox.DataSource = client._worldObjectBindingList;
            creatureHashListBox.DataSource = client._creatureBindingList;
            strangerList.DataSource = client._strangerBindingList;
            friendList.DataSource = client._friendBindingList;
            trashList.DataSource = _trashToDrop;

            profilePath = Settings.Default.DarkAgesPath.Replace("Darkages.exe", client.Name + "\\Talos");
            waypointsPath = AppDomain.CurrentDomain.BaseDirectory + "waypoints";
            setoaArchive = DATArchive.FromFile(Settings.Default.DarkAgesPath.Replace("Darkages.exe", "setoa.dat"));
            spellImageArchive = EPFImage.FromArchive("spell001.epf", setoaArchive);
            skillImageArchive = EPFImage.FromArchive("skill001.epf", setoaArchive);
            palette256 = Palette256.FromArchive("gui06.pal", setoaArchive);

            OnlyDisplaySpellsWeHave();
            AddClientToFriends();
            SetupInitialClientHacks();
        }

        private void ClientTab_Load(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke((Action)delegate { ClientTab_Load(sender, e); }); return; }
            
            ThreadPool.QueueUserWorkItem(delegate
            {
                _client.CheckNetStat();
            });

            
            _client._spellTimer.Start();

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
                    File.WriteAllLines(text + "\\inventory.txt", _client.Inventory.Select((Item item) => item.Name));
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
                    UpdateBindingList(_wayFormProfiles, _wayForm.savedWaysLBox, itemToAdd);
                }
            }
        }


        private void AddClientToFriends()
        {
            if (!_client._friendBindingList.Contains(_client.Name))
            {
                UpdateBindingList(_client._friendBindingList, friendList, _client.Name);
                _client._strangerBindingList.Remove(_client.Name);
            }
        }

        private void OnlyDisplaySpellsWeHave()
        {
            UIHelper.SetupComboBox(healCombox, new[] { "nuadhaich", "ard ioc", "mor ioc", "ioc", "beag ioc", "Cold Blood", "Spirit Essence" }, healCbox, healPctNum);
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
            string medeniaClass = _client._medeniaClass.ToString();

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
            string temuairClass = _client._temuairClass.ToString();

            switch (temuairClass)
            {
                case "Rogue":
                    SetControlVisibility(hideCbox, "Hide", spellBased: true);
                    break;
                case "Warrior":
                    if (_client._previousClass.ToString() == "Pure")
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
            string previousClass = _client._previousClass.ToString();

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
            if (_client._previousClass.ToString() == "Pure" && _client.Skillbook[skillName] != null)
                control.Visible = true;
        }
        private void SetPureClassSpells(Control control, string spellName)
        {
            if (_client._previousClass.ToString() == "Pure" && _client.Spellbook[spellName] != null)
                control.Visible = true;
        }
        private void SetPureClassItems(Control control, string itemName)
        {
            if (_client._previousClass.ToString() == "Pure" && _client.HasItem(itemName))
                control.Visible = true;
        }

        private void SetPureClassSpells(Control[] controls, string[] spellNames)
        {
            if (_client._previousClass.ToString() == "Pure")
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
            if (_client._previousClass.ToString() == "Pure" && _client.Spellbook["Lyliac Vineyard"] != null)
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

        internal void RefreshUnmaxedSpells(object sender, EventArgs e)
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

        internal void RefreshUnmaxedSkills(object sender, EventArgs e)
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
        internal void RefreshUnmaxedBashingSkills(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.Name;
            if (button.BackColor == Color.DodgerBlue)
            {
                button.BackColor = Color.White;
                _unmaxedBashingSkills.Remove(name);
            }
            else
            {
                button.BackColor = Color.DodgerBlue;
                _unmaxedBashingSkills.Add(name);
            }
        }
        internal void RenderUnmaxedSkills(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(skillImageArchive[index], palette256);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name,
                Name = name + "whatever"
            };
            textBox.Width = (int)((double)TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
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
            button.Click += RefreshUnmaxedSkills;
            unmaxedSkillsGroup.Controls.Add(button);
            unmaxedSkillsGroup.Controls.Add(textBox);
        }
        internal void RenderUnmaxedBashingSkills(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(skillImageArchive[index], palette256);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name,
                Name = name + "whatever"
            };
            textBox.Width = (int)((double)TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
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
            button.Click += RefreshUnmaxedBashingSkills;
            bashingSkillsToUseGrp.Controls.Add(button);
            bashingSkillsToUseGrp.Controls.Add(textBox);
        }

        internal void RenderUnmaxedSpells(string name, ushort index, System.Drawing.Point point)
        {
            Bitmap image = DAGraphics.RenderImage(spellImageArchive[index], palette256);
            Button button = new Button();
            TextBox textBox = new TextBox
            {
                Visible = false,
                Text = name
            };
            textBox.Width = (int)((double)TextRenderer.MeasureText(textBox.Text, textBox.Font).Width * 1.2);
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
            button.Click += RefreshUnmaxedSpells;
            unmaxedSpellsGroup.Controls.Add(button);
            unmaxedSpellsGroup.Controls.Add(textBox);
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
            if (chatPanel.TextLength != 0)
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
            _ = chatPanel.Text.Length;
            bool atBottom = chatPanel.IsScrollBarAtBottom();
            SetChatPanelTextColor(color);
            if (!string.IsNullOrWhiteSpace(message))
            {
                chatPanel.AppendText(message);
            }
            SetChatPanelTextColor(chatPanel.ForeColor);
            if (atBottom)
            {
                chatPanel.ScrollToCaret();
            }
        }
        internal void UpdateChatPanel(string input)
        {
            // Use pattern matching to extract the first match from the input string
            Match match = Regex.Match(input, @"(\[!|<!|[a-zA-Z]+)");

            // If there is no match, return early
            if (!match.Success)
            {
                return;
            }

            // Extract the matched text
            string text = match.Groups[1].Value;

            // Replace specific patterns with their corresponding replacements
            text = text switch
            {
                "[!" => "!!",
                "<!" => "!",
                _ => text
            };

            // Update the chat panel list based on the extracted text
            if (_chatPanelList.Contains(text))
            {
                int index = _chatPanelList.IndexOf(text);
                ShiftListItemsDown(index);
                _chatPanelList[0] = text;
            }
            else
            {
                ShiftListItemsDown(_chatPanelList.Count - 1);
                _chatPanelList[0] = text;
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
            chatPanel.SelectionStart = chatPanel.TextLength;
            chatPanel.SelectionLength = 0;
            chatPanel.SelectionColor = color;
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
            _client.DialogOn = toggleDialogBtn.Checked;
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
            if (_client._isRefreshing == 1)
            {
                button7.Text = "Refresh";
                _client._isRefreshing = 0;
            }
            else
            {
                button7.Text = "Stop Refreshing";
                _client._isRefreshing = 1;
            }   
        }

        private void formCbox_CheckedChanged(object sender, EventArgs e)
        {
            _client.InMonsterForm = (sender as CheckBox).Checked;
            _client.DisplayAisling(_client.Player);
        }
        private void formNum_ValueChanged(object sender, EventArgs e)
        {
            _client._monsterFormID = (ushort)(sender as NumericUpDown).Value;
            if (formCbox.Checked)
            {
                _client.DisplayAisling(_client.Player);
            }
        }

        private void deformCbox_CheckedChanged(object sender, EventArgs e)
        {

            if (deformCbox.Checked)
                _client._deformNearStrangers = true;
            else
                _client._deformNearStrangers = false;

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
            mapInfoInfoLbl.Text = "Pt:" + _client._serverLocation.Point.ToString() + "  Size: " + map.Width + "x" + map.Height + "  ID: " + map.MapID;
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
                    while (_client._clientLocation != targetLocation)
                    {
                        // Safely update the UI or check conditions that involve UI elements

                        Invoke((MethodInvoker)delegate
                        {
                            // Example of checking a condition or updating UI
                            shouldContinue = (_client._clientLocation != targetLocation);
                        });

                        if (!shouldContinue)
                            break;

                        _client.RouteFind(targetLocation);


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
                _client.RouteFind(targetLocation);

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _client.RemoveShield();
        }

        private void walkSpeedSldr_Scroll(object sender, EventArgs e)
        {
            walkSpeedLbl.Text = (sender as TrackBar).Value.ToString();
            _client._walkSpeed = walkSpeedSldr.Value;
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
                try
                {
                    bool isChargeSkillUsedRecently = _client.HasSkill("Charge") && (DateTime.UtcNow - _client.Skillbook["Charge"].LastUsed).TotalMilliseconds < 1500.0;
                    bool isSprintPotionUsedRecently = _client.HasItem("Sprint Potion") && (DateTime.UtcNow - _client.Inventory.HasItemReturnSlot("Sprint Potion").LastUsed).TotalMilliseconds < 1500.0;

                    if (!isChargeSkillUsedRecently && !isSprintPotionUsedRecently)
                    {
                        HashSet<string> nearbyPlayers = new HashSet<string>(_client.GetNearbyPlayers().Select(player => player.Name), StringComparer.CurrentCultureIgnoreCase);
                        HashSet<string> nonStrangers = new HashSet<string>(_client.GroupedPlayers.Concat(_client._friendBindingList), StringComparer.CurrentCultureIgnoreCase);

                        foreach (string name in new List<string>(_client._strangerBindingList))
                        {
                            if (nonStrangers.Contains(name, StringComparer.CurrentCultureIgnoreCase) || !nearbyPlayers.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                            {
                                _client._strangerBindingList.Remove(name);
                            }
                        }
                        foreach (string name in nearbyPlayers)
                        {
                            if (!nonStrangers.Contains(name, StringComparer.CurrentCultureIgnoreCase) && !_client._strangerBindingList.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                            {
                                UpdateBindingList(_client._strangerBindingList, strangerList, name);
                            }
                        }
                        foreach (string name in new List<string>(_client._strangerBindingList))
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                _client.DictLastSeen[name] = DateTime.UtcNow;
                            }
                        }
                        _client.DictLastSeen = _client.DictLastSeen.OrderByDescending((KeyValuePair<string, DateTime> kvp) => kvp.Value).Take(5).ToDictionary((KeyValuePair<string, DateTime> kvp) => kvp.Key, (KeyValuePair<string, DateTime> kvp) => kvp.Value);
                    }
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

            _client._staffList.Clear();
            _client.LoadStavesAndBows();
            _unmaxedSkills.Clear();
            _unmaxedSpells.Clear();

            setoaArchive = DATArchive.FromFile(Settings.Default.DarkAgesPath.Replace("Darkages.exe", "setoa.dat"));
            spellImageArchive = EPFImage.FromArchive("spell001.epf", setoaArchive);
            skillImageArchive = EPFImage.FromArchive("skill001.epf", setoaArchive);
            palette256 = Palette256.FromArchive("gui06.pal", setoaArchive);

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

            if (_client.ClientTab.startStrip.Text == "Stop")
            {
                _client.BotBase.Start();
            }
            OnlyDisplaySpellsWeHave();
            SetClassSpecificSpells();
        }

        private void addAislingBtn_Click(object sender, EventArgs e)
        {
            string name = addAislingText.Text;
            if (ParseAllyTextBox(name))
            {
                AddAllyPage(name);
                addAislingText.Clear();
                if (MessageDialog.Show(_client._server._mainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
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
                MessageDialog.Show(_client._server._mainForm, "Your ally target cannot be empty.");
                return false;
            }
            if (!name.All(char.IsLetter))
            {
                MessageDialog.Show(_client._server._mainForm, "Your ally target cannot contain noncharacters.");
                return false;
            }
            if (_client.Bot.IsAllyAlreadyListed(name))
            {
                MessageDialog.Show(_client._server._mainForm, "Ally already in list.");
                return false;
            }
            if (name.Equals(_client.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageDialog.Show(_client._server._mainForm, "Cannot add yourself to ally list.");
                return false;
            }
            return true;
        }

        internal void AddAllyPage(string name, Image image = null)
        {
            if (_client.Bot.IsAllyAlreadyListed(name))
            {
                return;
            }

            Ally ally = new Ally(name);
            ally.AllyPage = new AllyPage(ally, _client);

            TabPage tabPage = new TabPage(ally.ToString())
            {
                Name = name
            };
            tabPage.Controls.Add(ally.AllyPage);
            ally.AllyPage.allypictureCharacter.Image = image;

            aislingTabControl.TabPages.Add(tabPage);
            UpdateNearbyAllyTable(name);
            RefreshNearbyAllyTable();
            _client.Bot.UpdateAllyList(ally);

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
            _client._groupBindingList = new BindingList<string>(_client.GroupedPlayers.ToList());
            groupList.DataSource = _client._groupBindingList;
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



        private async void startStrip_Click_1(object sender, EventArgs e)
        {
            if (startStrip.Text == "Start")
            {
                startStrip.Text = "Stop";
                if (_client.BotBase != _client.Bot)
                {
                    _client.BotBase = _client.Bot;
                    _client.BotBase.Client = _client;
                    _client.BotBase.Server = _client._server;
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
                _client._isWalking = false;
                _client._isCasting = false;
                _client.Bot._dontWalk = false;
                _client.Bot._dontCast = false;
                _client._exchangeOpen = false;
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
                UpdateBindingList(_client._friendBindingList, friendList, selectedItem);
                _client._strangerBindingList.Remove(selectedItem);             
            }
            //Adam add method to save friends to file. 
            //Load it on startup
        }

        private void removeFriendBtn_Click(object sender, EventArgs e)
        {
            foreach (string seletedItem in friendList.SelectedItems.OfType<string>().ToList())
            {
                UpdateBindingList(_client._strangerBindingList, strangerList, seletedItem);
                _client._friendBindingList.Remove(seletedItem);
                if (_client.DictLastSeen.ContainsKey(seletedItem))
                {
                    _client.DictLastSeen.Remove(seletedItem);
                }
                UpdateStrangerList();
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
            foreach (Client client in _client._server._clientList)
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
                if (!_client._friendBindingList.Contains(selectedItem))
                {
                    UpdateBindingList(_client._friendBindingList, friendList, selectedItem);
                }
            }
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
            BindingList<string> friends = _client._friendBindingList;
            foreach (Client client in _client._server._clientList)
            {
                if (!friends.Contains(client.Name) && _client != client)
                {
                    UpdateBindingList(friends, friendList, client.Name);
                }
                if (_client._strangerBindingList.Contains(client.Name))
                {
                    _client._strangerBindingList.Remove(client.Name);
                }
            }
        }

        internal void RemoveAllyPage()
        {
            foreach (string name in _client.GroupedPlayers)
            {
                if (_client.Bot.AllyPage == null || _client._server.GetClient(name) == null)
                {
                    if (_client.Bot.IsAllyAlreadyListed(name))
                    {
                        if (aislingTabControl.TabPages.ContainsKey(name))
                        {
                            aislingTabControl.TabPages[name].Dispose();
                        }
                        _client.Bot.RemoveAlly(name);
                    }
                    Ally ally = new Ally(name)
                    {
                        AllyPage = _client.Bot.AllyPage
                    };
                    _client.Bot.UpdateAllyList(ally);
                }
            }
        }

        private void addGroupBtn_Click(object sender, EventArgs e)
        {
            if (_client.Bot.AllyPage != null)
            {
                MessageDialog.Show(_client._server._mainForm, "You are already targeting the group.");
                return;
            } 

            _client.Bot.AllyPage = new AllyPage(new Ally("group"), _client);
            _client.Bot.AllyPage.allyMDCRbtn.Visible = true;
            _client.Bot.AllyPage.allyMDCSpamRbtn.Visible = true;
            _client.Bot.AllyPage.allyMICSpamRbtn.Visible = true;
            _client.Bot.AllyPage.allyNormalRbtn.Visible = true;
            TabPage tabPage = new TabPage("group");
            tabPage.Controls.Add(_client.Bot.AllyPage);
            aislingTabControl.TabPages.Add(tabPage);
            foreach (string name in _client.GroupedPlayers)
            {
                if (_client.Bot.AllyPage == null || _client._server.GetClient(name) == null)
                {
                    if (_client.Bot.IsAllyAlreadyListed(name))
                    {
                        aislingTabControl.TabPages[name]?.Dispose();
                        _client.Bot.RemoveAlly(name);
                    }
                    Ally ally = new Ally(name)
                    {
                        AllyPage = _client.Bot.AllyPage
                    };
                    _client.Bot.UpdateAllyList(ally);
                }
            }
            if (MessageDialog.Show(_client._server._mainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
            {
                aislingTabControl.SelectTab(tabPage);
                clientTabControl.SelectTab(1);
            }
        }

        private void addAltsBtn_Click(object sender, EventArgs e)
        {
            if (_client.Bot.AllyPage != null)
            {
                MessageDialog.Show(_client._server._mainForm, "You already targeting alts.");
                return;
            }
            _client.Bot.AllyPage = new AllyPage(new Ally("alts"), _client);
            TabPage tabPage = new TabPage("alts");
            tabPage.Controls.Add(_client.Bot.AllyPage);
            aislingTabControl.TabPages.Add(tabPage);
            foreach (Client client in _client._server._clientList.Where((Client c) => c._identifier != _client._identifier))
            {
                if (_client.Bot.IsAllyAlreadyListed(client.Name))
                {
                    aislingTabControl.TabPages[client.Name]?.Dispose();
                    _client.Bot.RemoveAlly(client.Name);

                }
                Ally ally = new Ally(client.Name)
                {
                    AllyPage = _client.Bot.AllyPage
                };
                _client.Bot.UpdateAllyList(ally);
            }
            if (MessageDialog.Show(_client._server._mainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
            {
                aislingTabControl.SelectTab(tabPage);
                clientTabControl.SelectTab(1);
            }
        }

        private void addAllMonstersBtn_Click(object sender, EventArgs e)
        {
            if (_client.Bot.AllMonsters != null)
            {
                MessageDialog.Show(_client._server._mainForm, "Enemy already in list");
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
            if ( MessageDialog.Show(_client._server._mainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
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
                if (MessageDialog.Show(_client._server._mainForm, "Successfully Added!\nGo to it?") == DialogResult.OK)
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
                MessageDialog.Show(_client._server._mainForm, "Enemy already in list");
                addMonsterText.Clear();
                return false;
            }
            MessageDialog.Show(_client._server._mainForm, "Your sprite must be a number between 1-1000");
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
                MessageDialog.Show(_client._server._mainForm, "Cannot use characters /*\"[]\\:?|<>.");
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
                // ComboBox List items (lines)
                Combo1ListItems = new List<string>(combo1List.Lines),
                Combo2ListItems = new List<string>(combo2List.Lines),
                Combo3ListItems = new List<string>(combo3List.Lines),
                Combo4ListItems = new List<string>(combo4List.Lines),

                // Button text
                Combo1BtnText = combo1Btn.Text,
                Combo2BtnText = combo2Btn.Text,
                Combo3BtnText = combo3Btn.Text,
                Combo4BtnText = combo4Btn.Text,

                // CheckBox states
                DontCbox1Checked = dontCbox1.Checked,
                DontCBox2Checked = dontCBox2.Checked,
                DontCBox3Checked = dontCBox3.Checked,
                DontCBox4Checked = dontCBox4.Checked,

                DoublesComboxText = doublesCombox.Text,
                AutoDoubleCboxChecked = autoDoubleCbox.Checked,
                ExpGemsComboxText = expGemsCombox.Text,
                AutoGemCboxChecked = autoGemCbox.Checked,
                AutoStaffCboxChecked = autoStaffCbox.Checked,
                HideLinesCboxChecked = hideLinesCbox.Checked,
                FormCboxChecked = formCbox.Checked,
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
                SafeFSCboxChecked = safeFSCbox.Checked,
                EquipmentRepairCboxChecked = equipmentrepairCbox.Checked,
                RangerLogCboxChecked = rangerLogCbox.Checked,
                //GMLogCBoxChecked = gmLogCBox.Checked,
                //ChkGMSoundsChecked = chkGMSounds.Checked,
                DeformCBoxChecked = deformCbox.Checked,
                //ChkTavWallHacksChecked = chkTavWallHacks.Checked,
                //ChkTavWallStrangerChecked = chkTavWallStranger.Checked,
                ChkLastStepF5Checked = chkLastStepF5.Checked,
                //ChkAltLoginChecked = chkAltLogin.Checked,
                ChkSpeedStrangersChecked = chkSpeedStrangers.Checked,
                LockstepCboxChecked = lockstepCbox.Checked,
                ChkWaitForFasChecked = chkWaitForFas.Checked,
                ChkWaitForCradhChecked = chkWaitForCradh.Checked,
                ChkFrostStrikeChecked = chkFrostStrike.Checked,
                ChkUseSkillsFromRangeChecked = chkUseSkillsFromRange.Checked,
                ChargeToTargetCbxChecked = ChargeToTargetCbx.Checked,
                AssistBasherChkChecked = assistBasherChk.Checked,

                // TextBox values
                FasSpioradText = fasSpioradText.Text,
                VineText = vineText.Text,
                LeadBasherTxt = leadBasherTxt.Text,

                // NumericUpDown values
                FormNumValue = formNum.Value,
                DionPctNumValue = dionPctNum.Value,
                HealPctNumValue = healPctNum.Value,
                OverrideDistanceNumValue = overrideDistanceNum.Value,
                NumAssitantStrayValue = numAssitantStray.Value,
                //NumCrasherHealthValue = numCrasherHealth.Value,
                //NumExHealValue = numExHeal.Value,
                //NumBashSkillDelayValue = numBashSkillDelay.Value,
                //NumSkillIntValue = numSkillInt.Value,
                //PingCompensationNum1Value = pingCompensationNum1.Value,
                //MonsterWalkIntervalNum1Value = monsterWalkIntervalNum1.Value,
                //AtkRangeNumValue = atkRangeNum.Value,
                //EngageRangeNumValue = engageRangeNum.Value,
                FollowDistanceNumValue = followDistanceNum.Value,
                WalkSpeedSldrValue = walkSpeedSldr.Value,
                NumLastStepTimeValue = numLastStepTime.Value,
                NumPFCounterValue = numPFCounter.Value,

                // ListBox items
                TrashList = new List<string>(_trashToDrop),
                OverrideList = new List<string>(overrideList.Items.Cast<string>()),

                // ComboBox text
                DionComboxText = dionCombox.Text,
                DionWhenComboxText = dionWhenCombox.Text,
                AiteComboxText = aiteCombox.Text,
                HealComboxText = healCombox.Text,
                FasComboxText = fasCombox.Text,
                VineComboxText = vineCombox.Text,
            };

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

        internal async Task LoadProfileAsync(string profilePath)
        {
            await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(profilePath))
                {
                    return;
                }

                // Locking mechanism to prevent concurrent loading
                while (true)
                {
                    lock (this)
                    {
                        if (!_isLoading)
                        {
                            _isLoading = true; // Prevent multiple loads at the same time
                            break;
                        }
                    }
                    await Task.Delay(100); // Delay to avoid CPU overutilization
                }

                try
                {
                    // Set the last loaded profile
                    LastLoadedProfile = profilePath;

                    // Clear any existing options (implementation depending on your form)
                    ClearOptions(); // Assuming this clears current settings

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
                        this.Invoke((Action)(() =>
                        {

                            // Apply the deserialized values to the form's controls
                            // ComboBox List items (lines)
                            combo1List.Lines = formState.Combo1ListItems.ToArray();
                            combo2List.Lines = formState.Combo2ListItems.ToArray();
                            combo3List.Lines = formState.Combo3ListItems.ToArray();
                            combo4List.Lines = formState.Combo4ListItems.ToArray();

                            // Button text
                            combo1Btn.Text = formState.Combo1BtnText;
                            combo2Btn.Text = formState.Combo2BtnText;
                            combo3Btn.Text = formState.Combo3BtnText;
                            combo4Btn.Text = formState.Combo4BtnText;

                            // CheckBox states
                            dontCbox1.Checked = formState.DontCbox1Checked;
                            dontCBox2.Checked = formState.DontCBox2Checked;
                            dontCBox3.Checked = formState.DontCBox3Checked;
                            dontCBox4.Checked = formState.DontCBox4Checked;

                            doublesCombox.Text = formState.DoublesComboxText;
                            autoDoubleCbox.Checked = formState.AutoDoubleCboxChecked;
                            expGemsCombox.Text = formState.ExpGemsComboxText;
                            autoGemCbox.Checked = formState.AutoGemCboxChecked;
                            autoStaffCbox.Checked = formState.AutoStaffCboxChecked;
                            hideLinesCbox.Checked = formState.HideLinesCboxChecked;
                            formCbox.Checked = formState.FormCboxChecked;
                            optionsSkullCbox.Checked = formState.OptionsSkullCboxChecked;
                            optionsSkullSurrbox.Checked = formState.OptionsSkullSurrboxChecked;
                            oneLineWalkCbox.Checked = formState.OneLineWalkCboxChecked;
                            dionCbox.Checked = formState.DionCboxChecked;
                            healCbox.Checked = formState.HealCboxChecked;
                            deireasFaileasCbox.Checked = formState.DeireasFaileasCboxChecked;
                            aoSithCbox.Checked = formState.AoSithCboxChecked;
                            alertStrangerCbox.Checked = formState.AlertStrangerCboxChecked;
                            alertRangerCbox.Checked = formState.AlertRangerCboxChecked;
                            alertDuraCbox.Checked = formState.AlertDuraCboxChecked;
                            alertSkulledCbox.Checked = formState.AlertSkulledCboxChecked;
                            alertEXPCbox.Checked = formState.AlertEXPCboxChecked;
                            alertItemCapCbox.Checked = formState.AlertItemCapCboxChecked;
                            aiteCbox.Checked = formState.AiteCboxChecked;
                            fasCbox.Checked = formState.FasCboxChecked;
                            disenchanterCbox.Checked = formState.DisenchanterCboxChecked;
                            wakeScrollCbox.Checked = formState.WakeScrollCboxChecked;
                            hideCbox.Checked = formState.HideCboxChecked;
                            druidFormCbox.Checked = formState.DruidFormCboxChecked;
                            mistCbox.Checked = formState.MistCboxChecked;
                            armachdCbox.Checked = formState.ArmachdCboxChecked;
                            fasSpioradCbox.Checked = formState.FasSpioradCboxChecked;
                            beagCradhCbox.Checked = formState.BeagCradhCboxChecked;
                            aegisSphereCbox.Checked = formState.AegisSphereCboxChecked;
                            dragonScaleCbox.Checked = formState.DragonScaleCboxChecked;
                            manaWardCbox.Checked = formState.ManaWardCboxChecked;
                            regenerationCbox.Checked = formState.RegenerationCboxChecked;
                            perfectDefenseCbox.Checked = formState.PerfectDefenseCboxChecked;
                            dragonsFireCbox.Checked = formState.DragonsFireCboxChecked;
                            asgallCbox.Checked = formState.AsgallCboxChecked;
                            muscleStimulantCbox.Checked = formState.MuscleStimulantCboxChecked;
                            nerveStimulantCbox.Checked = formState.NerveStimulantCboxChecked;
                            monsterCallCbox.Checked = formState.MonsterCallCboxChecked;
                            vanishingElixirCbox.Checked = formState.VanishingElixirCboxChecked;
                            vineyardCbox.Checked = formState.VineyardCboxChecked;
                            autoRedCbox.Checked = formState.AutoRedCboxChecked;
                            fungusExtractCbox.Checked = formState.FungusExtractCboxChecked;
                            mantidScentCbox.Checked = formState.MantidScentCboxChecked;
                            aoSuainCbox.Checked = formState.AoSuainCboxChecked;
                            aoCurseCbox.Checked = formState.AoCurseCboxChecked;
                            aoPoisonCbox.Checked = formState.AoPoisonCboxChecked;
                            followCbox.Checked = formState.FollowCboxChecked;
                            bubbleBlockCbox.Checked = formState.BubbleBlockCboxChecked;
                            spamBubbleCbox.Checked = formState.SpamBubbleCboxChecked;
                            rangerStopCbox.Checked = formState.RangerStopCboxChecked;
                            pickupGoldCbox.Checked = formState.PickupGoldCboxChecked;
                            pickupItemsCbox.Checked = formState.PickupItemsCboxChecked;
                            dropTrashCbox.Checked = formState.DropTrashCboxChecked;
                            noBlindCbox.Checked = formState.NoBlindCboxChecked;
                            mapZoomCbox.Checked = formState.MapZoomCboxChecked;
                            seeHiddenCbox.Checked = formState.SeeHiddenCboxChecked;
                            ghostHackCbox.Checked = formState.GhostHackCboxChecked;
                            ignoreCollisionCbox.Checked = formState.IgnoreCollisionCboxChecked;
                            hideForegroundCbox.Checked = formState.HideForegroundCboxChecked;
                            mapFlagsEnableCbox.Checked = formState.MapFlagsEnableCboxChecked;
                            mapSnowCbox.Checked = formState.MapSnowCboxChecked;
                            mapTabsCbox.Checked = formState.MapTabsCboxChecked;
                            mapSnowTileCbox.Checked = formState.MapSnowTileCboxChecked;
                            unifiedGuildChatCbox.Checked = formState.UnifiedGuildChatCboxChecked;
                            toggleOverrideCbox.Checked = formState.ToggleOverrideCboxChecked;
                            safeFSCbox.Checked = formState.SafeFSCboxChecked;
                            equipmentrepairCbox.Checked = formState.EquipmentRepairCboxChecked;
                            rangerLogCbox.Checked = formState.RangerLogCboxChecked;
                            deformCbox.Checked = formState.DeformCBoxChecked;
                            //chkTavWallHacks.Checked = formState.ChkTavWallHacksChecked;
                            //chkTavWallStranger.Checked = formState.ChkTavWallStrangerChecked;
                            chkLastStepF5.Checked = formState.ChkLastStepF5Checked;
                            chkSpeedStrangers.Checked = formState.ChkSpeedStrangersChecked;
                            lockstepCbox.Checked = formState.LockstepCboxChecked;
                            chkWaitForFas.Checked = formState.ChkWaitForFasChecked;
                            chkWaitForCradh.Checked = formState.ChkWaitForCradhChecked;
                            chkFrostStrike.Checked = formState.ChkFrostStrikeChecked;
                            chkUseSkillsFromRange.Checked = formState.ChkUseSkillsFromRangeChecked;
                            ChargeToTargetCbx.Checked = formState.ChargeToTargetCbxChecked;
                            assistBasherChk.Checked = formState.AssistBasherChkChecked;

                            // TextBox values
                            fasSpioradText.Text = formState.FasSpioradText;
                            vineText.Text = formState.VineText;
                            leadBasherTxt.Text = formState.LeadBasherTxt;

                            // NumericUpDown values
                            formNum.Value = formState.FormNumValue;
                            dionPctNum.Value = formState.DionPctNumValue;
                            healPctNum.Value = formState.HealPctNumValue;
                            overrideDistanceNum.Value = formState.OverrideDistanceNumValue;
                            numAssitantStray.Value = formState.NumAssitantStrayValue;
                            followDistanceNum.Value = formState.FollowDistanceNumValue;
                            walkSpeedSldr.Value = (int)formState.WalkSpeedSldrValue;
                            numLastStepTime.Value = formState.NumLastStepTimeValue;
                            numPFCounter.Value = formState.NumPFCounterValue;

                            // ListBox items
                            _trashToDrop.Clear();
                            foreach (string item in formState.TrashList)
                            {
                                UpdateBindingList(_trashToDrop, trashList, item);
                            }
                            overrideList.Items.Clear();
                            foreach (string item in formState.OverrideList)
                            {
                                overrideList.Items.Add(item);
                            }

                            // ComboBox text
                            dionCombox.Text = formState.DionComboxText;
                            dionWhenCombox.Text = formState.DionWhenComboxText;
                            aiteCombox.Text = formState.AiteComboxText;
                            healCombox.Text = formState.HealComboxText;
                            fasCombox.Text = formState.FasComboxText;
                            vineCombox.Text = formState.VineComboxText;

                            // Commented out sections for future implementation (as per SaveProfile)
                            // GMLogCBoxChecked = formState.GMLogCBoxChecked;
                            // ChkGMSoundsChecked = formState.ChkGMSoundsChecked;
                            // NumCrasherHealthValue = formState.NumCrasherHealthValue;
                            // NumExHealValue = formState.NumExHealValue;
                            // NumBashSkillDelayValue = formState.NumBashSkillDelayValue;
                            // NumSkillIntValue = formState.NumSkillIntValue;
                            // PingCompensationNum1Value = formState.PingCompensationNum1Value;
                            // MonsterWalkIntervalNum1Value = formState.MonsterWalkIntervalNum1Value;
                            // AtkRangeNumValue = formState.AtkRangeNumValue;
                            // EngageRangeNumValue = formState.EngageRangeNumValue;
                        }));
                    });
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., file access issues, deserialization errors)
                    _client.ServerMessage(0, $"Error loading profile: {ex.Message}");
                }
                finally
                {
                    lock (this)
                    {
                        _isLoading = false; // Reset the loading flag
                    }
                }
            });
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
            ClearAllControls(this.Controls);

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
            btnBashing.Text = "Start Bashing";
            //btnBashingNew.Text = "Start Bashing";

            // Clear combo lists
            combo1List.Clear();
            combo2List.Clear();
            combo3List.Clear();
            combo4List.Clear();

            _client._walkSpeed = 150.0;

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

        private void spellBarIdsBtn_Click(object sender, EventArgs e)
        {

        }

        private void mapNodeIdsBtn_Click(object sender, EventArgs e)
        {

        }

        private void classDetectorBtn_Click(object sender, EventArgs e)
        {

        }

        private void pursuitIdsBtn_Click(object sender, EventArgs e)
        {

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
            if (MessageDialog.Show(_client._server._mainForm, "ARE YOU SURE YOU WANT TO DELETE " + text + "?") == DialogResult.OK)
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
            if (!string.IsNullOrEmpty(LastLoadedProfile) && MessageDialog.Show(_client._server._mainForm, "Save this profile as " + LastLoadedProfile + "?") == DialogResult.OK)
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

        private void startStrip_Click(object sender, EventArgs e)
        {

        }

        private void numComboImgSelect_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSkillCombo_Click(object sender, EventArgs e)
        {

        }

        private void btnResetAllStatus_Click(object sender, EventArgs e)
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

        }

        private void btnConLogin_Click(object sender, EventArgs e)
        {

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

        private void ascendBtn_Click(object sender, EventArgs e)
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
                expHourLbl.Text = "EXP/hr: " + Math.Truncate((double)_sessionExperience / _sessionExperienceStopWatch.Elapsed.TotalHours).ToString("N0");
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
                 apHourLbl.Text = "AP/hr: " + Math.Truncate((double)(_client.AbilityExperience - _sessionAbility) / _sessionAbilityStopWatch.Elapsed.TotalHours).ToString("N0");
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
                goldHourLbl.Text = "Gold/hr: " + Math.Truncate((double)(_client.Gold - _sessionGold) / _sessionGoldStopWatch.Elapsed.TotalHours).ToString("N0");
            }
        }

        private void safeScreenCbox_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                _client._safeScreen = true;
                noBlindCbox.Checked = false;
                seeHiddenCbox.Checked = false;
                mapZoomCbox.Checked = false;
                ghostHackCbox.Checked = false;
                mapFlagsEnableCbox.Checked = false;
                viewDMUCbox.Checked = false;
                toggleDmuCbox.Checked = false;
                //_client._server._mainForm.dressupDictionary.Clear();
                _client.RequestRefresh(false);
            }
            else
            {
                _client._safeScreen = false;
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
            _client.RequestRefresh();
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
            foreach (KeyValuePair<Structs.Point, Warp> exit in _client._map.Exits)
            {
                Structs.Point exitPoint = exit.Key; // The point where the warp exists on the map
                Warp warp = exit.Value;     // The warp object itself
                Console.WriteLine($"Exit at {exitPoint}: {warp}");
            }

            foreach (KeyValuePair<Structs.Point, WorldMap> worldMapLinks in _client._map.WorldMaps)
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
            _client._isWalking = false;
        }

        private void waypointsMenu_Click_1(object sender, EventArgs e)
        {
            if (_wayForm == null || _wayForm.IsDisposed)
            {
                _wayForm = new WayForm(_client);
                _wayForm.Show();
            }
            else
            {
                // Bring the existing form to the front
                _wayForm.Show();
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
                _client.ServerMessage(0, "Your " + _client.Inventory[1].Name + " has a Sprite ID of " + (_client.Inventory[1].Sprite - 32768) + ".");
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
            foreach (Client client in _client._server._clientList)
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
    }
}

