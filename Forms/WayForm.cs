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
            // Attempt to acquire the lock asynchronously
            bool lockAcquired = false;

            try
            {
                lockAcquired = await Client.ClientTab._loadLock.WaitAsync(TimeSpan.FromSeconds(2));
                if (!lockAcquired)
                {
                    Console.WriteLine("[Error] Unable to acquire lock for loading.");
                    return;
                }

                // Stop the bot and set loading flag
                Client.ClientTab.StopBot();
                Client.ClientTab._isLoading = true;

                // Perform the heavy task asynchronously
                var waypointsData = await Task.Run(() =>
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory + "waypoints\\";
                    string selectedFile = savedWaysLBox.SelectedItem?.ToString();

                    if (string.IsNullOrEmpty(selectedFile) || !File.Exists(basePath + selectedFile))
                        return (null, new List<(string, Location)>());

                    // Read file lines
                    var fileLines = File.ReadAllLines(basePath + selectedFile).ToList();

                    // Parse the configuration line
                    Match configMatch = Regex.Match(fileLines[0],
                        @"(True|False) (True|False) (True|False) (True|False) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+)");

                    if (!configMatch.Success)
                        return (null, new List<(string, Location)>());

                    // Extract configuration values
                    var config = new
                    {
                        Conditions = new bool[4]
                        {
                    bool.Parse(configMatch.Groups[1].Value),
                    bool.Parse(configMatch.Groups[2].Value),
                    bool.Parse(configMatch.Groups[3].Value),
                    bool.Parse(configMatch.Groups[4].Value)
                        },
                        MobSizes = new decimal[4]
                        {
                    decimal.Parse(configMatch.Groups[5].Value),
                    decimal.Parse(configMatch.Groups[6].Value),
                    decimal.Parse(configMatch.Groups[7].Value),
                    decimal.Parse(configMatch.Groups[8].Value)
                        },
                        Proximities = new decimal[4]
                        {
                    decimal.Parse(configMatch.Groups[9].Value),
                    decimal.Parse(configMatch.Groups[10].Value),
                    decimal.Parse(configMatch.Groups[11].Value),
                    decimal.Parse(configMatch.Groups[12].Value)
                        },
                        WalkSlows = new decimal[3]
                        {
                    decimal.Parse(configMatch.Groups[13].Value),
                    decimal.Parse(configMatch.Groups[14].Value),
                    decimal.Parse(configMatch.Groups[15].Value)
                        },
                        Distance = decimal.Parse(configMatch.Groups[17].Value)
                    };

                    // Parse waypoints
                    var waypoints = new List<(string DisplayText, Location Location)>();
                    for (int i = 1; i < fileLines.Count; i++)
                    {
                        Match waypointMatch = Regex.Match(fileLines[i],
                            @"\(([0-9]+),([0-9]+)\) ([a-zA-Z0-9' -]+): ([0-9]+)");

                        if (waypointMatch.Success)
                        {
                            var loc = new Location(
                                short.Parse(waypointMatch.Groups[4].Value),
                                short.Parse(waypointMatch.Groups[1].Value),
                                short.Parse(waypointMatch.Groups[2].Value)
                            );
                            waypoints.Add((fileLines[i], loc));
                        }
                    }

                    return (config, waypoints);
                });

                if (waypointsData.Item1 == null)
                {
                    Console.WriteLine("Error loading waypoints or configuration.");
                    return;
                }
                // Update UI on the main thread
                Invoke((Action)(() =>
                {
                    // Update UI on the main thread
                    UpdateUIWithWaypoints(waypointsData);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Exception in loadBtn_Click: {ex.Message}");
            }
            finally
            {
                if (lockAcquired)
                {
                    Client.ClientTab._loadLock.Release();
                }
                Client.ClientTab._isLoading = false;

                // Restart the bot if needed
                if (Client.ClientTab.startStrip.Text == "Stop")
                {
                    Client.BotBase.Start();
                }
            }
        }

        private void UpdateUIWithWaypoints((dynamic Config, List<(string DisplayText, Location Location)> Waypoints) waypointsData)
        {
            condition1.Checked = waypointsData.Config.Conditions[0];
            condition2.Checked = waypointsData.Config.Conditions[1];
            condition3.Checked = waypointsData.Config.Conditions[2];
            condition4.Checked = waypointsData.Config.Conditions[3];

            mobSizeUpDwn1.Value = waypointsData.Config.MobSizes[0];
            mobSizeUpDwn2.Value = waypointsData.Config.MobSizes[1];
            mobSizeUpDwn3.Value = waypointsData.Config.MobSizes[2];
            mobSizeUpDwn4.Value = waypointsData.Config.MobSizes[3];

            proximityUpDwn1.Value = waypointsData.Config.Proximities[0];
            proximityUpDwn2.Value = waypointsData.Config.Proximities[1];
            proximityUpDwn3.Value = waypointsData.Config.Proximities[2];
            proximityUpDwn4.Value = waypointsData.Config.Proximities[3];

            walkSlowUpDwn1.Value = waypointsData.Config.WalkSlows[0];
            walkSlowUpDwn2.Value = waypointsData.Config.WalkSlows[1];
            walkSlowUpDwn3.Value = waypointsData.Config.WalkSlows[2];

            distanceUpDwn.Value = waypointsData.Config.Distance;

            waypointsLBox.Items.Clear();
            Client.Bot.ways.Clear();
            foreach (var (displayText, loc) in waypointsData.Waypoints)
            {
                waypointsLBox.Items.Add(displayText);
                Client.Bot.ways.Add(loc);
            }
        }




        private void deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string text = AppDomain.CurrentDomain.BaseDirectory + "waypoints\\" + savedWaysLBox.SelectedItem.ToString();
                if (File.Exists(text))
                {
                    Console.WriteLine("Deleting waypoint file: " + text);
                    File.Delete(text);
                }
                Client.ClientTab._wayFormProfiles.Remove(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveTBox.Text))
            {
                MessageDialog.Show(Client._server._mainForm, "Save name cannot be empty.", this);
                return;
            }

            // Check for invalid characters in the filename
            if (Regex.IsMatch(saveTBox.Text, @"[\/\*\""\\\[\]\:\?\|\<\>]"))
            {
                MessageDialog.Show(Client._server._mainForm, "Cannot use characters /*\"[]\\:?|<> in the save name.", this);
                return;
            }

            // Define the directory for saving waypoints
            string waypointsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "waypoints");

            // Ensure the directory exists
            if (!Directory.Exists(waypointsDir))
            {
                Directory.CreateDirectory(waypointsDir);
            }

            // Prepare the file path for saving
            string filePath = Path.Combine(waypointsDir, saveTBox.Text);

            // Build the list of settings and waypoints
            var dataToSave = new List<string>
            {
                $"{condition1.Checked} {condition2.Checked} {condition3.Checked} {condition4.Checked} " +
                $"{mobSizeUpDwn1.Value} {mobSizeUpDwn2.Value} {mobSizeUpDwn3.Value} {mobSizeUpDwn4.Value} " +
                $"{proximityUpDwn1.Value} {proximityUpDwn2.Value} {proximityUpDwn3.Value} {proximityUpDwn4.Value} " +
                $"{walkSlowUpDwn1.Value} {walkSlowUpDwn2.Value} {walkSlowUpDwn3.Value} 0 {distanceUpDwn.Value}"
            };

            // Add all waypoints from the ListBox to the list
            foreach (var item in waypointsLBox.Items.Cast<string>())
            {
                dataToSave.Add(item);
            }

            try
            {
                // Write data to the file asynchronously using StreamWriter
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (var line in dataToSave)
                    {
                        await writer.WriteLineAsync(line);
                    }
                }

                // Update saved profiles list if the save name is new
                if (!savedWaysLBox.Items.Contains(saveTBox.Text))
                {
                    savedWaysLBox.Items.Add(saveTBox.Text);
                    Client.ClientTab._wayFormProfiles.Add(saveTBox.Text);
                }

                MessageDialog.Show(Client._server._mainForm, "Waypoints saved successfully!", this);
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                MessageDialog.Show(Client._server._mainForm, $"Failed to save waypoints: {ex.Message}", this);
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
                    // Correct the Regex pattern to match the format of your items
                    Match match = Regex.Match(item2, @"\((\d+),(\d+)\) (.+): (\d+)");
                    if (match.Success)
                    {
                        // Extract values using match.Groups[n].Value
                        short mapId = short.Parse(match.Groups[4].Value);  // Map ID
                        short x = short.Parse(match.Groups[1].Value);      // X Coordinate
                        short y = short.Parse(match.Groups[2].Value);      // Y Coordinate

                        Location item = new Location(mapId, x, y);

                        // Remove the waypoint from both the Client.Bot.ways and the ListBox
                        Client.Bot.ways.Remove(item);
                        waypointsLBox.Items.Remove(item2);
                    }
                    else
                    {
                        Console.WriteLine($"[Error] Failed to parse waypoint: {item2}");
                    }
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



