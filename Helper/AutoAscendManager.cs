using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Definitions;
using Newtonsoft.Json.Linq;
using Talos.Extensions;
using System.IO;
using System.Text.RegularExpressions;
using Talos.Structs;
using Talos.Definitions;
using Talos.Properties;

namespace Talos.Helper
{
    internal class AutoAscendManager
    {
        private Client _client;
        private Server _server;
        private Dictionary<string, int> _lastDelayTimes;
        private List<string> GotToSafety = new List<string>();
        private bool _setInitialState;
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
        internal void AutoAscendTask()
        {
            // Check preconditions
            if (!Settings.Default.EnableAutoAscending ||
                !_server.MainForm.AutoAscendDataList.OfType<JObject>().Any(data => data.HasValue<string>("Name", _client.Name)) ||
                !InitialChecks())
            {
                return;
            }

            // Ensure initial state is set
            if (!_setInitialState)
            {
                CheckAndSetHuntingActivity();
            }

            // Retrieve the current client state
            if (!_server.ClientStateList.TryGetValue(_client.Name, out var characterState))
            {
                return;
            }

            // Handle client states
            switch (characterState)
            {
                case CharacterState.Hunting:
                    ManageHuntingState();
                    break;

                case CharacterState.LoadingWaypointProfile:
                    LoadWaypointProfileAsync();
                    break;

                case CharacterState.LoadingHuntingProfile:
                    LoadHuntingProfileAsync();
                    break;

                case CharacterState.LoadingWalkingProfile:
                    LoadWalkingProfile();
                    break;

                case CharacterState.WantsToAscend:
                    ManageAscendingState();
                    break;

                case CharacterState.ClearProfile:
                    _client.ClientTab.Invoke(new Action(() => _client.ClientTab.ClearOptions()));
                    Thread.Sleep(3000);
                    if (TryGetAutoAscendProperty("NotGrouped", out bool notGrouped) && notGrouped)
                    {
                        _client.ServerMessage(0, "ClearProfile -> NotGrouped -> LoadWalkingProfile");
                        SetAscendState(CharacterState.LoadingWalkingProfile);
                    }
                    else
                    {
                        _client.ServerMessage(0, "Clearing Profiles -> Ascending");
                        SetAscendState(CharacterState.Ascending);
                    }
                    break;

                case CharacterState.Ascending:
                    // Ensure the ascend button is clicked if not already in the "Ascending" state
                    if (_client.ClientTab.ascendBtn.Text != "Ascending")
                    {
                        _client.ClientTab.ascendBtn_Click(this, EventArgs.Empty);
                    }

                    // Ensure the death option is checked
                    if (!_client.ClientTab.deathOptionCbx.Checked)
                    {
                        _client.ClientTab.deathOptionCbx.Checked = true;
                    }

                    // Retrieve and apply the HP or MP selection if valid
                    if (TryGetAutoAscendProperty("HPorMP", out string ascendType) &&
                        (ascendType == "HP" || ascendType == "MP") &&
                        _client.ClientTab.ascendOptionCbx.SelectedItem?.ToString() != ascendType)
                    {
                        _client.ClientTab.ascendOptionCbx.SelectedItem = ascendType;
                    }
                    break;

                case CharacterState.AscendingComplete:
                    ManageAscendingCompleteState();
                    break;

                case CharacterState.Walking:
                    ManageWalkingState();
                    break;

                case CharacterState.Waiting:
                    ManageWaitingState();
                    break;

                case CharacterState.WaitForSpells:
                    ManageWaitForSpellsState();
                    break;
            }
        }

        private bool InitialChecks()
        {
            // Check if the client's name exists in the AutoAscendDataList
            bool nameExists = _server.MainForm.AutoAscendDataList
                .OfType<JObject>()
                .Any(data => data.TryGetValue("Name", out JToken jtoken) && jtoken?.Value<string>() == _client.Name);

            // Ensure the client state is set to Idle if not already present
            _server.ClientStateList.TryAdd(_client.Name, CharacterState.Idle);

            // Return true if the name exists and auto ascending is enabled
            return nameExists && Settings.Default.EnableAutoAscending;
        }

