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

namespace Talos.Forms
{
    public partial class ClientTab : UserControl
    {

        private Client _client;
        private bool _isBotRunning = false;
        private string textMaptext = string.Empty;
        private string textXtext = string.Empty;
        private string textYtext = string.Empty;
        private short textMap;
        private short textX;
        private short testY;

        private uint _abilityExp;
        private uint _gold;
        internal ulong _sessionExperience;
        internal uint _sessionAbility;
        internal uint _sessionGold;

        private Stopwatch _sessionExperienceStopWatch = new Stopwatch();
        private Stopwatch _sessionAbilityStopWatch = new Stopwatch();
        private Stopwatch _sessionGoldStopWatch = new Stopwatch();

        internal DateTime _inventoryUpdateTime;
        internal DateTime _lastStatusUpdate;

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

        internal BindingList<string> trashToDrop = new BindingList<string>
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


        private readonly object _lock = new object();

        internal DATArchive setoaArchive;
        internal EPFImage skillImageArchive;
        internal EPFImage spellImageArchive;
        internal Palette256 palette256;


        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            UIHelper.Initialize(_client);
            InitializeComponent();

            worldObjectListBox.DataSource = client._worldObjectBindingList;
            creatureHashListBox.DataSource = client._creatureBindingList;
            strangerList.DataSource = client._strangerBindingList;
            friendList.DataSource = client._friendBindingList;
            trashList.DataSource = trashToDrop;

            setoaArchive = DATArchive.FromFile(Settings.Default.DarkAgesPath.Replace("Darkages.exe", "setoa.dat"));
            spellImageArchive = EPFImage.FromArchive("spell001.epf", setoaArchive);
            skillImageArchive = EPFImage.FromArchive("skill001.epf", setoaArchive);
            palette256 = Palette256.FromArchive("gui06.pal", setoaArchive);

            OnlyDisplaySpellsWeHave();

