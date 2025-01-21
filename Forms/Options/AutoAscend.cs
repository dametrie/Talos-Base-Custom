using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Properties;
using Talos.Structs;
using Talos.Utility;

namespace Talos.Options
{
    public partial class AutoAscend : UserControl, IOptionsPage
    {
        private MainForm _mainForm;
        public AutoAscend(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            chkAutoAscendEnable.Checked = Settings.Default.EnableAutoAscending;
        }

        public void Save()
        {
            Settings.Default.EnableAutoAscending = chkAutoAscendEnable.Checked;
            Settings.Default.Save();
        }

        private void AutoAscend_Load(object sender, EventArgs e)
        {
            foreach (JObject autoAscendData in _mainForm.AutoAscendDataList)
            {
                if (autoAscendData.TryGetValue("Name", out JToken nameToken) &&
                    autoAscendData.TryGetValue("Group", out JToken groupToken))
                {
                    string groupName = groupToken.ToString();
                    string playerName = nameToken.ToString();

                    ListBox targetListBox = groupName switch
                    {
                        "Group1" => charList1,
                        "Group2" => charList2,
                        "Group3" => charList3,
                        _ => null
                    };

                    if (targetListBox != null && !targetListBox.Items.Contains(playerName))
                    {
                        targetListBox.Items.Add(playerName);
                    }
                }
            }
        }

        private void addCharBtn_Click(object sender, EventArgs e)
        {
            var playerData = (Dictionary<string, object>)GatherPlayerData();

            string activeGroup = DetermineActiveGroup();
            playerData["Group"] = activeGroup;

            SavePlayerData(playerData);

            string playerName = playerData["Name"].ToString();
            UpdateCharacterList(activeGroup, playerName);

            ClearInputFields();
        }

        private void ClearInputFields()
        {
            _mainForm.LoadAutoAscendData();
            charList1.Items.Clear();
            charList2.Items.Clear();
            charList3.Items.Clear();
            UpdateCharacterListsBasedOnGroup();
            txtChar.Clear();
            comboWalk.Text = "";
            comboHunt.Text = "";
            comboWaypoint.Text = "";
            chkTriggerAll.Checked = false;
            hpRadio.Checked = true;
        }