        private bool IsPlayerInHuntingArea(string currentMapName)
        {
            // Check if the current map name starts with any hunting area prefix (case-insensitive)
            return CONSTANTS.HUNTING_AREA_PREFIXES.Any(prefix => currentMapName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
        private void CheckAndSetHuntingActivity()
        {
            if (_client.Map == null) return;
            // Check if the player is in a hunting area
            bool isInHuntingArea = IsPlayerInHuntingArea(_client.Map.Name);

            // Retrieve the current client state
            if (!_server.ClientStateList.TryGetValue(_client.Name, out var characterState))
                return;

            // Handle client states
            switch (characterState)
            {
                case CharacterState.WantsToAscend:
                    _client.ServerMessage(0, "CheckAndSet -> WantsToAscend");
                    _setInitialState = true;
                    break;

                case CharacterState.Ascending:
                    _client.ServerMessage(0, "CheckAndSet -> Ascending");
                    _setInitialState = true;
                    break;

                case CharacterState.Walking:
                    _client.ServerMessage(0, "CheckAndSet -> Walking");
                    _setInitialState = true;
                    break;

                default:
                    if (isInHuntingArea)
                    {
                        if (_client.Experience < 4200000000U)
                        {
                            _client.ServerMessage(0, "CheckAndSet -> Load Profiles -> Hunting");
                            SetAscendState(CharacterState.LoadingHuntingProfile);
                            _setInitialState = true;
                        }
                        else
                        {
                            SetAscendState(CharacterState.WantsToAscend);
                            _client.ServerMessage(0, "CheckAndSet -> State.WantsToAscend");
                            _setInitialState = true;
                        }
                        break;
                    }

                    // Handle Idle states based on experience
                    if (_client.Experience >= 4200000000U)
                    {
                        SetAscendState(CharacterState.WantsToAscend);
                        _client.ServerMessage(0, "Idle (> Exp) -> State.WantsToAscend");
                    }
                    else
                    {
                        SetAscendState(CharacterState.LoadingWalkingProfile);
                        _client.ServerMessage(0, "Idle (< Exp) -> State.Walking");
                    }
                    _setInitialState = true;
                    break;
            }
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
        private void ManageWalkingState()
        {
            // Retrieve player location data
            if (!TryGetAutoAscendProperty("PlayerLocation", out JObject playerLocation) || playerLocation == null)
                return;

            // Extract map ID and coordinates
            int mapId = playerLocation.Value<int>("MapId");
            int x = playerLocation.Value<int>("X");
            int y = playerLocation.Value<int>("Y");
            var targetLocation = new Location((short)mapId, (short)x, (short)y);

            // Check if the player is already at the target location
            if (_client.ClientLocation.MapID == mapId && _client.ServerLocation.AbsoluteXY((short)x, (short)y) <= 3)
            {
                _client.ServerMessage(0, "Walking -> State.Waiting");
                SetAscendState(CharacterState.Waiting);
            }
            else
            {
                // Continue routing if the bot is running
                if (!_client.Bot.CancellationToken.IsCancellationRequested)
                {
                    _client.Routefind(targetLocation);
                }
            }
        }
        private void ManageWaitingState()
        {
            string currentGroupName = GetCurrentClientGroupName();

            // Get the names of clients in the current group
            var autoAscendNamesInGroup = _server.MainForm.AutoAscendDataList
                .OfType<JObject>()
                .Where(data => data.ContainsKey("Name") && data.HasValue<string>("Group", currentGroupName))
                .Select(data => data["Name"]?.Value<string>())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            // Get the clients in the group who are ready to hunt or wait
            var clientsWantingToHunt = _server.Clients
                .Where(client =>
                {
                    bool isInGroup = autoAscendNamesInGroup.Contains(client.Name);
                    if (!isInGroup) return false;

                    // Check if the client is in one of the allowed states
                    return _server.ClientStateList.TryGetValue(client.Name, out var state) &&
                           (state == CharacterState.Hunting ||
                            state == CharacterState.LoadingWaypointProfile ||
                            state == CharacterState.LoadingHuntingProfile ||
                            state == CharacterState.Waiting);
                })
                .Select(client => client.Name)
                .ToList();

            // Verify if all group members are ready to hunt or wait
            bool allReadyToHunt = autoAscendNamesInGroup.All(name => clientsWantingToHunt.Contains(name));

            if (!allReadyToHunt)
                return;

            // Transition to the next state
            _client.ServerMessage(0, "Waiting -> LoadHuntingProfile");
            SetAscendState(CharacterState.LoadingHuntingProfile);
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
        public void ManageAscendingState()
        {
            string currentGroupName = GetCurrentClientGroupName();

            // Check if the group is set to ascend all
            bool ascendAll = _server.MainForm.AutoAscendDataList
                .OfType<JObject>()
                .Any(data => data.HasValue<string>("Group", currentGroupName) && data.HasValue<bool>("AscendAll", true));

            if (ascendAll)
            {
                SetGroupToWantsToAscend(currentGroupName);
            }

            // Get all group member names
            var groupMemberNames = _server.MainForm.AutoAscendDataList
                .OfType<JObject>()
                .Where(data => data.HasValue<string>("Group", currentGroupName))
                .Select(data => data["Name"]?.Value<string>())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            // Get clients who are ready to ascend
            var clientsWantingToAscend = _server.Clients
                .Where(c =>
                {
                    bool isGroupMember = groupMemberNames.Contains(c.Name);
                    bool isInAscendState = _server.ClientStateList.TryGetValue(c.Name, out var state) &&
                                           (state == CharacterState.WantsToAscend ||
                                            state == CharacterState.Ascending ||
                                            state == CharacterState.Walking ||
                                            state == CharacterState.Waiting);

                    return isGroupMember && isInAscendState && !c.Player.IsAsleep && !c.Player.IsSuained && !c.Player.IsSkulled;
                })
                .Select(c => c.Name)
                .ToList();

            // Check if all group members are ready to ascend
            bool allReadyToAscend = groupMemberNames.All(name => clientsWantingToAscend.Contains(name));

            // Handle safety check and transitioning states
            if (allReadyToAscend && !GotToSafety.Contains(_client.Name, StringComparer.OrdinalIgnoreCase))
            {
                if (!GetToSafetyAscend())
                {
                    return;
                }
            }

            if (allReadyToAscend && !GotToSafety.Contains(_client.Name, StringComparer.OrdinalIgnoreCase))
            {
                _client.RefreshRequest();
                _client.ServerMessage(0, "Getting to safety -> Clearing profiles");
                SetAscendState(CharacterState.ClearProfile);
            }
        }
        private void ManageHuntingState()
        {
            // Check if the client is in the Hunting state
            if (!_server.ClientStateList.TryGetValue(_client.Name, out var characterState) || characterState != CharacterState.Hunting)
                return;

            // Reset flags if necessary
            if (_client.WarBagDeposited)
                _client.WarBagDeposited = false;

            if (_client.AscendTaskDone)
                _client.AscendTaskDone = false;

            // Check experience and transition to WantsToAscend if criteria are met
            if (_client.Experience >= 4200000000U && _server.ClientStateList[_client.Name] != CharacterState.WantsToAscend)
            {
                _client.ServerMessage(0, "Hunting (> Exp) -> State.WantsToAscend");
                _server.ClientStateList[_client.Name] = CharacterState.WantsToAscend;
            }

            // Handle non-grouped logic
            if (TryGetAutoAscendProperty("NotGrouped", out bool notGrouped) && notGrouped)
            {
                _client.ServerMessage(0, "Hunting -> WantsToAscend (Non Grouped: Ready to Leave)");
                SetAscendState(CharacterState.WantsToAscend);
            }
        }
        private void ManageWaitForSpellsState()
        {
            // Notify the system about the state transition
            _client.ServerMessage(0, "WaitForSpells -> Sleeping (10) Seconds");

            // Pause walking for 10 seconds
            _client.IsWalking = false;
            Thread.Sleep(10000);
            _client.IsWalking = true;

            // Ensure the walk button is in the correct state
            if (!_client.ClientTab.IsBashingActive && _client.ClientTab.walkBtn.Text != "Stop" && _client.ClientTab.WayForm.waypointsLBox.Items.Count > 0)
            {
                _client.ClientTab.walkBtn.Text = "Stop";
            }

            // Notify about the transition to Hunting and update the state
            _client.ServerMessage(0, "WaitForSpells -> Hunting");
            SetAscendState(CharacterState.Hunting);
        }

        private void SetGroupToWantsToAscend(string groupName)
        {
            // Check if the "NotGrouped" property exists and is set to true; if so, exit
            if (TryGetAutoAscendProperty("NotGrouped", out bool notGrouped) && notGrouped)
                return;

            // Iterate through all clients
            foreach (var client in _server.Clients)
            {
                // Find the associated data in the AutoAscendDataList
                var clientData = _server.MainForm.AutoAscendDataList
                    .OfType<JObject>()
                    .FirstOrDefault(data =>
                        data.ContainsKey("Name") &&
                        data["Name"]?.ToString() == client.Name);

                // Check if the client is in the specified group and not already set to WantsToAscend
                if (clientData != null &&
                    clientData.TryGetValue("Group", out JToken groupToken) &&
                    groupToken?.ToString() == groupName &&
                    _server.ClientStateList[client.Name] != CharacterState.WantsToAscend)
                {
                    // Update the client's state to WantsToAscend
                    _server.ClientStateList[client.Name] = CharacterState.WantsToAscend;
                }
            }
        }
        private string GetCurrentClientGroupName()
        {
            string str;
            return TryGetAutoAscendProperty<string>("Group", out str) ? str : "Group1";
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
        private void LoadWalkingProfile()
        {
            // Retrieve the walking profile name
            if (!TryGetAutoAscendProperty("SafeWalk", out string walkProfile) || string.IsNullOrEmpty(walkProfile))
                return;

            // Load the walking profile
            if (_client.ClientTab.InvokeRequired)
            {
                _client.ClientTab.Invoke(new Action(async () => await _client.ClientTab.LoadProfileAsync(walkProfile)));
            }
            else
            {
                _client.ClientTab.LoadProfileAsync(walkProfile);
            }

            // Notify and update state
            _client.ServerMessage(0, $"LoadWalkingProfile -> Walking (Loaded {walkProfile} Profile)");
            SetAscendState(CharacterState.Walking);
        }
        private void SetAscendState(CharacterState state)
        {
            _server.ClientStateList[_client.Name] = state;
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