            AddClientToFriends();
            
            

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
                        SetPureClassSpells(asgallCbox, "Asgall Faileas");
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
            if (!string.IsNullOrEmpty(name) && listBox != null)
            {
                if (bindingList.Count != 0)
                {
                    bindingList.Add(name);
                    return;
                }
                listBox.DataSource = null;
                bindingList.Add(name);
                listBox.DataSource = bindingList;
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
            bool flag = chatPanel.IsScrollBarAtBottom();
            SetChatPanelTextColor(color);
            if (!string.IsNullOrWhiteSpace(message))
            {
                chatPanel.AppendText(message);
            }
            SetChatPanelTextColor(chatPanel.ForeColor);
            if (flag)
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
            TimeSpan elapsedTimeSinceLastBonus = DateTime.Now - _client.Bot._lastBonusAppliedTime;

            // Accumulate the total elapsed time with any previously stored elapsed time
            TimeSpan totalElapsedTime = elapsedTimeSinceLastBonus + _client.Bot._bonusElapsedTime;

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
            if (_client._isRefreshing)
            {
                button7.Text = "Refresh";
                _client._isRefreshing = false;
            }
            else
            {
                button7.Text = "Stop Refreshing";
                _client._isRefreshing = true;
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


        internal void checkMonsterForm(bool isChecked, ushort monsterID)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { checkMonsterForm(isChecked, monsterID); })); return; }

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
                while (!(_client._clientLocation.Equals(targetLocation)))
                {
                    //_client.WalkToLocation(targetLocation);
                    //_client.TryWalkToLocation(_client.Pathfinder, targetLocation, 0);
                    _client.TryWalkToLocation3(targetLocation, 0, true);
                }

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _client.RemoveShield();
        }



        internal void UpdateStrangerListAfterCharge()
        {
            Thread.Sleep(1600);
            UpdateStrangerList();
        }

        internal void UpdateStrangerList()
        {
            
            lock (_lock)
            {
                bool isChargeSkillUsedRecently = _client.HasSkill("Charge") && (DateTime.UtcNow - _client.Skillbook["Charge"].LastUsed).TotalMilliseconds < 1500.0;
                bool isSprintPotionUsedRecently = _client.HasItem("Sprint Potion") && (DateTime.UtcNow - _client.Inventory.HasItemReturnSlot("Sprint Potion").LastUsed).TotalMilliseconds < 1500.0;

                if (!isChargeSkillUsedRecently && !isSprintPotionUsedRecently)
                {
                    HashSet<string> nearbyPlayers = new HashSet<string>(_client.GetNearbyPlayerList().Select(player => player.Name), StringComparer.CurrentCultureIgnoreCase);
                    HashSet<string> nonStrangers = new HashSet<string>(_client.AllyListHashSet.Concat(_client._friendBindingList), StringComparer.CurrentCultureIgnoreCase);

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
            //LoadUnMaxedSkills();
            //LoadUnMaxedSpells();  
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
            else if (_client.Bot.EnemyPage == null && !_client.Bot._enemyListID.Contains(npc.SpriteID) && !nearbyEnemyTable.Controls.ContainsKey(npc.SpriteID.ToString()))
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
            if (_client.Bot.EnemyPage != null || !nearbyEnemyTable.Controls.ContainsKey(sprite.ToString()))
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
            _client._groupBindingList = new BindingList<string>(_client.AllyListHashSet.ToList());
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

        //internal void UpdateNearbyEnemyTable(ushort sprite)
        //{
        //    if (_client.Bot.EnemyPage != null || !nearbyEnemyTable.Controls.ContainsKey(sprite.ToString()))
        //    {
        //        return;
        //    }

        //    Control enemyControl = nearbyEnemyTable.Controls[sprite.ToString()];

        //    WaitForCondition(enemyControl, control => ((NearbyEnemy)control).bool_0);

        //    enemyControl.Dispose();
        //}

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



        private void startStrip_Click_1(object sender, EventArgs e)
        {
            if (startStrip.Text == "Start")
            {
                startStrip.Text = "Stop";
                if (_client.BotBase != _client.Bot)
                {
                    _client.BotBase = _client.Bot;
                    _client.BotBase.Client = _client;
                    _client.BotBase.Server = _client._server;
                    _client.Int32_1 = 0;
                }
                _client.BotBase.Start();
                if (!_client.ClientTab.safeScreenCbox.Checked)
                {
                    _client.ServerMessage(1, "Bot Started");
                }
            }
            else if (startStrip.Text == "Stop")
            {
                startStrip.Text = "Start";
                _client.BotBase.Stop();
                if (!_client.ClientTab.safeScreenCbox.Checked)
                {
                    _client.ServerMessage(1, "Bot Stopped");
                }
                _client._isWalking = false;
                _client._isCasting = false;
                _client.Bot.bool_11 = false;
                _client.Bot.bool_12 = false;
                _client.bool_44 = false;
            }
        }


        private void lastSeenBtn_Click(object sender, EventArgs e)
        {
            _client.ServerMessage(0, "- Last 5 strangers sighted -");

            // Take only the top 5 most recently seen strangers
            var lastSeenStrangers = _client.DictLastSeen
                .OrderByDescending(kvp => kvp.Value)
                .Take(5);

            foreach (var item in lastSeenStrangers)
            {
                string message = $"{item.Key}: {item.Value.ToLocalTime():t}";
                _client.ServerMessage(0, message);
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
                if (!_client.AllyListHashSet.Contains(client.Name) && _client != client)
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
            foreach (string name in _client.AllyListHashSet)
            {
                if (_client.Bot.AllyPage == null || _client._server.FindClientByName(name) == null)
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
            foreach (string name in _client.AllyListHashSet)
            {
                if (_client.Bot.AllyPage == null || _client._server.FindClientByName(name) == null)
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
            if (_client.Bot.EnemyPage != null)
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
            _client.Bot.EnemyPage = enemyPage;
        }

/*        internal void ClearNearbyEnemies()
        {
            if (_client.Bot.EnemyPage != null)
            {
                return;
            }
            DateTime utcNow = DateTime.UtcNow;
            while (nearbyEnemyTable.Controls.Count > 0 && !(DateTime.UtcNow.Subtract(utcNow).TotalMilliseconds >= 50.0))
            {
                if ((nearbyEnemyTable.Controls[0] as NearbyEnemy)._isLoaded)
                {
                    nearbyEnemyTable.Controls[0].Dispose();
                }
            }
            nearbyEnemyTable.Controls.Clear();
        }
*/
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
                if (!_client.Bot.IsEnemyAlreadyListed(sprite) && _client.Bot.EnemyPage == null)
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

        }

        private void saveStrip_Click(object sender, EventArgs e)
        {

        }

        private void loadStrip_MouseEnter(object sender, EventArgs e)
        {

        }

        private void clearStrip_Click(object sender, EventArgs e)
        {

        }

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
            // Mapping of combobox text to the item names
            var itemMappings = new Dictionary<string, string>
            {
                { "Kruna 50%", "50 Percent EXP/AP Bonus" },
                { "Xmas 50%", "XMas Bonus Exp-Ap" },
                { "Star 100%", "Double Bonus Exp-Ap" },
                { "Vday 100%", "VDay Bonus Exp-Ap" }
            };

            // Check for special case where additional logic is needed
            if (doublesCombox.Text == "Xmas 100%")
            {
                var itemText = _client.HasItem("Christmas Double Exp-Ap") ? "Christmas Double Exp-Ap" : "XMas Double Exp-Ap";
                UseItem(itemText);
            }
            else if (itemMappings.TryGetValue(doublesCombox.Text, out var itemText))
            {
                UseItem(itemText);
            }

            UpdateBonusTimer();
        }

        private void UseItem(string itemText)
        {
            if (!_client.UseItem(itemText))
            {
                _client.ServerMessage(0, $"You do not own any {itemText}.");
                autoDoubleCbox.Checked = false;
            }
        }

        private void UpdateBonusTimer()
        {
            _client.Bot._lastBonusAppliedTime = DateTime.Now;
            _client.Bot._bonusElapsedTime = _client.Bot._bonusElapsedTime;

            if (!bonusCooldownTimer.Enabled)
            {
                bonusCooldownTimer.Enabled = true;
                bonusCooldownTimer.Start();
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
                _client.ServerMessage(1, "You do not have any experience gems.");
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

        internal void UpdateInventoryAndWaypoints()
        {
            //Adam
        }

        internal void UpdateClientStatus()
        {
            //Adam
        }
    }
}

