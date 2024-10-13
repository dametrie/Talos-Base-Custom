using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Structs;

namespace Talos.Forms
{
    public partial class WayForm : Form
    {
        private readonly object _lock = new object();

        internal Client Client { get; set; }


        internal WayForm(Client client)
        {
            InitializeComponent();
            Client = client;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            Client.Bot.ways.Clear();
            waypointsLBox.Items.Clear();
        }
        private void savedWaysLBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (savedWaysLBox.SelectedItem != null)
            {
                saveTBox.Text = savedWaysLBox.SelectedItem.ToString();
            }
            else
            {
                saveTBox.Text = string.Empty;
            }
        }

        private async void loadBtn_Click(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(Server.SyncObj, 2000))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        Client.ClientTab.StopBot();
                        Client.ClientTab._isLoading = true;
                        string str = AppDomain.CurrentDomain.BaseDirectory + "waypoints\\";
                        if (savedWaysLBox.SelectedItem != null && !string.IsNullOrEmpty(savedWaysLBox.SelectedItem.ToString()) && File.Exists(str + savedWaysLBox.SelectedItem.ToString()))
                        {
                            List<string> list = File.ReadAllLines(str + savedWaysLBox.SelectedItem.ToString()).ToList();
                            Match match = Regex.Match(list[0], "(True|False) (True|False) (True|False) (True|False) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+)");
                            condition1.Checked = Convert.ToBoolean(match.Groups[1].Value);
                            condition2.Checked = Convert.ToBoolean(match.Groups[2].Value);
                            condition3.Checked = Convert.ToBoolean(match.Groups[3].Value);
                            condition4.Checked = Convert.ToBoolean(match.Groups[4].Value);
                            mobSizeUpDwn1.Value = Convert.ToDecimal(match.Groups[5].Value);
                            mobSizeUpDwn2.Value = Convert.ToDecimal(match.Groups[6].Value);
                            mobSizeUpDwn3.Value = Convert.ToDecimal(match.Groups[7].Value);
                            mobSizeUpDwn4.Value = Convert.ToDecimal(match.Groups[8].Value);
                            proximityUpDwn1.Value = Convert.ToDecimal(match.Groups[9].Value);
                            proximityUpDwn2.Value = Convert.ToDecimal(match.Groups[10].Value);
                            proximityUpDwn3.Value = Convert.ToDecimal(match.Groups[11].Value);
                            proximityUpDwn4.Value = Convert.ToDecimal(match.Groups[12].Value);
                            walkSlowUpDwn1.Value = Convert.ToDecimal(match.Groups[13].Value);
                            walkSlowUpDwn2.Value = Convert.ToDecimal(match.Groups[14].Value);
                            walkSlowUpDwn3.Value = Convert.ToDecimal(match.Groups[15].Value);
                            distanceUpDwn.Value = Convert.ToDecimal(match.Groups[17].Value);
                            waypointsLBox.Items.Clear();
                            Client.Bot.ways.Clear();
                            for (int i = 1; i < list.Count; i++)
                            {
                                waypointsLBox.Items.Add(list[i]);
                                Match match2 = Regex.Match(list[i], "(?:\\(([0-9]+),([0-9]+)\\)) ([a-zA-Z0-9' -]+): ([0-9]+)");
                                Client.Bot.ways.Add(new Location(short.Parse(match2.Groups[4].Value), short.Parse(match2.Groups[1].Value), short.Parse(match2.Groups[2].Value)));
                            }
                        }
                        Client.ClientTab._isLoading = false;
                        if (Client.ClientTab.startStrip.Text == "Stop")
                        {
                            Client.BotBase.Start();
                        }
                    });
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string text = AppDomain.CurrentDomain.BaseDirectory + "waypoints\\" + savedWaysLBox.SelectedItem.ToString();
                if (File.Exists(text))
                {
                    File.Delete(text);
                }
                Client.ClientTab._wayFormProfiles.Remove(text);
            }
            catch
            {
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveTBox.Text))
            {
                return;
            }
            if (Regex.Match(saveTBox.Text, "[\\/\\*\\\"\\[\\]\\\\\\:\\?\\|\\<\\>]").Success)
            {
                MessageDialog.Show(Client._server._mainForm, "Cannot use characters /*\"[]\\:?|<>", this);
                return;
            }
            string text = AppDomain.CurrentDomain.BaseDirectory + "waypoints";
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            List<string> list = new List<string>();
            list.Add(condition1.Checked + " " + condition2.Checked + " " + condition3.Checked + " " + condition4.Checked + " " + mobSizeUpDwn1.Value + " " + mobSizeUpDwn2.Value + " " + mobSizeUpDwn3.Value + " " + mobSizeUpDwn4.Value + " " + proximityUpDwn1.Value + " " + proximityUpDwn2.Value + " " + proximityUpDwn3.Value + " " + proximityUpDwn4.Value + " " + walkSlowUpDwn1.Value + " " + walkSlowUpDwn2.Value + " " + walkSlowUpDwn3.Value + " " + 0 + " " + distanceUpDwn.Value);
            List<string> list2 = list;
            foreach (object item in waypointsLBox.Items)
            {
                list2.Add(item.ToString());
            }
            File.WriteAllLines(text + "\\" + saveTBox.Text, list2);
            if (!savedWaysLBox.Items.Contains(saveTBox.Text))
            {
                Client.ClientTab._wayFormProfiles.Add(saveTBox.Text);
            }
        }

        private void hideFormBtn_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void waypointsRemoveBtn_Click(object sender, EventArgs e)
        {
            if (waypointsLBox.SelectedItems.Count > 0)
            {
                foreach (string item2 in waypointsLBox.SelectedItems.Cast<string>().ToList())
                {
                    Match match = Regex.Match(item2, "(?:\\(([0-9]+),([0-9]+)\\)) ([a-zA-Z0-9' -]+): ([0-9]+)");
                    Location item = new Location(short.Parse(match.Groups[4].ToString()), short.Parse(match.Groups[1].ToString()), short.Parse(match.Groups[2].ToString()));
                    Client.Bot.ways.Remove(item);
                    waypointsLBox.Items.Remove(item2);
                }
            }
        }

        private void MoveUpBtn_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                int topIndex = waypointsLBox.TopIndex;
                List<string> list = waypointsLBox.Items.Cast<string>().ToList();
                List<string> list2 = waypointsLBox.SelectedItems.Cast<string>().ToList();
                List<Location> list_ = Client.Bot.ways;
                for (int i = 0; i < list2.Count; i++)
                {
                    if (list2[i] != list[i])
                    {
                        int num = list.IndexOf(list2[i]);
                        string value = list[num - 1];
                        list[num - 1] = list[num];
                        list[num] = value;
                        Location value2 = list_[num - 1];
                        list_[num - 1] = list_[num];
                        list_[num] = value2;
                    }
                }
                waypointsLBox.Items.Clear();
                ListBox.ObjectCollection items = waypointsLBox.Items;
                object[] items2 = list.ToArray();
                items.AddRange(items2);
                waypointsLBox.SelectedItems.Clear();
                foreach (string item in list2)
                {
                    waypointsLBox.SelectedItems.Add(item);
                }
                waypointsLBox.TopIndex = topIndex;
            }
        }

        private void MoveDownBtn_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                int topIndex = waypointsLBox.TopIndex;
                List<string> list = waypointsLBox.Items.Cast<string>().Reverse().ToList();
                List<string> list2 = waypointsLBox.SelectedItems.Cast<string>().Reverse().ToList();
                List<Location> list_ = Client.Bot.ways;
                Client.Bot.ways.Reverse();
                for (int i = 0; i < list2.Count; i++)
                {
                    if (list2[i] != list[i])
                    {
                        int num = list.IndexOf(list2[i]);
                        string value = list[num - 1];
                        list[num - 1] = list[num];
                        list[num] = value;
                        Location value2 = list_[num - 1];
                        list_[num - 1] = list_[num];
                        list_[num] = value2;
                    }
                }
                list.Reverse();
                list2.Reverse();
                list_.Reverse();
                waypointsLBox.Items.Clear();
                ListBox.ObjectCollection items = waypointsLBox.Items;
                object[] items2 = list.ToArray();
                items.AddRange(items2);
                waypointsLBox.SelectedItems.Clear();
                foreach (string item in list2)
                {
                    waypointsLBox.SelectedItems.Add(item);
                }
                waypointsLBox.TopIndex = topIndex;
            }
        }

        private void waypointsAddBtn_Click(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(Server.SyncObj, 1000))
            {
                try
                {
                    string item = string.Empty;
                    item = "(" + Client._clientLocation.X + "," + Client._clientLocation.Y + ") " + Client._map.Name + ": " + Client._map.MapID;
                    waypointsLBox.Items.Add(item);
                    Client.Bot.ways.Add(Client._clientLocation);
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
        }
    }
}



