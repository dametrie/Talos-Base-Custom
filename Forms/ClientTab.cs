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

namespace Talos.Forms
{
    public partial class ClientTab : UserControl
    {

        internal Client _client;
        string textMaptext = string.Empty;
        string textXtext = string.Empty;
        string textYtext = string.Empty;
        short textMap;
        short textX;
        short testY;

        internal List<string> chatPanelList = new List<string>
        {
            "",
            "",
            "",
            "",
            ""
        };
        internal ulong _sessionExperience;

        

        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            InitializeComponent();
            worldObjectListBox.DataSource = client._worldObjectBindingList;
            creatureHashListBox.DataSource = client._creatureBindingList;
            OnlyDisplaySpellsWeHave();

        }

        private void OnlyDisplaySpellsWeHave()
        {
            //setup heals combo box
            healCombox.Items.Clear();
            string[] heals = { "nuadhaich", "ard ioc", "mor ioc", "ioc", "beag ioc", "Cold Blood", "Spirit Essence" };
            foreach (string spell in heals)
            {
                if (_client.Spellbook[spell] != null)
                    healCombox.Items.Add(spell);
            }
            if (healCombox.Items.Count > 0)
                healCombox.SelectedIndex = 0;
            else
            {
                healCbox.Enabled = false;
                healCombox.Enabled = false;
                healCombox.Text = String.Empty;
                healPctNum.Enabled = false;
                healPctNum.Value = 0;
            }

            //setup dion combo box
            dionCombox.Items.Clear();
            string[] dions = { "mor dion", "Iron Skin", "Wings of Protection", "Draco Stance", "dion", "Stone Skin", "Glowing Stone" };
            foreach (string spell in dions)
            {
                if (_client.Spellbook[spell] != null)
                    dionCombox.Items.Add(spell);
            }
            if (_client.HasItem("Glowing Stone"))
                dionCombox.Items.Add("Glowing Stone");
            if (dionCombox.Items.Count > 0)
                dionCombox.SelectedIndex = 0;
            else
            {
                dionCbox.Enabled = false;
                dionCombox.Enabled = false;
                dionCombox.Text = String.Empty;
                dionWhenCombox.Enabled = false;
                dionWhenCombox.Text = String.Empty;
                dionPctNum.Enabled = false;
                dionPctNum.Value = 0;
                aoSithCbox.Enabled = false;
            }

            //deireas faileas checkbox
            if (_client.Spellbook["deireas faileas"] != null)
                deireasFaileasCbox.Enabled = true;
            else
                deireasFaileasCbox.Enabled = false;

            //setup fas combo box  
            fasCombox.Items.Clear();
            string[] fases = { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" };
            foreach (string spell in fases)
            {
                if (_client.Spellbook[spell] != null)
                    fasCombox.Items.Add(spell);
            }
            if (fasCombox.Items.Count > 0)
                fasCombox.SelectedIndex = 0;
            else
            {
                fasCbox.Enabled = false;
                fasCombox.Enabled = false;
                fasCombox.Text = String.Empty;
            }


            //setup aite combo box
            aiteCombox.Items.Clear();
            string[] aites = { "ard naomh aite", "mor naomh aite", "naomh aite", "beag naomh aite" };
            foreach (string spell in aites)
            {
                if (_client.Spellbook[spell] != null)
                {
                    aiteCombox.Items.Add(spell);
                }

            }
            if (aiteCombox.Items.Count > 0)
                aiteCombox.SelectedIndex = 0;
            else
            {
                aiteCbox.Enabled = false;
                aiteCombox.Enabled = false;
                aiteCombox.Text = String.Empty;
            }

            //ao suain check box
            if (_client.Spellbook["ao suain"] != null || _client.Spellbook["Leafhopper Chirp"] != null)
                aoSuainCbox.Enabled = true;
            else
                aoSuainCbox.Enabled = false;

            //ao curse check box
            //Adam add grim scent or make new checkbox?
            if (_client.Spellbook["ao beag cradh"] != null || _client.Spellbook["ao cradh"] != null
                || _client.Spellbook["ao mor cradh"] != null || _client.Spellbook["ao ard cradh"] != null)
                aoCurseCbox.Enabled = true;
            else
                aoCurseCbox.Enabled = false;

            //ao puinsein check box
            if (_client.Spellbook["ao puinsein"] != null)
                aoPoisonCbox.Enabled = true;
            else
                aoPoisonCbox.Enabled = false;

            //bubble block
            if (_client.Spellbook["Bubble Block"] != null)
            {
                bubbleBlockCbox.Enabled = true;
                spamBubbleCbox.Enabled = true;
            }
            else
            {
                bubbleBlockCbox.Enabled = false;
                spamBubbleCbox.Enabled = false;
            }

            //class specific group


            //fungus extract check box
            if (_client.HasItem("Fungus Beetle Extract"))
                fungusExtractCbox.Enabled = true;
            else
                fungusExtractCbox.Enabled = false;

            //Mantid Scent check box
            if (_client.HasItem("Mantid Scent") || _client.HasItem("Potent Mantid Scent"))
                mantidScentCbox.Enabled = true;
            else
                mantidScentCbox.Enabled = false;
        }

        internal void SetClassSpecificSpells()
        {
            string medeniaClass = _client._medeniaClass.ToString();
            string previousClass = _client._previousClass.ToString();
            string temuairClass = _client._temuairClass.ToString();

            // Display check boxes based on Medenia class
            switch (medeniaClass)
            {
                case "Archer":
                    if (_client.HasItem("Nerve Stimulant"))
                        nerveStimulantCbox.Visible = true;

                    if (previousClass == "Pure")
                    {
                        if (_client.HasItem("Vanishing Elixir"))
                            vanishingElixirCbox.Visible = true;
                    }
                    break;
                case "Gladiator":
                    if (_client.HasItem("Muscle Stimulant"))
                        muscleStimulantCbox.Visible = true;

                    if (previousClass == "Pure")
                    {
                        if (_client.Spellbook["Aegis Sphere"] != null)
                            aegisSphereCbox.Visible = true;

                        if (_client.HasItem("Monster Call"))
                            monsterCallCbox.Visible = true;
                    }
                    break;
                case "Summoner":
                    if (_client.HasItem("Dragon's Scale"))
                        dragonScaleCbox.Enabled = true;;
                    
                    if (previousClass == "Pure")
                    {
                        if (_client.Spellbook["Lyliac Vineyard"] != null)
                        {
                            vineyardCbox.Visible = true;
                            vineCombox.Visible = true;
                            vineText.Visible = true;
                        }

                        if (_client.Spellbook["Disenchanter"] != null)
                            disenchanterCbox.Visible = true;

                        if (_client.Spellbook["Mana Ward"] != null)
                            manaWardCbox.Visible = true;

                        if (_client.HasItem("Dragon's Fire"))
                            dragonsFireCbox.Visible = true;
                    }
                    break;
                case "Bard":
                    if (_client.Spellbook["Regeneration"] != null || _client.Spellbook["Increased Regeneration"] != null)
                        regenerationCbox.Visible = true;

                    if (_client.HasItem("Wake Scroll"))
                        wakeScrollCbox.Visible = true;
                    break;
                case "Druid":
                    bool hasSpellWithForm = _client.Spellbook.Any(spell => spell.Name.Contains("Form"));
                    if (hasSpellWithForm)
                        druidFormCbox.Visible = true;
                    break;
            }

            // Display check boxes based on Temuair class
            switch (temuairClass)
            {
                case "Rogue":
                    if (_client.Spellbook["Hide"] != null)
                        hideCbox.Visible = true;
                    break;
                case "Warrior":
                    if (previousClass == "Pure")
                    {
                        if (_client.Spellbook["Asgall Faileas"] != null)
                            asgallCbox.Visible = true;

                        if (_client.Skillbook["Perfect Defense"] != null)
                            perfectDefenseCbox.Visible = true;
                    }
                    break;
                case "Wizard":
                    if (_client.Spellbook["fas spiorad"] != null)
                    {
                        fasSpioradCbox.Visible = true;
                        fasSpioradText.Visible = true;
                    }
                    break;
                case "Priest":
                    if (_client.Spellbook["armachd"] != null)
                        armachdCbox.Visible = true;

                    if (_client.Spellbook["beag cradh"] != null)
                        beagCradhCbox.Visible = true;
                    break;
                case "Monk":
                    if (_client.Spellbook["Mist"] != null)
                        mistCbox.Visible = true;

                    if (_client.Spellbook["White Bat Stance"] != null)
                        hideCbox.Visible = true;
                    break;
            }

            // Display check boxes based on previous class
            switch (previousClass)
            {
                case "Rogue":
                    if (_client.Spellbook["Hide"] != null)
                        hideCbox.Visible = true;
                    break;
                case "Warrior":
                    if (previousClass == "Pure")
                    {
                        asgallCbox.Visible = true;
                        perfectDefenseCbox.Visible = true;
                    }
                    break;
                case "Wizard":
                    if (_client.Spellbook["fas spiorad"] != null)
                    {
                        fasSpioradCbox.Visible = true;
                        fasSpioradText.Visible = true;
                    }
                    break;
                case "Priest":
                    if (_client.Spellbook["armachd"] != null)
                        armachdCbox.Visible = true;

                    if (_client.Spellbook["beag cradh"] != null)
                        beagCradhCbox.Visible = true;
                    break;
                case "Monk":
                    if (_client.Spellbook["Mist"] != null)
                        mistCbox.Visible = true;

                    if (_client.Spellbook["White Bat Stance"] != null)
                        hideCbox.Visible = true;
                    break;
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

        internal void UpdateStrangerListAfterCharge()
        {
            Thread.Sleep(1600);
            //UpdateStrangerList();/ADAM
        }
        internal void UpdateBindingList<T>(BindingList<T> bindingList, ListBox listBox, T item)
        {
            if (item != null && listBox != null)
            {
                
                if (bindingList.Contains(item))
                {

                    //bindingList.Remove(item);
                }
                else
                {
                    
                    //bindingList.Add(item);
                }

                // Update the DataSource of the ListBox
                listBox.DataSource = null; // Reset the DataSource
                listBox.DataSource = bindingList; // Reassign the DataSource

                listBox.Invoke((MethodInvoker)delegate {
                    listBox.Refresh(); // Refresh the ListBox to reflect changes
                });
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
            if (chatPanelList.Contains(text))
            {
                int index = chatPanelList.IndexOf(text);
                ShiftListItemsDown(index);
                chatPanelList[0] = text;
            }
            else
            {
                ShiftListItemsDown(chatPanelList.Count - 1);
                chatPanelList[0] = text;
            }
        }

        private void ShiftListItemsDown(int endIndex)
        {
            for (int i = endIndex; i > 0; i--)
            {
                chatPanelList[i] = chatPanelList[i - 1];
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
            _client.Walk(Direction.South);
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
                //while (!(_client._clientLocation.Equals(targetLocation)))
                //{
                _client.WalkToLocation(targetLocation);
                //}

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _client.RemoveShield();
        }

        internal void UpdateStrangerList()
        {
            throw new NotImplementedException();
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
            UpdateNearbyAllyTable(name);
            RefreshNearbyAllyTable();
            _client.Bot.UpdateAllyList(ally);

        }

        internal void UpdateNearbyAllyTable(string name)
        {
            if (!nearbyAllyTable.Controls.ContainsKey(name))
            {
                return;
            }

            Control allyControl = nearbyAllyTable.Controls[name];

            WaitForCondition(allyControl, control => ((NearbyAlly)control).bool_0);

            allyControl.Dispose();
        }

        internal void UpdateNearbyEnemyTable(ushort sprite)
        {
            if (_client.Bot.EnemyPage != null || !nearbyEnemyTable.Controls.ContainsKey(sprite.ToString()))
            {
                return;
            }

            Control enemyControl = nearbyEnemyTable.Controls[sprite.ToString()];

            WaitForCondition(enemyControl, control => ((NearbyEnemy)control).bool_0);

            enemyControl.Dispose();
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

    }
}

