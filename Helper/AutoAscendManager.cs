using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talos.Base;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Talos.Maps;
using Talos.Enumerations;
using Newtonsoft.Json.Linq;
using Talos.Extensions;
using System.IO;
using System.Text.RegularExpressions;
using Talos.Structs;

namespace Talos.Helper
{
    internal class AutoAscendManager
    {
        private Client _client;
        private Server _server;
        private Dictionary<string, int> _lastDelayTimes;
        private List<string> GotToSafety = new List<string>();
        private List<string> _ascendSongs = new List<string>
        {
            "Abel Song",
            "Loures Song",
            "Piet Song",
            "Undine Song",
            "Rucesion Song",
            "Mileth Song",
            "Suomi Song"
        };

        internal AutoAscendManager(Client client, Server server)
        {
            _client = client;
            _server = server;
        }

        private int GenerateStaggeredDelay(string clientIdentifier)
        {

            int staggeredDelay = Enumerable.Max(Enumerable.DefaultIfEmpty(_lastDelayTimes.Values, 0)) + 750;
            _lastDelayTimes[clientIdentifier] = staggeredDelay;
            return staggeredDelay;
        }

        private bool GetToSafetyAscend()
        {
            // Step 1: Notify the system about the ascending process
            _client.ServerMessage((byte)ServerMessageType.Whisper, "WantsToAscend -> Getting to safety.");
            _server.ManualServerSwitch[_client.Name] = true;

            string originalMapName = _client.Map.Name;

            Thread.Sleep(GenerateStaggeredDelay(_client.Name));

            _server.MedTask[_client.Name] = "Ascending";
            _server.MedWalkSpeed[_client.Name] = _client.ClientTab.walkSpeedSldr.Value;
            GotToSafety.Add(_client.Name);

            // Step 6: Iterate through each song and attempt to use it
            foreach (string song in _ascendSongs)
            {
                if (_client.UseItem(song))
                {
                    DateTime songUsedTime = DateTime.UtcNow;
                    string targetMapName = song.Replace(" Song", "");

                    // Step 7: Wait until the map name changes to the target map or timeout occurs
                    while (!_client.Map.Name.Contains(targetMapName))
                    {
                        TimeSpan elapsed = DateTime.UtcNow - songUsedTime;

                        if (elapsed.TotalSeconds > 2.0)
                        {
                            // Attempt to use the song again after timeout
                            _client.UseItem(song);

                            // Check if the map has changed to the target or if it's different from the original
                            if (_client.Map.Name.Contains(targetMapName) || _client.Map.Name != originalMapName)
                                return true;

                            // Break out of the inner loop to proceed to the next song
                            break;
                        }

                        // Sleep briefly before rechecking the condition
                        Thread.Sleep(10);
                    }

                    // If the map has changed successfully, exit the method
                    if (_client.Map.Name.Contains(targetMapName) || _client.Map.Name != originalMapName)
                        return true;
                }
            }

            // Step 8: Final check to determine if the map has changed from the original
            return _client.Map.Name != originalMapName;
        }

        private void ManageAscendingCompleteState()
        {
            bool flag;
            switch (_client.Map.MapID)
            {
                case 3086:
                case 3087:
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }
            if (flag)
                return;
            if (GotToSafety.Contains(_client.Name))
                GotToSafety.Remove(_client.Name);
            _lastDelayTimes.Remove(_client.Name);
            _client.ServerMessage((byte)ServerMessageType.Whisper, "AscendingComplete -> LoadWalkingProfile");
            SetAscendState(CharacterState.LoadingWalkingProfile);
        }

        private void SetAscendState(CharacterState state)
        {
            _server.ClientStateList[_client.Name] = state;
        }

