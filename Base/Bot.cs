using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Forms;
using Talos.Forms.UI;
using Talos.Objects;
using Talos.Properties;
using Talos.Structs;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
        private static object _lock { get; set; } = new object();
        private int _dropCounter;


        internal Client _client;
        internal Server _server;
        internal Creature creature;

        private bool _autoStaffSwitch;
        private bool _fasSpiorad;
        private bool _isSilenced;
        internal bool _needFasSpiorad = true;
        internal bool _manaLessThanEightyPct = true;
        internal bool _shouldBotStop = false;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;
        internal bool bool_11;
        internal bool bool_12;
        private bool bool_13;
        private bool bool_32;
        internal bool _hasRescue;

        internal byte _fowlCount;

        internal DateTime _lastKill = DateTime.MinValue;
        internal DateTime _lastDisenchanterCast = DateTime.MinValue;
        internal DateTime _lastGrimeScentCast = DateTime.MinValue;
        internal DateTime _skullTime = DateTime.MinValue;
        internal DateTime _lastRefresh = DateTime.MinValue;
        internal DateTime _lastVineCast = DateTime.MinValue;
        internal DateTime _botChecks = DateTime.MinValue;
        internal DateTime _lastBonusAppliedTime = DateTime.MinValue;
        internal DateTime _lastCast = DateTime.MinValue;
        internal DateTime _lastUsedGem = DateTime.MinValue;
        internal TimeSpan _bonusElapsedTime = TimeSpan.Zero;
        private DateTime _lastUsedFungusBeetle = DateTime.MinValue;

        internal List<Ally> _allyList = new List<Ally>();
        internal List<Enemy> _enemyList = new List<Enemy>();
        internal List<Player> _playersExistingOver250ms = new List<Player>();
        internal List<Player> _playersNeedingRed = new List<Player>();
        internal List<Player> _nearbyAllies = new List<Player>();
        internal List<Creature> _nearbyValidCreatures = new List<Creature>();

        internal HashSet<string> _allyListName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        internal HashSet<ushort> _enemyListID = new HashSet<ushort>();



        internal System.Windows.Forms.Label currentAction;
        private Location _lastBubbleLocation;
        private string _bubbleType;


        public bool RecentlyUsedGlowingStone { get; set; } = false;
        public bool RecentlyUsedDragonScale { get; set; } = false;
        public bool RecentlyUsedFungusExtract { get; set; } = false;
        internal AllyPage AllyPage { get; set; }
        internal EnemyPage EnemyPage { get; set; }


        internal Bot(Client client, Server server) : base(client, server)
        {
            _client = client;
            _server = server;
            AddTask(new TaskDelegate(BotLoop));
        }

        private void BotLoop()
        {
            while (true)
            {
                try
                {
                    //Console.WriteLine("BotLoop running");

                    if (Client.InArena)
                    {
                        //Console.WriteLine("Sleeping for 1 second");
                        Thread.Sleep(1000);
                        return;
                    }
                    if (currentAction == null)
                    {
                        currentAction = Client.ClientTab.currentAction;
                    }

                    _shouldBotStop = IsRangerNearBy();

                    //Console.WriteLine("Checking for stop conditions");
                    if (CheckForStopConditions())
                    {
                        //Console.WriteLine("Sleeping for 100ms");
                        Thread.Sleep(100);
                        return;
                    }

                    //Console.WriteLine("Processing players");
                    ProcessPlayers();

                    if (Client.CurrentHP <= 1U && Client.IsSkulled)
                    {
                        HandleSkullStatus();
                        return;
                    }

                    if (ShouldRequestRefresh())
                    {
                        Console.WriteLine("Requesting refresh");
                        Client.RequestRefresh(false);
                        _lastRefresh = DateTime.UtcNow;
                        continue;
                    }

                    //Console.WriteLine("Checking and handling spells");
                    //CheckAndHandleSpells();

                    //Console.WriteLine("Checking for autoRed conditions");
                    if (autoRedConditionsMet())
                    {
                        HandleAutoRed();//ADAM
                    }

                    //Console.WriteLine("Checking for strangers");
                    if (IsStrangerNearby())
                    {
                        FilterStrangerPlayers();
                    }

                    if (DateTime.UtcNow.Subtract(_botChecks).TotalSeconds < 2.5)
                    {
                        Console.WriteLine("Botcheck");
                        continue;//adam return?
                    }

                    PerformSpellActions();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in BotLoop: {ex.Message}");
                }
            }

        }

        internal bool IsRangerNearBy()
        {
            if (!Settings.Default.paranoiaMode)
            {
                return Client.GetNearbyPlayerList().Any(new Func<Player, bool>(RangerListContains));
            }
            return IsStrangerNearby();
        }

        private bool RangerListContains(Player player)
        {
            return CONSTANTS.KNOWN_RANGERS.Contains(player.Name, StringComparer.OrdinalIgnoreCase);
        }

        private bool CheckForStopConditions()
        {
            return (Client._inventoryFull && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   (Client.Bot.bool_32 && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   Client.bool_44 || Client.ClientTab == null || Client.Dialog != null || bool_12;
        }

        private void ProcessPlayers()
        {
            _nearbyAllies = Client.GetNearbyAllies();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            var nearbyPlayers = Client.GetNearbyPlayerList();
            _playersExistingOver250ms = nearbyPlayers?.Where(Delegates.HasPlayerExistedForOver250ms).ToList() ?? new List<Player>();
            _playersNeedingRed.Clear();
        }

        private void HandleSkullStatus()
        {
            currentAction.Text = Client._action + "Nothing, you're dead";
            _skullTime = (_skullTime == DateTime.MinValue) ? DateTime.UtcNow : _skullTime;
            LogIfSkulled();
            LogIfSkulledAndSurrounded();
            if (!Client._safeScreen)
            {
                Client.ServerMessage(18, "Skulled for: " + DateTime.UtcNow.Subtract(_skullTime).Seconds.ToString() + " seconds.");
            }
        }
        private void LogIfSkulled()
        {
            if (Client.ClientTab.optionsSkullCbox.Checked && DateTime.UtcNow.Subtract(_skullTime).TotalSeconds > 6.0)
            {
                string text = AppDomain.CurrentDomain.BaseDirectory + "\\skull.txt";
                File.WriteAllText(text, string.Concat(new string[]
                {
                    DateTime.UtcNow.ToLocalTime().ToString(),
                    "\n",
                    Client._map.Name,
                    ": (",
                    Client._serverLocation.ToString(),
                    ")"
                }));
                Process.Start(text);
                Client.DisconnectWait(false);
            }
        }

        private void LogIfSkulledAndSurrounded()
        {
            if (Client.ClientTab.optionsSkullSurrbox.Checked && DateTime.UtcNow.Subtract(_skullTime).TotalSeconds > 4.0 && Client.IsLocationSurrounded(Client._serverLocation))
            {
                string text2 = AppDomain.CurrentDomain.BaseDirectory + "\\skull.txt";
                File.WriteAllText(text2, string.Concat(new string[]
                {
                    DateTime.UtcNow.ToLocalTime().ToString(),
                    "\n",
                    Client._map.Name,
                    ": (",
                    Client._serverLocation.ToString(),
                    ")"
                }));
                Process.Start(text2);
                Client.DisconnectWait(false);
            }
        }

        private bool ShouldRequestRefresh()
        {
            return DateTime.UtcNow.Subtract(Client.LastStep).TotalSeconds > 20.0 &&
                   DateTime.UtcNow.Subtract(_lastKill).TotalSeconds > 20.0 &&
                   DateTime.UtcNow.Subtract(_lastRefresh).TotalSeconds > 30.0;
        }

        private void CheckAndHandleSpells()
        {
            if (Client.HasSpell("Lyliac Vineyard") && !Client.Spellbook["Lyliac Vineyard"].CanUse)
            {
                _lastVineCast = DateTime.UtcNow;
            }
        }

        private bool autoRedConditionsMet()
        {
           return Client.ClientTab!= null && _playersExistingOver250ms != null && Client.ClientTab.autoRedCbox.Checked;
        }

        private void HandleAutoRed()
        {
            if (_playersExistingOver250ms.Any(Delegates.PlayerIsSkulled))
            {
                _playersExistingOver250ms.RemoveAll(Delegates.PlayerIsSkulled);
                foreach (Player player in Client.GetNearbyPlayerList().Where(IsSkulledFriendOrGroupMember))
                {
                    _playersNeedingRed.Add(player);
                }
                _playersNeedingRed = _playersNeedingRed.OrderBy(DistanceFromPlayer).ToList();
            }
        }

        private bool IsSkulledFriendOrGroupMember(Player player)
        {
            return Client._groupBindingList.Concat(Client._friendBindingList).Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player.IsSkulled;
        }

        private int DistanceFromPlayer(Player player)
        {
            return Client._serverLocation.DistanceFrom(player.Location);
        }

        private void FilterStrangerPlayers()
        {
            var duplicateOrHiddenPlayers = new List<Player>();
            foreach (var player in _playersExistingOver250ms)
            {
                foreach (var otherPlayer in _playersExistingOver250ms)
                {
                    if (player != otherPlayer && (player.Location.Equals(otherPlayer.Location) || otherPlayer._isHidden) && !duplicateOrHiddenPlayers.Contains(player))
                    {
                        duplicateOrHiddenPlayers.Add(player);
                    }
                }
                foreach (var creature in _nearbyValidCreatures)
                {
                    if (creature != null && player.Location.Equals(creature.Location) && !duplicateOrHiddenPlayers.Contains(player))
                    {
                        duplicateOrHiddenPlayers.Add(player);
                    }
                }
            }
            _playersExistingOver250ms = _playersExistingOver250ms.Except(duplicateOrHiddenPlayers).ToList();
        }

        private void PerformSpellActions()
        {
            Loot();
            //ManageSpellCasting(); //Adam rework this, it breaks casting

            _autoStaffSwitch = Client.ClientTab.autoStaffCbox.Checked;
            _fasSpiorad = Client.HasEffect(EffectsBar.FasSpiorad) || (Client.HasSpell("fas spiorad") && DateTime.UtcNow.Subtract(Client.Spellbook["fas spiorad"].LastUsed).TotalSeconds < 1.5);
            _isSilenced = Client.HasEffect(EffectsBar.Silenced);

            AoSuain();
            AutoGem();

            UpdatePlayersListBasedOnStrangers();

            CheckFasSpioradRequirement();

            try
            {
                if (CastDefensiveSpells())
                {
                    CastOffensiveSpells();
                }
            }
            catch
            {
                // Consider logging or handling the exception as needed.
            }

            //ManageSpellCastingDelay();


            Client currentClient = Client;
            byte? castLinesCountNullable;

            if (currentClient == null)
            {
                castLinesCountNullable = null;
            }
            else
            {
                Spell currentSpell = currentClient._currentSpell;
                castLinesCountNullable = (currentSpell != null) ? new byte?(currentSpell.CastLines) : null;
            }

            byte? castLinesCount = castLinesCountNullable;
            int? castLines = (castLinesCount != null) ? new int?((int)castLinesCount.GetValueOrDefault()) : null;

            if (castLines.GetValueOrDefault() <= 0 & castLines != null)
            {
                //Console.WriteLine("Sleeping for 10ms");
                Thread.Sleep(330);
            }
        }
        private bool CastDefensiveSpells()
        {
            FasSpiorad();
            Hide();
            BubbleBlock(); //Adam, fix this, we are spamming buble block/shield
            Heal();
            DispellAllyCurse();
            Dion();
            Aite();
            Fas();
            DragonScale();
            Armachd();
            AiteAllies();
            FasAllies();
            ArmachdAllies();
            WakeScroll();
            BeagCradh();
            BeagCradhAllies();
            AoPoison();




            return true;
        }

        private void FasSpiorad()
        {
            if (_needFasSpiorad)
            {
                uint currentMP = Client.CurrentMP;
                DateTime startTime = DateTime.UtcNow;

                while (_needFasSpiorad)
                {
                    int fasSpioradThreshold;

                    bool isFasSpioradChecked = Client.ClientTab.fasSpioradCbox.Checked;
                    bool isThresholdParsed = int.TryParse(Client.ClientTab.fasSpioradText.Text.Trim(), out fasSpioradThreshold);
                    bool isBelowThreshold = Client.CurrentMP <= fasSpioradThreshold;

                    if (isFasSpioradChecked && isThresholdParsed && isBelowThreshold)
                    {
                        Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                    }

                    bool hasManaReachedHalf = Client.CurrentMP >= Client.Stats.MaximumMP / 2U;
                    bool hasManaIncreasedByFivePercent = Client.CurrentMP - currentMP >= Client.Stats.MaximumMP / 20U;
                    bool isDurationExceeded = DateTime.UtcNow.Subtract(startTime).TotalSeconds > 20.0;

                    if (hasManaReachedHalf || hasManaIncreasedByFivePercent || isDurationExceeded)
                    {
                        _needFasSpiorad = false;
                    }

                    Thread.Sleep(10);
                }
            }
        }

        private bool BeagCradh()
        {
            bool isBeagCradhChecked = Client.ClientTab.beagCradhCbox.Checked;
            bool isPlayerCursed = Client.Player.IsCursed;

            if (isBeagCradhChecked && !isPlayerCursed)
            {
                Client.UseSpell("beag cradh", Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool BeagCradhAllies()
        {

            foreach (Ally ally in ReturnAllyList())
            {
                bool isBeagCradhChecked = ally.AllyPage.dbBCCbox.Checked;

                if (isBeagCradhChecked && IsAlly(ally, out Player player, out Client client) && !player.IsCursed)
                {
                    Client.UseSpell("beag cradh", player, _autoStaffSwitch, false);
                    return false;
                }
            }

            return true;
        }

        private bool WakeScroll()
        {
            
            bool isWakeScrollChecked = Client.ClientTab.wakeScrollCbox.Checked;
            bool isRegistered = Client._isRegistered;

            if (isWakeScrollChecked && isRegistered && _nearbyAllies.Any(player => IsAllyAffectedByPramhOrAsleep(player)))
            {
                if (Client.UseItem("Wake Scroll"))
                {
                    foreach (Player player in _nearbyAllies)
                    {
                        Client client = _server.FindClientByName(player.Name);
                        if (client != null)
                        {
                            client.ClearEffect(EffectsBar.Pramh);
                        }
                    }
                }
                return false;
            }

            return true;
        }

        internal bool IsAllyAffectedByPramhOrAsleep(Player player)
        {
            if (!player.IsAsleep)
            {
                Client client = _server.FindClientByName(player.Name);
                return client != null && client.HasEffect(EffectsBar.Pramh);
            }
            return true;
        }

        private bool AoPoison()
        {
            bool isAoPoisonChecked = Client.ClientTab.aoPoisonCbox.Checked;
            bool isPlayerPoisoned = Client.Player.IsPoisoned;
            bool isFungusExtractChecked = Client.ClientTab.fungusExtractCbox.Checked;
            bool shouldUseFungusExtract = DateTime.UtcNow.Subtract(_lastUsedFungusBeetle).TotalSeconds > 1.0;

            //Ao allies
            foreach (Ally ally in ReturnAllyList())
            {
                bool isAoAllyChecked = ally.AllyPage.dispelPoisonCbox.Checked;

                if (isAoAllyChecked && IsAlly(ally, out Player player, out Client client) &&
                    client.HasEffect(EffectsBar.Poison) && player.IsPoisoned && shouldUseFungusExtract)
                {
                    if (Client._isRegistered && Client.HasItem("Fungus Beetle Extract"))
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            Client.UseItem("Fungus Beetle Extract");
                        }
                        _lastUsedFungusBeetle = DateTime.UtcNow;
                    }
                    else
                    {
                        Client.UseSpell("ao puinsein", player, _autoStaffSwitch, false);
                        return false;
                    }
                }
            }

            //Ao self
            if (isAoPoisonChecked && Client.HasEffect(EffectsBar.Poison) && isPlayerPoisoned)
            {
                if (isFungusExtractChecked && Client._isRegistered)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Client.UseItem("Fungus Beetle Extract");
                    }
                    _lastUsedFungusBeetle = DateTime.UtcNow;
                }
                else
                {
                    Client.UseSpell("ao puinsein", Client.Player, _autoStaffSwitch, false);
                    return false;
                }
            }

            return true;
        }

        private bool Aite()
        {
            bool isAiteChecked = Client.ClientTab.aiteCbox.Checked;
            bool isPlayerAited = Client.Player.IsAited;
            double aiteDuration = Client.Player.AiteDuration;
            string aiteSpell = Client.ClientTab.aiteCombox.Text;

            if (isAiteChecked && !Client.HasEffect(EffectsBar.NaomhAite) && (!isPlayerAited || aiteDuration != 2.0))
            {
                Client.UseSpell(aiteSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool AiteAllies()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isAiteChecked = ally.AllyPage.dbAiteCbox.Checked;
                string aiteSpell = ally.AllyPage.dbAiteCombox.Text;

                if (!isAiteChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.NaomhAite))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.IsAited)
                {
                    continue;
                }

                Client.UseSpell(aiteSpell, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool FasAllies()
        {

            foreach (Ally ally in ReturnAllyList())
            {
                bool isFasChecked = ally.AllyPage.dbFasCbox.Checked;
                string fasSpell = ally.AllyPage.dbFasCombox.Text;

                if (!isFasChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.FasNadur))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.IsAited)
                {
                    continue;
                }

                Client.UseSpell(fasSpell, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool Fas()
        {
            bool isFasChecked = Client.ClientTab.fasCbox.Checked;
            bool isPlayerFassed = Client.Player.IsFassed;
            double fasDuration = Client.Player.FasDuration;
            string fasSpell = Client.ClientTab.fasCombox.Text;

            if (isFasChecked && !Client.HasEffect(EffectsBar.FasNadur) && (!isPlayerFassed || fasDuration != 2.0))
            {
                Client.UseSpell(fasSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool DragonScale()
        {
            bool isDragonScaleChecked = Client.ClientTab.dragonScaleCbox.Checked;

            if (isDragonScaleChecked && Client._isRegistered && !Client.HasEffect(EffectsBar.Armachd))
            {
                if (!RecentlyUsedDragonScale)
                {
                    RecentlyUsedDragonScale = true;

                    Console.WriteLine("[DragonScale] Using Dragon's Scale");
                    
                    Client.UseItem("Dragon's Scale");

                    Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => RecentlyUsedDragonScale = false);

                    return false;
                }

            }

            return true;
        }

        private bool Dion()
        {
            bool isDionChecked = Client.ClientTab.dionCbox.Checked;

            if (!isDionChecked || Client.HasEffect(EffectsBar.Dion))
            {
                return false; // Exit early if Dion is not checked or effect already exists
            }

            string dionWhen = Client.ClientTab.dionWhenCombox.Text;
            bool shouldUseSpell = false;

            switch (dionWhen)
            {
                case "Always":
                    shouldUseSpell = true;
                    break;
                case "In Danger":
                    shouldUseSpell = _nearbyValidCreatures.Count > 0;
                    break;
                case "Taking Damage":
                    shouldUseSpell = Client.CurrentHP < Client.MaximumHP;
                    break;
                case "At Percent":
                    shouldUseSpell = Client.CurrentHP * 100U / Client.MaximumHP < Client.ClientTab.dionPctNum.Value;
                    break;
                case "Green Not Nearby":
                    shouldUseSpell = !_nearbyValidCreatures.Any(Delegates.isGreenMantis);
                    break;
            }

            if (shouldUseSpell || (Client.ClientTab.aoSithCbox.Checked && _recentlyAoSithed))
            {
                UseDionOrStone();
                return false; // Spell used, exit the method
            }

            // Reset Ao Sith flag if Dion effect is present
            if (Client.HasEffect(EffectsBar.Dion))
            {
                _recentlyAoSithed = false;
            }

            return true; // No spell used, continue execution
        }

        private bool Armachd()
        {
            bool isArmachdChecked = Client.ClientTab.armachdCbox.Checked;
            
            if (isArmachdChecked && !Client.HasEffect(EffectsBar.Armachd))
            {
                Client.UseSpell("armachd", Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool ArmachdAllies()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isArmachdChecked = ally.AllyPage.dbArmachdCbox.Checked;

                if (!isArmachdChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.Armachd))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.HasArmachd)
                {
                    continue;
                }

                Client.UseSpell("armachd", player, _autoStaffSwitch, false);

                return false;

            }

            return true;
        }

        private void UseDionOrStone()
        {
            string dionSpell = Client.ClientTab.dionCombox.Text;

            if (dionSpell == "Glowing Stone" && !RecentlyUsedGlowingStone)
            {
                RecentlyUsedGlowingStone = true;

                Client.UseItem("Glowing Stone");

                Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => RecentlyUsedGlowingStone = false);
            }
            else if (dionSpell != "Glowing Stone")
            {
                Client.UseSpell(dionSpell, null, _autoStaffSwitch, false);
            }
        }

        private bool DispellAllyCurse()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isDispelCurseChecked = ally.AllyPage.dispelCurseCbox.Checked;

                if (isDispelCurseChecked && TryGetCursedAlly(ally, out Player player, out Client client))
                {

                    var cursesToDispel = new HashSet<string> { "cradh", "mor cradh", "ard cradh" };

                    if (cursesToDispel.Contains(player.Curse))
                    {
                        Client.UseSpell("ao " + player.Curse, player, _autoStaffSwitch, true);

                        player.CurseDuration = 0.0;
                        player.Curse = "";
                        Console.WriteLine($"[DispellAllyCurse] Curse data reset on {player.Name}, Hash: {player.GetHashCode()}. Curse: {player.Curse}, CurseDuration: {player.CurseDuration}, IsCursed: {player.IsCursed}");
                        
                        return false;

                    }
                }
            }
            return true;
        }
        
        private bool TryGetCursedAlly(Ally ally, out Player player, out Client client)
        {

            if (IsAlly(ally, out player, out client))
            {
                Console.WriteLine($"[TryGetCursedAlly] Player.ID: {player.ID}, Hash: {player.GetHashCode()}, Player {player.Name} is cursed: {player.IsCursed}");
                return player.IsCursed;
            }
            return false;
        }

        internal bool IsAlly(Ally ally, out Player player, out Client client)
        {
            player = Client.GetNearbyPlayer(ally.Name);

            if (player == null)
            {
                client = null;
                return false;
            }

            client = Server.FindClientByName(ally.Name);

            if (client != null && client != Client)
            {
                return true;
            }

            return false;
        }

        private bool Heal()
        {
            if (Client.HasEffect(EffectsBar.FasSpiorad))
            {
                return false;
            }

            int loopPercentThreshold = 20;

            while (loopPercentThreshold <= 100 && !_needFasSpiorad)
            {
                foreach (Player player in Client.GetNearbyPlayerList())
                {
                    if (IsAllyAlreadyListed(player.Name) || player == Client.Player)
                    {
                        Ally ally = ReturnAllyList().FirstOrDefault(a => a.Name == player.Name);
                        AllyPage allyPage = ally?.AllyPage;
                        Client client = Server.FindClientByName(player.Name);

                        if (client == null) continue;

                        if ((allyPage == null && client != Client) || (client == Client && !Client.ClientTab.healCbox.Checked) || (client != Client && !allyPage.dbIocCbox.Checked))
                        {
                            continue;
                        }

                        if (ShouldExcludePlayer(player)) continue;

                        string healSpell = player == client.Player ? client.ClientTab.healCombox.Text : allyPage.dbIocCombox.Text;

                        //Console.WriteLine($"[CastDefensiveSpells] heal spell: {healSpell}");

                        if (loopPercentThreshold == 20 && player != client.Player && (client.HasSpell("ard ioc comlha") || client.HasSpell("mor ioc comlha")))
                        {

                            int alliesInNeed = Client.GetNearbyAllies().Count(p => p != Client.Player && IsAllyInNeed(p));

                            if (alliesInNeed > 2)
                            {
                                healSpell = Client.HasSpell("ard ioc comlha") ? "ard ioc comlha" : "mor ioc comlha";
                            }
                        }

                        if (!Client.GetNearbyPlayerList().Any(player => ShouldExcludePlayer(player)) || player == Client.Player || healSpell.Contains("comlha"))
                        {
                            
                            int healAtPercent = (int)((player == Client.Player) ? Client.ClientTab.healPctNum.Value : allyPage.dbIocNumPct.Value);
                            healAtPercent = ((healAtPercent > loopPercentThreshold) ? loopPercentThreshold : healAtPercent);
                            bool shouldHeal = player.NeedsHeal || (client.CurrentHP * 100 / client.MaximumHP) <= healAtPercent;

                            if (player.NeedsHeal || shouldHeal)
                            {

                                uint healAmount = (uint)Client.CalculateHealAmount(healSpell);

                                List<Player> playersHealed = new List<Player>();

                                if (!(healSpell == "ard ioc comlha") && !(healSpell == "mor ioc comlha"))
                                {
                                    if (Client.UseSpell(healSpell, player, _autoStaffSwitch, false))
                                    {
                                        playersHealed.Add(player);
                                    }
                                }
                                else if (Client.UseSpell(healSpell, null, _autoStaffSwitch, false))
                                {
                                    playersHealed.AddRange(Client.GetNearbyAllies());
                                }
                                foreach (Player p in playersHealed)
                                {

                                    if (((client != null) ? client.Player : null) == player)
                                    {

                                        // Calculate the new health, ensuring it does not exceed the maximum
                                        uint newHealth = Math.Min(client.CurrentHP + healAmount, client.MaximumHP);

                                        // Update the health and print the debug message
                                        client.CurrentHP = newHealth;
                                        Console.WriteLine($"[Heal] {client.Name} healed for {healAmount} HP. New HP: {client.CurrentHP} / {client.MaximumHP}");
                                        // Update health percentage and needs heal status
                                        p.HealthPercent = (byte)(client.CurrentHP * 100 / client.MaximumHP);
                                        p.NeedsHeal = p.HealthPercent <= healAtPercent;

                                    }
                                    else
                                    {
                                        // We don't have access to the actual health value, so we'll just update it based on a health percentage
                                        // Calculate the new health percentage based on loopPercentThreshold, ensuring it does not exceed 100%
                                        byte newHealthPercent = (byte)Math.Min(p.HealthPercent + 20, 100);

                                        // Update health percentage and needs heal status
                                        p.HealthPercent = newHealthPercent;
                                        p.NeedsHeal = p.HealthPercent <= healAtPercent;
                                    }                                    

                                }
                            }
                        }
                    }
                }
               
                loopPercentThreshold += 20;
            }
            return true;
        }

        private bool IsAllyInNeed(Player player)
        {
            return player.HealthPercent < 20 || player.NeedsHeal;
        }

        internal bool ShouldExcludePlayer(Player player)
        {
            // Checks if playerToCheck is not in friend list, is not the reference player,
            // and is either at the same location as reference player or is hidden
            return !Client.ClientTab.friendList.Items.OfType<string>().Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player != Client.Player && (Equals(player.Location, Client.Player.Location) || player._isHidden);
        }

        private bool BubbleBlock()
        {
            bool isBubbleBlockChecked = Client.ClientTab.bubbleBlockCbox.Checked;
            bool isSpamBubbleChecked = Client.ClientTab.spamBubbleCbox.Checked;
            bool isFollowChecked = Client.ClientTab.followCbox.Checked;
            string walkMap = Client.ClientTab.walkMapCombox.Text;

            if (isBubbleBlockChecked && isSpamBubbleChecked)
            {
                if (Client.UseSpell("Bubble Block", null, true, true))
                {
                    return false;
                }
            }
            else if (isBubbleBlockChecked && Client.bool_41)
            {
                if (walkMap == "WayPoints")
                {
                    if (CastBubbleBlock())
                    {
                        return false;
                    }
                }
                else if (isFollowChecked && Client._distnationReached && CastBubbleBlock())
                {
                    return false;
                }
            }

            return true;
        }
        private bool CastBubbleBlock()
        {
            //ADAM FIX THIS IT SPAMS BUBBLE SHIELD

            // Check if the player has moved since the last bubble was cast.
            bool hasMoved = !Location.Equals(_lastBubbleLocation, Client._serverLocation);

            // Define the preferred order of bubble spells.
            var bubbleSpells = new[] { "Bubble Block", "Bubble Shield" };

            // Attempt to cast a bubble spell if the player has moved or the current bubble type is not set.
            if (hasMoved || string.IsNullOrEmpty(_bubbleType))
            {
                foreach (var spellName in bubbleSpells)
                {
                    if (Client.HasSpell(spellName) && Client.CanUseSpell(Client.Spellbook[spellName], null))
                    {
                        Client.UseSpell(spellName, null, _autoStaffSwitch, true);
                        _lastBubbleLocation = Client._serverLocation;
                        _bubbleType = spellName.ToUpperInvariant().Contains("BLOCK") ? "BLOCK" : "SHIELD";
                        return true;
                    }
                }
            }
            else
            {
                // If the player hasn't moved, attempt to refresh the current bubble type without the "true" force argument.
                string spellToCast = _bubbleType == "BLOCK" ? "Bubble Block" : "Bubble Shield";
                if (Client.HasSpell(spellToCast) && Client.CanUseSpell(Client.Spellbook[spellToCast], null))
                {
                    if (Client.UseSpell(spellToCast, null, _autoStaffSwitch, false))
                    _lastBubbleLocation = Client._serverLocation;
                    return true;

                }
            }

            return false;
        }

        private bool Hide()
        {
            if (CastHide())
            {
                bool_13 = true;
                return false;
            }

            return true;
        }
        private bool CastHide()
        {
            bool isHideChecked = Client.ClientTab.hideCbox.Checked;
            bool canUseSpells = Client._map.CanUseSpells;

            if (isHideChecked && canUseSpells)
            {
                string spellName = "";
                if (Client.HasSpell("Hide"))
                {
                    spellName = "Hide";
                }
                else if (Client.HasSpell("White Bat Stance"))
                {
                    spellName = "White Bat Stance";
                }
                if (!Client.HasEffect(EffectsBar.Hide) || DateTime.UtcNow.Subtract(Client._lastHidden).TotalSeconds > 50.0)
                {
                    Client.UseSkill("Assail");
                    Client.UseSpell(spellName, null, true, true);
                    Client._lastHidden = DateTime.UtcNow;
                }
                return true;
            }
            return false;
        }

        private bool CastOffensiveSpells()
        {
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);

            if (_nearbyValidCreatures.Count > 0)
            {
                _nearbyValidCreatures = _nearbyValidCreatures.OrderBy(Delegates.NextRandom)
                                                           .ToList();
            }
            if (IsStrangerNearby() && _nearbyValidCreatures.Count > 0)
            {
                _nearbyValidCreatures.RemoveAll(Delegates.CreaturesExisitingLessThan2s);
            }
            if (IsStrangerNearby()) //we don't want to cast on overlapped creatures if a stranger is nearby
            {
                var duplicates = _nearbyValidCreatures
                    .SelectMany((creature, index) => _nearbyValidCreatures
                        .Where((otherCreature, otherIndex) => index != otherIndex && creature.Location == otherCreature.Location)
                        .Take(1))
                    .Distinct()
                    .ToList();

                foreach (var duplicate in duplicates)
                {
                    _nearbyValidCreatures.Remove(duplicate);
                }
            }
            if (EnemyPage != null)
            {
                if (EnemyPage.ignoreCbox.Checked)
                {
                    _nearbyValidCreatures.RemoveAll(CreaturesToIgnore);
                }
                if (EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in _nearbyValidCreatures)
                    {
                        if (EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (EnemyPage.spellAllRbtn.Checked)
                    {
                        creature = null;
                        if (priority.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, priority))
                        {
                            bool_13 = true;
                            return true;
                        }
                        if (nonPriority.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, nonPriority))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (priority.Count > 0 && SpellOneAtATime(EnemyPage, priority))
                        {
                            bool_13 = true;
                            return true;
                        }
                        if (!priority.Contains(creature) && nonPriority.Count > 0 && SpellOneAtATime(EnemyPage, nonPriority))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
                //Spell all at once
                else if (EnemyPage.spellAllRbtn.Checked)
                {
                    creature = null;
                    if (_nearbyValidCreatures.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, _nearbyValidCreatures))
                    {
                        bool_13 = true;
                        return true;
                    }
                }
                //Spell one at a time
                else if (_nearbyValidCreatures.Count > 0 && SpellOneAtATime(EnemyPage, _nearbyValidCreatures))
                {
                    bool_13 = true;
                    return true;
                }
            }
            else
            {
                List<Creature> creatureList = new List<Creature>();
                foreach (Enemy enemy in ReturnEnemyList())
                {
                    creatureList.Clear();
                    foreach (Creature creature in Client.GetCreaturesInRange(11, new ushort[] { enemy.SpriteID }))
                    {
                        if (creature.SpriteID.ToString() == enemy.SpriteID.ToString())
                        {
                            creatureList.Add(creature);
                        }
                    }
                    creatureList.OrderBy(new Func<Creature, Creature>(Delegates.IsCreature));
                    if (creatureList.Count > 0)
                    {
                        if (enemy.EnemyPage.spellAllRbtn.Checked)
                        {
                            creature = null;
                            if (DecideAndExecuteEngagementStrategy(enemy.EnemyPage, creatureList))
                            {
                                bool_13 = true;
                                return true;
                            }
                        }
                        else if (SpellOneAtATime(enemy.EnemyPage, creatureList))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
            }
            if (EnemyPage != null)
            {
                if (EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in _nearbyValidCreatures)
                    {
                        if (EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (CastAttackSpell(EnemyPage, priority))
                    {
                        return true;
                    }
                    if (CastAttackSpell(EnemyPage, nonPriority))
                    {
                        return true;
                    }
                }
                else if (CastAttackSpell(EnemyPage, _nearbyValidCreatures))
                {
                    return true;
                }
            }
            else
            {
                List<Creature> list7 = new List<Creature>();
                foreach (Enemy enemy in ReturnEnemyList())
                {
                    list7.Clear();
                    foreach (Creature class8 in Client.GetCreaturesInRange(11, new ushort[]
                    {
                    enemy.SpriteID
                    }))
                    {
                        if (class8.SpriteID.ToString() == enemy.SpriteID.ToString())
                        {
                            list7.Add(class8);
                        }
                    }
                    list7.OrderBy(new Func<Creature, Creature>(Delegates.IsCreature));
                    if (CastAttackSpell(enemy.EnemyPage, list7))
                    {
                        return true;
                    }
                }
            }
            bool_13 = false;
            return false;
        }

        private bool CastAttackSpell(EnemyPage enemyPage, List<Creature> creatureList)
        {
            bool result = false;

            if (!enemyPage.targetCbox.Checked || creatureList.Count == 0)
            {
                return false;
            }

            if (enemyPage.spellOneRbtn.Checked)
            {
                if (!creatureList.Contains(creature))
                {
                    creature = creatureList.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (creature == null)
                {
                    return false;
                }
                creatureList = new List<Creature>
                {
                    creature
                };
            }
            if (EnemyPage != null && EnemyPage.mpndDioned.Checked)
            {
                if (!enemyPage.attackCboxOne.Checked && !enemyPage.attackCboxTwo.Checked)
                {
                    if (!_nearbyValidCreatures.Any(new Func<Creature, bool>(Delegates.IsDioned)))
                    {
                        goto IL_1A5;
                    }
                }
                else if (!_nearbyValidCreatures.All(new Func<Creature, bool>(Delegates.CanCastPND)))
                {
                    goto IL_1A5;
                }
                Creature creatureTarget = _nearbyValidCreatures.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault(new Func<Creature, bool>(Delegates.CanCastPND));
                if (creatureTarget != null)
                {
                    return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, creatureTarget, _autoStaffSwitch, false);
                }
            }
        IL_1A5:
            if (!_isSilenced)
            {
                if (enemyPage.attackCboxTwo.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxTwo.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxTwo.Text;
                        uint fnvHash = Utility.CalculateFNV(spellName);

                        switch (fnvHash)
                        {
                            case 1007116742U: // Supernova Shot
                                return Client.UseSpell("Supernova Shot", target, _autoStaffSwitch, false);

                            case 1285349432U: // Shock Arrow
                                return Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false) && 
                                       Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false);

                            case 1792178996U: // Volley
                                return Client.UseSpell("Volley", target, _autoStaffSwitch, true);

                            case 2591503996U: // MSPG
                                if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
                                {
                                    _manaLessThanEightyPct = true;
                                    Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                                    return true;
                                }
                                return Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true);


                            case 3122026787U: // Unholy Explosion
                                return Client.UseSpell("Unholy Explosion", target, _autoStaffSwitch, false);

                            case 3210331623U: // Cursed Tune
                                return TryCastAnyRank("Cursed Tune", target, _autoStaffSwitch, false);

                            case 3848328981U: // M/DSG
                                return Client.UseSpell("mor deo searg gar", Client.Player, _autoStaffSwitch, false) || 
                                       Client.UseSpell("deo searg gar", Client.Player, _autoStaffSwitch, false);        
                    }
                }
                }
                if (enemyPage.attackCboxOne.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxOne.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxOne.Text;
                        switch (spellName)
                        {
                            case "lamh":
                                return Client.TryUseAnySpell(new[] { "beag athar lamh", "beag srad lamh", "athar lamh", "srad lamh", "Howl" }, null, _autoStaffSwitch, false);

                            case "A/DS":
                                return Client.TryUseAnySpell(new[] { "ard deo searg", "deo searg" }, target, _autoStaffSwitch, false);

                            case "A/M/PND":
                                return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, target, _autoStaffSwitch, false);

                            case "Frost Arrow":
                                //return TryCastAnyRank("Frost Arrow", target, _autoStaffSwitch, false);
                                return Client.UseSpell("Frost Arrow 4", target, _autoStaffSwitch, false);//Adam fix. Not casting frost arrow

                            default:
                                return Client.UseSpell(spellName, target, _autoStaffSwitch, false) || TryCastAnyRank(spellName, target, _autoStaffSwitch, false);
                        }
                    }
                }
            }
            if (_isSilenced && enemyPage.mpndSilenced.Checked)
            {
                Creature mpndTarget = SelectAttackTarget(enemyPage, creatureList, "A/M/PND");
                return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, mpndTarget, _autoStaffSwitch, false);
            }
            if (!_isSilenced || !enemyPage.mspgSilenced.Checked || SelectAttackTarget(enemyPage, creatureList, "MSPG") == null)
            {
                return result;
            }
            if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
            {
                _manaLessThanEightyPct = true;
                return Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false); ;
            }
            Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true); //Adam check this
            return true;
        }

        private bool TryCastAnyRank(string spellNAme, Creature creatureTarget = null, bool autoStaffSwitch = true, bool keepSpellAfterUse = true)
        {
            for (int i = 20; i > 0; i--)
            {
                if (Client.UseSpell(spellNAme + " " + i.ToString(), creatureTarget, autoStaffSwitch, keepSpellAfterUse))
                {
                    return true;
                }
            }
            return false;
        }


        private bool SpellOneAtATime(EnemyPage enemyPage, List<Creature> creatureList)
        {
            foreach (var creature in creatureList.OrderBy(c => DistanceFromClientLocation(c)))
            {

                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (creature == null)
                {
                    continue; 
                }
                this.creature = creature;

                if (enemyPage.pramhFirstRbtn.Checked && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, _autoStaffSwitch, false);
                    return true;
                }
                if (enemyPage.spellFirstRbtn.Checked && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.pramhFirstRbtn.Checked && (!enemyPage.spellsControlCbox.Checked || creature.IsAsleep) && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.spellFirstRbtn.Checked && (!enemyPage.spellsFasCbox.Checked || creature.IsFassed) && (!enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, _autoStaffSwitch, false);
                    return true;
                }
            }

            // If the method reaches this point, it means no spells were cast successfully on any of the creatures
            return false;
        }

        private bool CastFasOrCurse(EnemyPage enemyPage, Creature creature)
        {
            if (enemyPage.fasFirstRbtn.Checked && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if (enemyPage.curseFirstRbtn.Checked && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.fasFirstRbtn.Checked || !enemyPage.spellsFasCbox.Checked || creature.IsFassed) && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.curseFirstRbtn.Checked || !enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            return false;
        }


        private bool DecideAndExecuteEngagementStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            if (enemyPage.pramhFirstRbtn.Checked)
            {
                if (enemyPage.spellsControlCbox.Checked)
                    if (ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            else if (enemyPage.spellFirstRbtn.Checked)
            {
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsControlCbox.Checked)
                    if (ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            return false;
        }

        private bool ExecuteDebuffStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            List<Creature> eligibleCreatures = creatureList.Where(Delegates.IsNotFassedOrNotCursed).ToList();
            if (eligibleCreatures != null && eligibleCreatures.Any() && (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked))
            {
                if (enemyPage.fasFirstRbtn.Checked)
                {
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
                else if (enemyPage.curseFirstRbtn.Checked)
                {
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private bool CastCurseIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (_lock)
            {
                Console.WriteLine($"[CastCurseIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not cursed
                var eligibleCreatures = creatures.Where(Delegates.IsNotCursed);

                Console.WriteLine($"[CastCurseIfApplicable] Eligible creatures (not cursed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting curses is enabled, cast the curse spell
                if (targetCreature != null && enemyPage.spellsCurseCbox.Checked)
                {
                    Console.WriteLine($"[CastCurseIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastCursed: {targetCreature.LastCursed}, IsCursed: {targetCreature.IsCursed}");
                    Client.UseSpell(enemyPage.spellsCurseCombox.Text, targetCreature, _autoStaffSwitch, false);
                    bool_13 = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken

            }
        }

        private bool CastFasIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (_lock)
            {
                Console.WriteLine($"[CastFasIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not fassed
                var eligibleCreatures = creatures.Where(Delegates.IsNotFassed);

                Console.WriteLine($"[CastFasIfApplicable] Eligible creatures (not fassed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting the 'fas' spell is enabled, cast the spell
                if (targetCreature != null && enemyPage.spellsFasCbox.Checked)
                {
                    Console.WriteLine($"[CastFasIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastFassed: {targetCreature.LastFassed}, IsFassed: {targetCreature.IsFassed}");
                    Client.UseSpell(enemyPage.spellsFasCombox.Text, targetCreature, _autoStaffSwitch, false);
                    bool_13 = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken
            }
          
        }

        private Creature SelectAttackTarget(EnemyPage enemyPage, List<Creature> creatureList, string spellName = "")
        {
            List<Creature> attackList = new List<Creature>();

            bool flag = enemyPage.attackCboxTwo.Checked && enemyPage.targetCbox.Checked && 
                        (enemyPage.attackComboxTwo.Text == "MSPG" || enemyPage.attackComboxTwo.Text == "M/DSG");


            if (enemyPage.targetCursedCbox.Checked && enemyPage.targetFassedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsCursed && creature.IsFassed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (enemyPage.targetCursedCbox.Checked && !enemyPage.targetFassedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsCursed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (enemyPage.targetFassedCbox.Checked && !enemyPage.targetCursedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsFassed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (!enemyPage.targetCursedCbox.Checked && !enemyPage.targetFassedCbox.Checked)
            {
                attackList = creatureList;
            }

            if (attackList.Count == 0)
            {
                return null;
            }
            if (!(spellName == "Frost Arrow"))
            {
                if (!(spellName == "Cursed Tune"))
                {
                    if (!(spellName == "lamh") && !(spellName == "Shock Arrow") && !(spellName == "Volley"))
                    {
                        if (spellName != "A/M/PND")
                        {
                            attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotDioned)).ToList<Creature>();
                        }
                        if (enemyPage.expectedHitsNum.Value > 0m)
                        {
                            attackList = attackList.Where(creature => CalculateHitCounter(creature, enemyPage)).ToList<Creature>();
                        }
                    }
                }
                else
                {
                    attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotPoisoned)).ToList<Creature>();
                }
            }
            else
            {
                attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotFrozen)).ToList<Creature>();
            }
            if (attackList.Count == 0)
            {
                return null;
            }
            if (creature != null && attackList.Contains(creature))
            {
                return creature;
            }
            if (!(enemyPage.targetCombox.Text == "Nearest") && (!_isSilenced || !enemyPage.mpndSilenced.Checked))
            {
                if (!enemyPage.targetCombox.Text.Contains("Cluster") || flag)
                {
                    return null;
                }
                int maxDistance = (enemyPage.targetCombox.Text == "Cluster 29") ? 3 : ((enemyPage.targetCombox.Text == "Cluster 13") ? 2 : 1);
                Dictionary<Creature, int> dictionary = new Dictionary<Creature, int>();
                List<Creature> list2 = Client.GetListOfValidCreatures(12).Concat(_playersExistingOver250ms).ToList<Creature>();
                List<Creature> list3 = Client.GetNearbyValidCreatures(12);
                List<Creature> list4 = list3.Where(creature => CONSTANTS.GREEN_BOROS.Contains(creature.SpriteID) || CONSTANTS.RED_BOROS.Contains(creature.SpriteID)).ToList();
                if (!list2.Contains(Client.Player))                {
                    list2.Add(Client.Player);
                }
                foreach (Creature c in list2)
                {
                    int num2 = 0;
                    foreach (Creature creature in attackList)
                    {
                        if (c.Location.DistanceFrom(creature.Location) <= maxDistance || (maxDistance > 1 && IsDiagonallyAdjacent(c.Location.Point, creature.Location.Point, maxDistance)))
                        {
                            num2++;
                        }
                    }
                    bool flag2 = false;
                    using (List<Creature>.Enumerator enumerator2 = list4.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Creature creature = enumerator2.Current;
                            if (c == creature)
                            {
                                flag2 = true;
                                break;
                            }
                            foreach (Location loc in Client.GetAdjacentPoints(creature))
                            {
                                if (c.Location.DistanceFrom(loc) <= maxDistance || IsDiagonallyAdjacent(c.Location.Point, loc.Point, maxDistance))
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                        }
                        goto IL_5EA;
                    }
                IL_5DA:
                    dictionary.Add(c, num2);
                    continue;
                IL_5EA:
                    if (!flag2)
                    {
                        goto IL_5DA;
                    }
                }
                foreach (Creature creature in dictionary.Keys.ToList<Creature>())
                {
                    if ((creature.Type == CreatureType.Aisling || (creature.Type == CreatureType.WalkThrough && !list3.Contains(creature)) || creature.IsDioned) && dictionary[creature] <= 1)
                    {
                        dictionary.Remove(creature);
                    }
                }
                if (dictionary.Count <= 0)
                {
                    return null;
                }
                return dictionary.OrderByDescending(new Func<KeyValuePair<Creature, int>, int>(Delegates.KVPMatch)).ThenBy(new Func<KeyValuePair<Creature, int>, DateTime>(Delegates.KVPCreation)).First<KeyValuePair<Creature, int>>().Key;
            }
            else
            {
                if (attackList.Count <= 0)
                {
                    return null;
                }
                return attackList.OrderBy(creature => DistanceFromServerLocation(creature)).FirstOrDefault();

            }
        }

        private bool IsDiagonallyAdjacent(Point location, Point otherLocation, int maxDistance)
        {
            int adjustedDistance = maxDistance - 1;
            int deltaX = Math.Abs(location.X - otherLocation.X);
            int deltaY = Math.Abs(location.Y - otherLocation.Y);

            return deltaX == adjustedDistance && deltaY == adjustedDistance;
        }

        internal bool CalculateHitCounter(Creature creature, EnemyPage enemyPage)
        {
            if (creature.HealthPercent == 0 && creature.SpellAnimationHistory.Count != 0 && creature._animation != 33 && DateTime.UtcNow.Subtract(creature._lastUpdate).TotalSeconds <= 1.5)
            {
                return creature._hitCounter < enemyPage.expectedHitsNum.Value;
            }
            if (creature.HealthPercent != 0 && creature._hitCounter > enemyPage.expectedHitsNum.Value)
            {
                creature._hitCounter = (int)enemyPage.expectedHitsNum.Value - 1;
            }
            return true;
        }

        private int GetFurthestClient()
        {
            int result;
            try
            {
                List<Client> nearbyClients = Client.GetNearbyPlayerList()
                    .Select(player => Server.FindClientByName(player?.Name))
                    .Where(client => client != null)
                    .ToList();

                if (nearbyClients.Any())
                {
                    Client furthestClient = nearbyClients
                        .OrderByDescending(client => CalculateDistanceFromBaseClient(client))
                        .First();

                    result = 11 - furthestClient._serverLocation.DistanceFrom(Client._serverLocation);
                }
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                Console.WriteLine($"Error in GetFurthestClient: {ex.Message}");
            }

            return 11; // Default value if no nearby clients or an exception occurs
        }

        private int CalculateDistanceFromBaseClient(Client client)
        {
            return client._serverLocation.DistanceFrom(Client._serverLocation);
        }

        private bool ExecutePramhStrategy(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> creatureList = FilterCreaturesByControlStatus(enemyPage, creatures);
            if (CONSTANTS.GREEN_BOROS.Contains(enemyPage.Enemy.SpriteID))
            {
                List<Creature> greenBorosInRange = Client.GetCreaturesInRange(8, CONSTANTS.GREEN_BOROS.ToArray());
                foreach (Creature creature in greenBorosInRange.ToList<Creature>())
                {
                    foreach (Location location in Client.GetObstacleLocations(new Location(Client._map.MapID, 0, 0)))
                    {
                        if (creature.Location.DistanceFrom(location) <= 3)
                        {
                            greenBorosInRange.Remove(creature);
                        }
                    }
                }
                creatureList.AddRange(greenBorosInRange);
            }
            Creature targetCreature = creatureList.FirstOrDefault<Creature>();
            Console.WriteLine($"[ExecutePramhStrategy] Attempting to cast 'pramh' on creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, IsAsleep: {targetCreature.IsAsleep}");
            if (targetCreature != null && creatureList.Any() && enemyPage.spellsControlCbox.Checked && !targetCreature.IsAsleep)
            {
                Console.WriteLine($"[ExecutePramhStrategy] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastPramhd: {DateTime.UtcNow}");
                Client.UseSpell(enemyPage.spellsControlCombox.Text, creatureList.FirstOrDefault<Creature>(), _autoStaffSwitch, false);
                return true;
            }
            return false;
        }

        private List<Creature> SelectCreaturesForPramh(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> selectedCreatures = FilterCreaturesByControlStatus(enemyPage, creatures);

            if (CONSTANTS.GREEN_BOROS.Contains(enemyPage.Enemy.SpriteID))
            {
                var additionalCreatures = Client.GetCreaturesInRange(8, CONSTANTS.GREEN_BOROS.ToArray())
                    .Where(creature => !Client.GetObstacleLocations(new Location(Client._map.MapID, 0, 0))
                    .Any(location => creature.Location.DistanceFrom(location) <= 3))
                    .ToList();

                selectedCreatures.AddRange(additionalCreatures);
            }

            return selectedCreatures;
        }

        private List<Creature> FilterCreaturesByControlStatus(EnemyPage enemyPage, List<Creature> creatureListIn)
        {
            bool isSuainSelected = enemyPage.spellsControlCombox.Text.Equals("suain", StringComparison.OrdinalIgnoreCase);

            return creatureListIn.Where(creature =>
                isSuainSelected ? !creature.IsSuained : !creature.IsAsleep || enemyPage.spellsControlCombox.Text != "suain"
            ).ToList();
        }

        private bool CreaturesToIgnore(Creature creature)
        {
            return EnemyPage.ignoreLbox.Items.Contains(creature.SpriteID.ToString());
        }

        private int DistanceFromServerLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._serverLocation);
        }

        private int DistanceFromClientLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._clientLocation);
        }

        private void AoSuain()
        {
            // Check if Ao Suain is enabled and the Suain effect is present before attempting to cast spells.
            if (!Client.ClientTab.aoSuainCbox.Checked || !Client.HasEffect(EffectsBar.Suain))
            {
                return;
            }

            // Attempt to cast "Leafhopper Chirp" first. If it fails, attempt to cast "ao suain".
            // Only clear the Suain effect if one of the spells is successfully cast.
            if (Client.UseSpell("Leafhopper Chirp", null, false, false) || Client.UseSpell("ao suain", Client.Player, false, true))
            {
                Client.ClearEffect(EffectsBar.Suain);
            }
        }

        private void AutoGem()
        {
            bool shouldUseGem = Client.ClientTab.autoGemCbox.Checked &&
                                Client.Experience > 4250000000U &&
                                DateTime.UtcNow.Subtract(_lastUsedGem).TotalMinutes > 5.0;

            if (!shouldUseGem)
            {
                return;
            }

            // Determine the gem type based on the selected text, then use the gem.
            byte choice = Client.ClientTab.expGemsCombox.Text == "Ascend HP" ? (byte)1 : (byte)2;
            Client.UseExperienceGem(choice);
        }

        private void UpdatePlayersListBasedOnStrangers()
        {
            _playersExistingOver250ms = !IsStrangerNearby() ? _playersExistingOver250ms : _playersExistingOver250ms.Where(Delegates.HasPlayerExistedForOver2s).ToList();
        }

        private void CheckFasSpioradRequirement()
        {
            int requiredMp;

            if (Client.ClientTab.fasSpioradCbox.Checked && int.TryParse(Client.ClientTab.fasSpioradText.Text.Trim(), out requiredMp) && Client.ManaPct < requiredMp)
            {
                _needFasSpiorad = true;
            }
        }

        private void ManageSpellCasting()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (Client._creatureToSpellList.Count >= 3 || Client._spellCounter >= 3)
            {
                if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds > 1.0)
                {
                    Client._creatureToSpellList.Clear();
                }
                Thread.Sleep(10);
            }
            if (DateTime.UtcNow.Subtract(_lastCast).TotalSeconds > 1.0)
            {
                Client._creatureToSpellList.Clear();
            }
        }

        private void ManageSpellCastingDelay()
        {
            var spellCastLines = Client?._currentSpell?.CastLines ?? 0;
            if (spellCastLines <= 0)
            {
                //Thread.Sleep(10); // Adjust the sleep time as needed
            }
        }

        internal bool IsAllyAlreadyListed(string name)
        {
            lock (_lock)
            {
                return _allyListName.Contains(name, StringComparer.CurrentCultureIgnoreCase);
            }
        }

        internal bool IsEnemyAlreadyListed(ushort sprite)
        {

            lock (_lock)
            {
                return _enemyListID.Contains(sprite);
            }
        }

        internal void UpdateAllyList(Ally ally)
        {
            lock (_lock)
            {
                _allyList.Add(ally);
                _allyListName.Add(ally.Name);
            }
        }

        internal void UpdateEnemyList(Enemy enemy)
        {
            lock (_lock)
            {
                _enemyList.Add(enemy);
                _enemyListID.Add(enemy.SpriteID);
            }
        }

        internal void RemoveAlly(string name)
        {
            if (Monitor.TryEnter(_lock, 1000))
            {
                try
                {
                    foreach (Ally ally in _allyList)
                    {
                        if (ally.Name == name)
                        {
                            _allyList.Remove(ally);
                            _allyListName.Remove(ally.Name);
                            break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        internal List<Ally> ReturnAllyList()
        {
            lock (_lock)
            {
                return new List<Ally>(_allyList);
            }
        }

        internal List<Enemy> ReturnEnemyList()
        {
            lock (_lock)
            {
                return new List<Enemy>(_enemyList);
            }
        }

        internal void ClearEnemyLists(string name)
        {
            if (ushort.TryParse(name, out ushort spriteId))
            {
                List<Enemy> enemiesToRemove = new List<Enemy>();

                foreach (Enemy enemy in _enemyList)
                {
                    if (enemy.SpriteID == spriteId)
                    {
                        enemiesToRemove.Add(enemy);
                    }
                }

                if (enemiesToRemove.Any())
                {
                    if (Monitor.TryEnter(_lock, 1000))
                    {
                        try
                        {
                            foreach (Enemy enemy in enemiesToRemove)
                            {
                                _enemyList.Remove(enemy);
                                _enemyListID.Remove(enemy.SpriteID);
                            }
                        }
                        finally
                        {
                            Monitor.Exit(_lock);
                        }
                    }
                }
            }
        }

        internal bool IsStrangerNearby()
        {
            return _client.GetNearbyPlayerList().Any(player => IsNotInFriendList(player));
        }

        private bool IsNotInFriendList(Player player)
        {
            if (_client.ClientTab != null)
            {
                return !_client.ClientTab.friendList.Items.OfType<string>().Any(friend => string.Equals(friend, player.Name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return true;//Adam
            }
        }

        private void Loot()
        {
            bool isPickupGoldChecked = Client.ClientTab.pickupGoldCbox.Checked;
            bool isPickupItemsChecked = Client.ClientTab.pickupItemsCbox.Checked;
            bool isDropTrashChecked = Client.ClientTab.dropTrashCbox.Checked;

            if (!isPickupGoldChecked && !isPickupItemsChecked && !isDropTrashChecked)
            {
                return;
            }

            try
            {
                if (!IsStrangerNearby())
                {
                    var lootArea = new Structs.Rectangle(new Point(Client._serverLocation.X - 2, Client._serverLocation.Y - 2), new Point(5, 5));
                    List<Objects.Object> nearbyObjects = Client.GetNearbyObjects(4);

                    if (nearbyObjects.Count > 0)
                    {
                        ProcessLoot(nearbyObjects, lootArea);
                    }

                    HandleTrashItems();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Loot: {ex.Message}");
            }
        }

        private void ProcessLoot(List<Objects.Object> nearbyObjects, Rectangle lootArea)
        {
            bool isPickupGoldChecked = Client.ClientTab.pickupGoldCbox.Checked;
            bool isPickupItemsChecked = Client.ClientTab.pickupItemsCbox.Checked;

            if (isPickupGoldChecked)
            {
                foreach (var obj in nearbyObjects.Where(obj => IsGold(obj, lootArea)))
                {
                    Client.Pickup(0, obj.Location);

                }
            }

            if (isPickupItemsChecked)
            {
                foreach (var obj in nearbyObjects.Where(obj => IsLootableItem(obj, lootArea)))
                {
                    Client.Pickup(0, obj.Location);
                }
            }
        }

        private bool IsGold(Objects.Object obj, Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.SpriteID == 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private bool IsLootableItem(Objects.Object obj, Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.SpriteID != 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private void HandleTrashItems()
        {
            if (Client.ClientTab.dropTrashCbox.Checked)
            {
                if (_dropCounter >= 15)
                {
                    foreach (Item item in Client.Inventory.ToList())
                    {
                        if (Client.ClientTab.trashToDrop.Contains(item.Name, StringComparer.CurrentCultureIgnoreCase))
                        {
                            Client.Drop(item.Slot, Client._serverLocation, item.Quantity);
                        }
                    }
                    _dropCounter = 0;
                }
                else
                {
                    _dropCounter++;
                }
            }
        }

        private sealed class Delegates
        {

            static internal bool HasPlayerExistedForOver250ms(Player player)
            {
                return DateTime.UtcNow.Subtract(player.Creation).TotalMilliseconds > 250.0;
            }
            static internal bool PlayerIsSkulled(Player player)
            {
                return player._isSkulled;
            }

            static internal bool HasPlayerExistedForOver2s(Player player)
            {
                return DateTime.UtcNow.Subtract(player.Creation).TotalSeconds > 2.0;
            }

            static internal bool IsPoisoned(Player player)
            {
                return player.IsPoisoned;
            }

            static internal bool isGreenMantis(Creature creature)
            {
                return creature.SpriteID == 87;
            }

            static internal bool NotDioned(Player player)
            {
                return !player.IsDioned;
            }

            static internal int NextRandom(Creature creature)
            {
                return Utility.Random();
            }

            static internal bool CreaturesExisitingLessThan2s(Creature creature)
            {
                return DateTime.UtcNow.Subtract(creature.Creation).TotalSeconds < 2.0;
            }

            static internal Creature IsCreature(Creature creature)
            {
                return creature;
            }

            static internal bool IsNotFassedOrNotCursed(Creature creature)
            {
                return !creature.IsFassed || !creature.IsCursed;
            }

            static internal bool IsNotFassed(Creature creature)
            {
                return !creature.IsFassed;
            }


            static internal bool IsFassed(Creature creature)
            {
                return creature.IsFassed;
            }


            static internal bool IsNotCursed(Creature creature)
            {
                return !creature.IsCursed;
            }


            static internal bool IsCursed(Creature creature)
            {
                return creature.IsCursed;
            }


            static internal bool IsDioned(Creature creature)
            {
                return creature.IsDioned;
            }


            static internal bool CanCastPND(Creature creature)
            {
                return creature._canPND;
            }


            static internal bool IsNotFrozen(Creature creature)
            {
                return !creature.IsFrozen;
            }


            static internal bool IsNotPoisoned(Creature creature)
            {
                return !creature.IsPoisoned;
            }


            static internal bool IsNotDioned(Creature creature)
            {
                return !creature.IsDioned;
            }


            static internal bool IsAsleep(Player player)
            {
                return player.IsAsleep;
            }

            static internal int KVPMatch(KeyValuePair<Creature, int> kvp)
            {
                return kvp.Value;
            }


            static internal DateTime KVPCreation(KeyValuePair<Creature, int> kvp)
            {
                return kvp.Key.Creation;
            }

            static internal bool CastRegeneration(Spell spell)
            {
                return spell.Name != "Increased Regeneration" && spell.Name.Contains("Regeneration");
            }


            static internal bool CreaturesExistingOver250ms(Creature creature)
            {
                return DateTime.UtcNow.Subtract(creature.Creation).TotalMilliseconds > 250.0;
            }


            static internal DateTime CreatureCreationDate(Creature creature)
            {
                return creature.Creation;
            }


            static internal bool CastArrowShot(string spellName)
            {
                return spellName.Contains("Arrow Shot");
            }


            static internal bool CreatureNotHiddenOrPoisoned(Creature creature)
            {
                Player player = creature as Player;
                if (player != null)
                {
                    if (player._isHidden)
                    {
                        return false;
                    }
                }
                return !creature.IsPoisoned;
            }

            static internal bool NeedsRegeneration(Creature creature)
            {
                return !creature.SpellAnimationHistory.ContainsKey(187) || DateTime.UtcNow.Subtract(creature.SpellAnimationHistory[187]).TotalSeconds > 4.0;
            }


            static internal bool NeedsCounterAttack(Creature creature)
            {
                return creature is Player && (!creature.SpellAnimationHistory.ContainsKey(184) || DateTime.UtcNow.Subtract(creature.SpellAnimationHistory[184]).TotalSeconds > 20.0);
            }



            static internal bool SpellNotNull(Spell spell)
            {
                return spell != null;
            }

            public static readonly Bot.Delegates class145 = new Bot.Delegates();


        }

    }


}
