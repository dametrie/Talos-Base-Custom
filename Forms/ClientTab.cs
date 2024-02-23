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

        internal ulong _sessionExperience;

        private List<string> _chatPanelList = new List<string>
        {
            "",
            "",
            "",
            "",
            ""
        };
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


        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            InitializeComponent();

            worldObjectListBox.DataSource = client._worldObjectBindingList;
            creatureHashListBox.DataSource = client._creatureBindingList;
            strangerList.DataSource = client._strangerBindingList;
            friendList.DataSource = client._friendBindingList;

            trashList.DataSource = trashToDrop;

            OnlyDisplaySpellsWeHave();

        }

        private void OnlyDisplaySpellsWeHave()
        {
            SetupComboBox(healCombox, new[] { "nuadhaich", "ard ioc", "mor ioc", "ioc", "beag ioc", "Cold Blood", "Spirit Essence" }, healCbox, healPctNum);
            SetupComboBox(dionCombox, new[] { "mor dion", "Iron Skin", "Wings of Protection", "Draco Stance", "dion", "Stone Skin", "Glowing Stone" }, dionCbox, dionPctNum, dionWhenCombox, aoSithCbox);
            SetupComboBox(fasCombox, new[] { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" }, fasCbox);
            SetupComboBox(aiteCombox, new[] { "ard naomh aite", "mor naomh aite", "naomh aite", "beag naomh aite" }, aiteCbox);

            SetupCheckbox(deireasFaileasCbox, "deireas faileas");
            SetupCheckbox(aoSuainCbox, "ao suain", "Leafhopper Chirp");
            SetupCheckbox(aoCurseCbox, "ao beag cradh", "ao cradh", "ao mor cradh", "ao ard cradh");
            SetupCheckbox(aoPoisonCbox, "ao puinsein");
            SetupCheckbox(bubbleBlockCbox, "Bubble Block", "Bubble Shield");
            SetupCheckbox(spamBubbleCbox, "Bubble Block", "Bubble Shield");
            SetupCheckbox(fungusExtractCbox, "Fungus Beetle Extract");
            SetupCheckbox(mantidScentCbox, "Mantid Scent", "Potent Mantid Scent");
        }
        private void SetupComboBox(ComboBox comboBox, string[] spells, Control control1, Control control2 = null, Control control3 = null, Control control4 = null)
        {
            comboBox.Items.Clear();
            foreach (var spell in spells)
            {
                if (_client.Spellbook[spell] != null)
                {
                    comboBox.Items.Add(spell);
                }
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
            {
                control1.Enabled = false;
                comboBox.Enabled = false;
                comboBox.Text = String.Empty;
                if (control2 is not null)
                {
                    control2.Enabled = false;
                    if (control2 is NumericUpDown numericUpDown)
                        numericUpDown.Value = 0;
                }
                if(control3 is not null)
                {
                    control3.Enabled = false;
                    if (control3 is TextBox textBox)
                        textBox.Text = String.Empty;
                }
                if(control4 is not null)
                    control4.Enabled = false;
            }
        }

        private void SetupCheckbox(CheckBox checkBox, params string[] spellOrItemNames)
        {
            bool enabled = spellOrItemNames.Any(name => _client.Spellbook[name] != null || _client.HasItem(name));
            checkBox.Enabled = enabled;
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
                    SetControlVisibility(dragonScaleCbox, "Dragon's Scale", true);
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
            healthBar.Value = _client.Health;
            manaBar.Value = _client.Mana;

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
            if (base.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    AddMessageToChatPanel(color, message);
                });
                return;
            }
            if (chatPanel.TextLength != 0)
            {
                message = System.Environment.NewLine + message;
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
            Packet packet = (Packet)packetList.SelectedItem;
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
                    int num2 = 1;
                    if (!string.IsNullOrEmpty(match_0.Groups[2].Value))
                    {
                        num2 = int.Parse(match_0.Groups[2].Value);
                    }
                    byte[] array2 = Encoding.GetEncoding(949).GetBytes(match_0.Groups[1].Value.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t"));
                    if (num2 != 0)
                    {
                        int num3 = array2.Length;
                        Array.Resize(ref array2, num3 + num2);
                        Buffer.BlockCopy(array2, 0, array2, num2, num3);
                        if (num2 == 1)
                        {
                            array2[0] = (byte)num3;
                        }
                        else
                        {
                            array2[0] = (byte)(num3 >> 8);
                            array2[1] = (byte)num3;
                        }
                    }
                    return BitConverter.ToString(array2);
                }).Replace("-", string.Empty).Replace(" ", string.Empty);
                if (text.Length < 2 || text.Length % 2 != 0 || !Regex.IsMatch(text, "^[a-f0-9]+$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                ClientPacket clientPacket = new ClientPacket(byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber));
                if (text.Length > 2)
                {
                    int num = (text.Length - 2) / 2;
                    byte[] array = new byte[num];
                    for (int j = 0; j < num; j++)
                    {
                        int startIndex = 2 + j * 2;
                        array[j] = byte.Parse(text.Substring(startIndex, 2), NumberStyles.HexNumber);
                    }
                    clientPacket.Write(array);
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
                    int num2 = 1;
                    if (!string.IsNullOrEmpty(match_0.Groups[2].Value))
                    {
                        num2 = int.Parse(match_0.Groups[2].Value);
                    }
                    byte[] array2 = Encoding.GetEncoding(949).GetBytes(match_0.Groups[1].Value.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t"));
                    if (num2 != 0)
                    {
                        int num3 = array2.Length;
                        Array.Resize(ref array2, num3 + num2);
                        Buffer.BlockCopy(array2, 0, array2, num2, num3);
                        if (num2 == 1)
                        {
                            array2[0] = (byte)num3;
                        }
                        else
                        {
                            array2[0] = (byte)(num3 >> 8);
                            array2[1] = (byte)num3;
                        }
                    }
                    return BitConverter.ToString(array2);
                }).Replace("-", string.Empty).Replace(" ", string.Empty);
                if (text.Length < 2 || text.Length % 2 != 0 || !Regex.IsMatch(text, "^[a-f0-9]+$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                ServerPacket serverPacket = new ServerPacket(byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber));
                if (text.Length > 2)
                {
                    int num = (text.Length - 2) / 2;
                    byte[] array = new byte[num];
                    for (int j = 0; j < num; j++)
                    {
                        int startIndex = 2 + j * 2;
                        array[j] = byte.Parse(text.Substring(startIndex, 2), NumberStyles.HexNumber);
                    }
                    serverPacket.Write(array);
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

        private void timer_0_Tick(object sender, EventArgs e)
        {

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

        internal void DisplayObject(WorldObject worldObject)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { DisplayObject(worldObject); })); return; }

            lastClickedIDLbl.Text = "ID: " + worldObject.ID;
            lastClickedNameLbl.Text = (worldObject.Name ?? "");

            if (worldObject is VisibleObject visibleObject)
            {
                lastClickedSpriteLbl.Text = "Sprite: " + visibleObject.Sprite;
                lastClickedSpriteLbl.Refresh();
            }
                

        }

        internal void checkMonsterForm(bool isChecked, ushort monsterID)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => { checkMonsterForm(isChecked, monsterID); })); return; }

            formCbox.Checked = isChecked;
            formNum.Value = monsterID;


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
                    HashSet<string> nearbyPlayers = new HashSet<string>(_client.GetNearbyPlayers().Select(player => player.Name), StringComparer.CurrentCultureIgnoreCase);
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
            OnlyDisplaySpellsWeHave();
            SetClassSpecificSpells();
            //LoadUnMaxedSkills();
            //LoadUnMaxedSpells();  
        }

        private void addAislingBtn_Click(object sender, EventArgs e)
        {
            string text = addAislingText.Text;
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

        internal void AddAlly(string name, Image image = null)
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
            //UpdateNearbyAllyTable(name);
            //RefreshNearbyAllyTable();
            _client.Bot.UpdateAllyList(ally);

        }

        //internal void UpdateNearbyAllyTable(string name)
        //{
        //    if (!nearbyAllyTable.Controls.ContainsKey(name))
        //    {
        //        return;
        //    }

        //    Control allyControl = nearbyAllyTable.Controls[name];

        //    WaitForCondition(allyControl, control => ((NearbyAlly)control).bool_0);

        //    allyControl.Dispose();
        //}

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

        //private void RefreshNearbyAllyTable()
        //{
        //    List<NearbyAlly> allies = nearbyAllyTable.Controls.OfType<NearbyAlly>().ToList();
        //    nearbyAllyTable.Controls.Clear();
        //    foreach (NearbyAlly ally in allies)
        //    {
        //        nearbyAllyTable.Controls.Add(ally);
        //    }
        //}

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
            _isBotRunning = !_isBotRunning; // Toggle the running state

            if (_isBotRunning)
            {
                StartBot();
            }
            else
            {
                StopBot();
            }
        }

        private void StartBot()
        {
            startStrip.Text = "Stop";

            // Initialize BotBase only if it's different from the current Bot
            if (_client.BotBase != _client.Bot)
            {
                _client.BotBase = _client.Bot;
                _client.BotBase.Client = _client;
                _client.BotBase.Server = _client._server;
            }

            _client.BotBase.Start();
            _client.ServerMessage(1, "Bot Started");
        }

        private void StopBot()
        {
            startStrip.Text = "Start";
            _client.BotBase.Stop();

            _client._isWalking = false;
            _client._isCasting = false;

            _client.ServerMessage(1, "Bot Stopped");
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
    }
}