        private async Task LoadHuntingProfileAsync()
        {
            if (!TryGetAutoAscendProperty<string>("HuntProfile", out var huntProfile) || string.IsNullOrEmpty(huntProfile))
                return;

            Func<Task> loadProfileAction = async () => await _client.ClientTab.LoadProfileAsync(huntProfile);

            // Invoke the action on the UI thread if required
            if (_client.ClientTab.InvokeRequired)
            {
                _client.ClientTab.Invoke(new Action(async () => await loadProfileAction()));
            }
            else
            {
                await loadProfileAction();
            }

            // Notify the server that the hunting profile has been loaded
            _client.ServerMessage((byte)ServerMessageType.Whisper, $"LoadHuntingProfile -> Loaded {huntProfile}.");

            // Update the ascend state to indicate the waypoint profile is loading
            SetAscendState(CharacterState.LoadingWaypointProfile);
        }

        private async Task LoadWaypointProfileAsync()
        {
            // Attempt to retrieve the "Waypoints" property and ensure it's not null or whitespace
            if (!TryGetAutoAscendProperty<string>("Waypoints", out var waypoints) || string.IsNullOrWhiteSpace(waypoints))
                return;

            if (waypoints.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                // Notify that no waypoints are to be loaded and set the state accordingly
                _client.ServerMessage((byte)ServerMessageType.Whisper, "LoadWaypoints -> WaitForSpells");
                SetAscendState(CharacterState.WaitForSpells);
            }
            else
            {
                // Construct the full path to the waypoint file
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "waypoints", waypoints);

                if (File.Exists(path))
                {
                    // Define the asynchronous action to load waypoints
                    Func<Task> loadWaypointsFunc = async () => await Task.Run(() => LoadWaypoints(path));

                    // Invoke the action on the UI thread if required
                    if (_client.ClientTab.InvokeRequired)
                    {
                        _client.ClientTab.Invoke(new Action(async () => await loadWaypointsFunc()));
                    }
                    else
                    {
                        await loadWaypointsFunc();
                    }
                }
                else
                {
                    // Notify that the specified waypoint file was not found
                    _client.ServerMessage((byte)ServerMessageType.Whisper, $"Waypoint file \"{path}\" not found.");
                }

                // Notify about the load process and set the state accordingly
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"LoadWaypoints ({waypoints}) -> WaitForSpells");
                SetAscendState(CharacterState.WaitForSpells);
            }
        }

        public void LoadWaypoints(string waypoints)
        {
            // Define the path to the waypoint file
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string waypointsPath = Path.Combine(baseDirectory, "waypoints", waypoints);

            // Check if the waypoint file exists
            if (!File.Exists(waypointsPath))
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"Waypoint file \"{waypointsPath}\" not found.");
                return;
            }

            // Read all lines from the waypoint file
            List<string> lines = File.ReadAllLines(waypointsPath).ToList();

            // Ensure the file is not empty
            if (lines.Count == 0)
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"Waypoint file \"{waypointsPath}\" is empty.");
                return;
            }

            Regex waypointHeaderPattern = new Regex(
                @"^(True|False)\s+(True|False)\s+(True|False)\s+(True|False)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)\s+([0-9]+)$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Regex locationPattern = new Regex(
                @"^\(([0-9]+),([0-9]+)\)\s+([a-zA-Z0-9' -]+):\s+([0-9]+)$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Match the header using the predefined regex pattern
            Match headerMatch = waypointHeaderPattern.Match(lines[0]);

            // Validate the regex match
            if (!headerMatch.Success)
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"Invalid waypoint header in \"{waypointsPath}\".");
                return;
            }

            // Local functions to parse boolean and decimal values safely
            bool TryParseBool(string input, out bool result)
            {
                return bool.TryParse(input, out result);
            }

            decimal TryParseDecimal(string input)
            {
                return decimal.TryParse(input, out decimal value) ? value : 0;
            }

            // Assign values from regex groups to UI controls
            try
            {
                _client.ClientTab.WayForm.condition1.Checked = TryParseBool(headerMatch.Groups[1].Value, out bool cond1) && cond1;
                _client.ClientTab.WayForm.condition2.Checked = TryParseBool(headerMatch.Groups[2].Value, out bool cond2) && cond2;
                _client.ClientTab.WayForm.condition3.Checked = TryParseBool(headerMatch.Groups[3].Value, out bool cond3) && cond3;
                _client.ClientTab.WayForm.condition4.Checked = TryParseBool(headerMatch.Groups[4].Value, out bool cond4) && cond4;

                _client.ClientTab.WayForm.mobSizeUpDwn1.Value = TryParseDecimal(headerMatch.Groups[5].Value);
                _client.ClientTab.WayForm.mobSizeUpDwn2.Value = TryParseDecimal(headerMatch.Groups[6].Value);
                _client.ClientTab.WayForm.mobSizeUpDwn3.Value = TryParseDecimal(headerMatch.Groups[7].Value);
                _client.ClientTab.WayForm.mobSizeUpDwn4.Value = TryParseDecimal(headerMatch.Groups[8].Value);

                _client.ClientTab.WayForm.proximityUpDwn1.Value = TryParseDecimal(headerMatch.Groups[9].Value);
                _client.ClientTab.WayForm.proximityUpDwn2.Value = TryParseDecimal(headerMatch.Groups[10].Value);
                _client.ClientTab.WayForm.proximityUpDwn3.Value = TryParseDecimal(headerMatch.Groups[11].Value);
                _client.ClientTab.WayForm.proximityUpDwn4.Value = TryParseDecimal(headerMatch.Groups[12].Value);

                _client.ClientTab.WayForm.walkSlowUpDwn1.Value = TryParseDecimal(headerMatch.Groups[13].Value);
                _client.ClientTab.WayForm.walkSlowUpDwn2.Value = TryParseDecimal(headerMatch.Groups[14].Value);
                _client.ClientTab.WayForm.walkSlowUpDwn3.Value = TryParseDecimal(headerMatch.Groups[15].Value);

                _client.ClientTab.WayForm.distanceUpDwn.Value = TryParseDecimal(headerMatch.Groups[17].Value);
            }
            catch (IndexOutOfRangeException ex)
            {
                _client.ServerMessage((byte)ServerMessageType.Whisper, $"Insufficient waypoint header groups: {ex.Message}");
                return;
            }

            // Clear existing waypoints in the UI and bot
            _client.ClientTab.WayForm.waypointsLBox.Items.Clear();
            _client.Bot.ways.Clear();

            // Iterate through each waypoint line and process it
            for (int index = 1; index < lines.Count; index++)
            {
                string waypointLine = lines[index];
                _client.ClientTab.WayForm.waypointsLBox.Items.Add(waypointLine);

                Match locationMatch = locationPattern.Match(waypointLine);
                if (locationMatch.Success)
                {
                    short ID;
                    bool parseID = short.TryParse(locationMatch.Groups[1].Value, out ID);

                    short X;
                    bool parseX = short.TryParse(locationMatch.Groups[2].Value, out X);

                    short Y;
                    bool parseY = short.TryParse(locationMatch.Groups[4].Value, out Y);

                    bool isValid = parseID && parseX && parseY;

                    if (isValid)
                    {
                        Location location = new Location(ID, X, Y);
                        _client.Bot.ways.Add(location);
                    }
                    else
                    {
                        _client.ServerMessage((byte)ServerMessageType.Whisper, $"Error parsing waypoint line {index}: Invalid number format.");
                    }
                }
                else
                {
                    _client.ServerMessage((byte)ServerMessageType.Whisper, $"Invalid waypoint format in line {index}.");
                }
            }
        }

        private bool TryGetAutoAscendProperty<T>(string propertyName, out T? value)
        {
            if (TryGetAutoAscendData(out JObject? data))
            {
                return data.TryGetTypedValue(propertyName, out value);
            }

            value = default;
            return false;
        }

        private bool TryGetAutoAscendData(out JObject? data)
        {
            data = _server.MainForm.AutoAscendDataList
                        .FirstOrDefault(d => d.HasValue("Name", _client.Name));

            return data != null;
        }
    }
}
