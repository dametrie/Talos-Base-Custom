using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Cryptography;
using Talos.Forms.UI;
using Talos.Networking;

namespace Talos.Forms
{
    public partial class ClientTab : UserControl
    {
        
        internal Client _client;

        internal ResourceBar _manaBar;
        internal ResourceBar _healthBar;

        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            InitializeComponent();
        }

        internal void DisplayHPMP()
        {
            if (base.InvokeRequired) { Invoke((Action)delegate { DisplayHPMP(); }); }
            else
            {
                hpLbl.Text = _client.CurrentHP.ToString();
                mpLbl.Text = _client.CurrentMP.ToString();
                _healthBar.Value = _client.Health;
                _manaBar.Value = _client.Mana;
            }
        }

        internal void RemoveClient()
        {
            _client = null;
        }


        internal void LogPackets(Packet  p)
        {
            if (base.InvokeRequired) { Invoke((Action)delegate { LogPackets(p); }); }
            else
                packetList.Items.Add(p);
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
            if (base.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    LogPackets(clientPacket);
                });
            }
            else if (toggleLogSendBtn.Checked)
            {
                packetList.Items.Add(clientPacket.Copy());
            }
        }

        internal void LogPackets(ServerPacket serverPacket)
        {
            if (base.InvokeRequired)
            {
                Invoke((Action)delegate
                {
                    LogPackets(serverPacket);
                });
            }
            else if (toggleLogRecvBtn.Checked)
            {
                packetList.Items.Add(serverPacket.Copy());
            }
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
    }
}