        private void UpdateCharacterListsBasedOnGroup()
        {
            foreach (JObject autoAscendData in _mainForm.AutoAscendDataList)
            {
                if (autoAscendData.TryGetValue("Name", out JToken nameToken) &&
                    autoAscendData.TryGetValue("Group", out JToken groupToken))
                {
                    string groupName = groupToken.ToString();
                    string characterName = nameToken.ToString();

                    switch (groupName)
                    {
                        case "Group1":
                            if (!charList1.Items.Contains(characterName))
                                charList1.Items.Add(characterName);
                            break;

                        case "Group2":
                            if (!charList2.Items.Contains(characterName))
                                charList2.Items.Add(characterName);
                            break;

                        case "Group3":
                            if (!charList3.Items.Contains(characterName))
                                charList3.Items.Add(characterName);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void UpdateCharacterList(string groupName, string playerName)
        {
            var listBoxForGroup = GetListBoxForGroup(groupName);
            if (!listBoxForGroup.Items.Contains(playerName))
            {
                listBoxForGroup.Items.Add(playerName);
            }
        }

        private ListBox GetListBoxForGroup(string groupName)
        {
            switch (groupName)
            {
                case "Group1":
                    return charList1;

                case "Group2":
                    return charList2;

                case "Group3":
                    return charList3;

                default:
                    throw new ArgumentException($"Invalid group name: {groupName}", nameof(groupName));
            }
        }

        private void SavePlayerData(Dictionary<string, object> playerData)
        {
            try
            {
                // Serialize player data to JSON
                string jsonData = JsonConvert.SerializeObject(playerData, GetSerializerSettings());

                // Write the JSON data to the appropriate file
                string filePath = GetFullPath(playerData["Name"].ToString());
                File.WriteAllText(filePath, jsonData);

                // Reload AutoAscend data in the main form
                _mainForm.LoadAutoAscendData();
            }
            catch (Exception ex)
            {
                // Log any errors encountered during the save process
                Console.WriteLine($"Error saving player data: {ex.Message}");
            }
        }
        private JsonSerializerSettings GetSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            serializerSettings.Converters.Add(new LocationConverter());
            return serializerSettings;
        }
        private string GetFullPath(string playerName)
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "autoascend");
            Directory.CreateDirectory(basePath);
            return Path.Combine(basePath, $"{playerName}.json");
        }
        private string DetermineActiveGroup()
        {
            return tabCharSelect.SelectedTab switch
            {
                var tab when tab == tabGroupOne => "Group1",
                var tab when tab == tabGroupTwo => "Group2",
                var tab when tab == tabGroupThree => "Group3",
                _ => "Group1"
            };
        }

        private object GatherPlayerData()
        {
            var location = new Location(
                (short)numMapId.Value,
                (short)numX.Value,
                (short)numY.Value
            );

            string playerName = txtChar.Text;
            string safeWalkProfile = comboWalk.SelectedItem?.ToString();
            string huntProfile = comboHunt.SelectedItem?.ToString();
            string waypoints = comboWaypoint.Text.StartsWith("Found", StringComparison.OrdinalIgnoreCase)
                ? "None"
                : comboWaypoint.SelectedItem?.ToString();
            bool isNotGrouped = chkNotGrouped.Checked;
            string hpOrMp = hpRadio.Checked ? "HP" : "MP";
            bool triggerAll = chkTriggerAll.Checked;

            // Create a dictionary to store player data
            var playerData = new Dictionary<string, object>
            {
                { "Name", playerName },
                { "PlayerLocation", location },
                { "SafeWalk", safeWalkProfile },
                { "HuntProfile", huntProfile },
                { "Waypoints", waypoints },
                { "NotGrouped", isNotGrouped },
                { "HPorMP", hpOrMp },
                { "AscendAll", triggerAll }
            };

            return playerData;
        }

        private void removeCharBtn_Click(object sender, EventArgs e)
        {
            if (charList1.SelectedIndex >= 0)
            {
                string selectedCharacter = charList1.SelectedItem?.ToString();
                if (selectedCharacter != null)
                {
                    charList1.Items.RemoveAt(charList1.SelectedIndex);

                    JObject characterData = _mainForm.AutoAscendDataList
                        .FirstOrDefault(data => data["Name"]?.ToString() == selectedCharacter);

                    if (characterData != null)
                    {
                        _mainForm.AutoAscendDataList.Remove(characterData);

                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "autoascend", $"{selectedCharacter}.json");

                        try
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            else
                            {
                                Console.WriteLine($"File '{filePath}' does not exist.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error removing character '{selectedCharacter}': {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Character '{selectedCharacter}' not found in the data list.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No character selected.");
            }

        }

        private void txtChar_TextChanged(object sender, EventArgs e)
        {
            // Clear existing items and reset combo boxes
            comboWalk.Items.Clear();
            comboWalk.Text = "";
            comboHunt.Items.Clear();
            comboHunt.Text = "";

            if (!string.IsNullOrWhiteSpace(txtChar.Text))
            {
                string profilesPath = Path.Combine(Settings.Default.DarkAgesPath.Replace("Darkages.exe", ""), txtChar.Text, "Talos");
                if (Directory.Exists(profilesPath))
                {
                    var profileFiles = Directory.GetFiles(profilesPath, "*.xml")
                        .Select(Path.GetFileNameWithoutExtension);

                    int foundProfiles = 0;
                    foreach (var profile in profileFiles)
                    {
                        foundProfiles++;
                        comboWalk.Items.Add(profile);
                        comboHunt.Items.Add(profile);
                    }

                    comboWalk.Text = $"Found {foundProfiles} profiles.";
                    comboHunt.Text = $"Found {foundProfiles} profiles.";
                }
            }

            // Clear waypoints and reset combo boxes
            comboWaypoint.Items.Clear();
            comboWaypoint.Text = "";

            if (charList1 != null)
            {
                string waypointsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "waypoints");
                if (!Directory.Exists(waypointsPath))
                {
                    Directory.CreateDirectory(waypointsPath);
                }

                var waypointFiles = Directory.GetFiles(waypointsPath, "*")
                    .Select(Path.GetFileNameWithoutExtension);

                int foundWaypoints = 0;
                foreach (var waypoint in waypointFiles)
                {
                    foundWaypoints++;
                    comboWaypoint.Items.Add(waypoint);
                }

                comboWaypoint.Text = $"Found {foundWaypoints} waypoints.";
            }
        }

    }
}

