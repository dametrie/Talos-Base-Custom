using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        internal List<Ally> _allyList = new List<Ally>();
        internal List<Enemy> _enemyList = new List<Enemy>();
        internal List<Player> _playersExistingOver250ms = new List<Player>();
        internal List<Player> _playersNeedingRed = new List<Player>();
        internal List<Player> nearbyAllies = new List<Player>();
        internal List<Creature> nearbyValidCreatures = new List<Creature>();

        internal HashSet<string> _allyListName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        internal HashSet<ushort> _enemyListID = new HashSet<ushort>();



        internal System.Windows.Forms.Label currentAction;


        internal AllyPage AllyPage { get; set; }
        internal EnemyPage EnemyPage { get; set; }


        internal Bot(Client client, Server server) : base(client, server)
        {
            _client = client;
            _server = server;
            base.AddTask(new TaskDelegate(BotLoop));
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
                    if (this.currentAction == null)
                    {
                        this.currentAction = Client.ClientTab.currentAction;
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
                    CheckAndHandleSpells();

                    //Console.WriteLine("Checking for autoRed conditions");
                    if (autoRedConditionsMet())
                    {
                        HandleAutoRed();
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
                return Client.GetNearbyPlayers().Any(new Func<Player, bool>(this.RangerListContains));
            }
            return this.IsStrangerNearby();
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
            nearbyAllies = Client.GetNearbyAllies();
            nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            var nearbyPlayers = Client.GetNearbyPlayers();
            _playersExistingOver250ms = nearbyPlayers?.Where(Bot.Delegates.HasPlayerExistedForOver250ms).ToList() ?? new List<Player>();
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
            if (Client.ClientTab.optionsSkullSurrbox.Checked && DateTime.UtcNow.Subtract(this._skullTime).TotalSeconds > 4.0 && Client.IsLocationSurrounded(Client._serverLocation))
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
            if (_playersExistingOver250ms.Any(Bot.Delegates.PlayerIsSkulled))
            {
                _playersExistingOver250ms.RemoveAll(Bot.Delegates.PlayerIsSkulled);
                foreach (var player in Client.GetNearbyPlayers().Where(IsSkulledFriendOrGroupMember))
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
                foreach (var creature in nearbyValidCreatures)
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


            Client currentClient = base.Client;
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
                Console.WriteLine("Sleeping for 10ms");
                Thread.Sleep(330);
            }
        }

        private bool CastOffensiveSpells()
        {
            this.nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            if (this.nearbyValidCreatures.Count > 0)
            {
                this.nearbyValidCreatures = this.nearbyValidCreatures.OrderBy(new Func<Creature, int>(Bot.Delegates.NextRandom)).ToList<Creature>();
            }
            if (this.IsStrangerNearby() && this.nearbyValidCreatures.Count > 0)
            {
                this.nearbyValidCreatures.RemoveAll(new Predicate<Creature>(Bot.Delegates.CreaturesExisitingLessThan2s));
            }
            if (this.IsStrangerNearby())
            {
                List<Creature> list = new List<Creature>();
                foreach (Creature creature in this.nearbyValidCreatures)
                {
                    foreach (Creature creature2 in this.nearbyValidCreatures)
                    {
                        if (creature != creature2 && Point.Equals(creature.Location, creature2.Location) && !list.Contains(creature))
                        {
                            list.Add(creature);
                        }
                    }
                }
                foreach (Creature creature in list)
                {
                    this.nearbyValidCreatures.Remove(creature);
                }
            }
            if (this.EnemyPage != null)
            {
                if (this.EnemyPage.ignoreCbox.Checked)
                {
                    this.nearbyValidCreatures.RemoveAll(new Predicate<Creature>(this.CreaturesToIgnore));
                }
                if (this.EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in this.nearbyValidCreatures)
                    {
                        if (this.EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!this.EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (this.EnemyPage.spellAllRbtn.Checked)
                    {
                        this.creature = null;
                        if (priority.Count > 0 && this.DecideAndExecuteEngagementStrategy(this.EnemyPage, priority))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                        if (nonPriority.Count > 0 && this.DecideAndExecuteEngagementStrategy(this.EnemyPage, nonPriority))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (priority.Count > 0 && this.SpellOneAtATime(this.EnemyPage, priority))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                        if (!priority.Contains(this.creature) && nonPriority.Count > 0 && this.SpellOneAtATime(this.EnemyPage, nonPriority))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                }
                //Spell all at once
                else if (this.EnemyPage.spellAllRbtn.Checked)
                {
                    this.creature = null;
                    if (this.nearbyValidCreatures.Count > 0 && this.DecideAndExecuteEngagementStrategy(this.EnemyPage, this.nearbyValidCreatures))
                    {
                        this.bool_13 = true;
                        return true;
                    }
                }
                //Spell one at a time
                else if (this.nearbyValidCreatures.Count > 0 && this.SpellOneAtATime(this.EnemyPage, this.nearbyValidCreatures))
                {
                    this.bool_13 = true;
                    return true;
                }
            }
            else
            {
                List<Creature> creatureList = new List<Creature>();
                foreach (Enemy enemy in this.ReturnEnemyList())
                {
                    creatureList.Clear();
                    foreach (Creature creature in Client.GetCreaturesInRange(11, new ushort[] { enemy.SpriteID }))
                    {
                        if (creature.SpriteID.ToString() == enemy.SpriteID.ToString())
                        {
                            creatureList.Add(creature);
                        }
                    }
                    creatureList.OrderBy(new Func<Creature, Creature>(Bot.Delegates.IsCreature));
                    if (creatureList.Count > 0)
                    {
                        if (enemy.EnemyPage.spellAllRbtn.Checked)
                        {
                            this.creature = null;
                            if (this.DecideAndExecuteEngagementStrategy(enemy.EnemyPage, creatureList))
                            {
                                this.bool_13 = true;
                                return true;
                            }
                        }
                        else if (this.SpellOneAtATime(enemy.EnemyPage, creatureList))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                }
            }
            if (this.EnemyPage != null)
            {
                if (this.EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in this.nearbyValidCreatures)
                    {
                        if (this.EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!this.EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (this.CastAttackSpell(this.EnemyPage, priority))
                    {
                        return true;
                    }
                    if (this.CastAttackSpell(this.EnemyPage, nonPriority))
                    {
                        return true;
                    }
                }
                else if (this.CastAttackSpell(this.EnemyPage, this.nearbyValidCreatures))
                {
                    return true;
                }
            }
            else
            {
                List<Creature> list7 = new List<Creature>();
                foreach (Enemy enemy in this.ReturnEnemyList())
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
                    list7.OrderBy(new Func<Creature, Creature>(Bot.Delegates.IsCreature));
                    if (this.CastAttackSpell(enemy.EnemyPage, list7))
                    {
                        return true;
                    }
                }
            }
            this.bool_13 = false;
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
                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(new Func<Creature, int>(this.DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (this.creature == null)
                {
                    return false;
                }
                creatureList = new List<Creature>
                {
                    this.creature
                };
            }
            if (this.EnemyPage != null && this.EnemyPage.mpndDioned.Checked)
            {
                if (!enemyPage.attackCboxOne.Checked && !enemyPage.attackCboxTwo.Checked)
                {
                    if (!this.nearbyValidCreatures.Any(new Func<Creature, bool>(Bot.Delegates.IsDioned)))
                    {
                        goto IL_1A5;
                    }
                }
                else if (!this.nearbyValidCreatures.All(new Func<Creature, bool>(Bot.Delegates.CanCastPND)))
                {
                    goto IL_1A5;
                }
                Creature creatureTarget = this.nearbyValidCreatures.OrderBy(new Func<Creature, int>(this.DistanceFromClientLocation)).FirstOrDefault(new Func<Creature, bool>(Bot.Delegates.CanCastPND));
                if (creatureTarget != null)
                {
                    return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, creatureTarget, _autoStaffSwitch, false);
                }
            }
        IL_1A5:
            if (!this._isSilenced)
            {
                if (enemyPage.attackCboxTwo.Checked)
                {
                    Creature target = this.SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxTwo.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxTwo.Text;
                        uint fnvHash = Utility.CalculateFNV(spellName);

                        switch (fnvHash)
                        {
                            case 1007116742U: // Supernova Shot
                                return Client.UseSpell("Supernova Shot", target, this._autoStaffSwitch, false);

                            case 1285349432U: // Shock Arrow
                                return Client.UseSpell("Shock Arrow", target, this._autoStaffSwitch, false) && 
                                       Client.UseSpell("Shock Arrow", target, this._autoStaffSwitch, false);

                            case 1792178996U: // Volley
                                return Client.UseSpell("Volley", target, this._autoStaffSwitch, true);

                            case 2591503996U: // MSPG
                                if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
                                {
                                    this._manaLessThanEightyPct = true;
                                    Client.UseSpell("fas spiorad", null, this._autoStaffSwitch, false);
                                    return true;
                                }
                                return Client.UseSpell("mor strioch pian gar", Client.Player, this._autoStaffSwitch, true);


                            case 3122026787U: // Unholy Explosion
                                return Client.UseSpell("Unholy Explosion", target, this._autoStaffSwitch, false);

                            case 3210331623U: // Cursed Tune
                                return this.TryCastAnyRank("Cursed Tune", target, this._autoStaffSwitch, false);

                            case 3848328981U: // M/DSG
                                return Client.UseSpell("mor deo searg gar", Client.Player, this._autoStaffSwitch, false) || 
                                       Client.UseSpell("deo searg gar", Client.Player, this._autoStaffSwitch, false);        
                    }
                }
                }
                if (enemyPage.attackCboxOne.Checked)
                {
                    Creature target = this.SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxOne.Text);
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
            if (this._isSilenced && enemyPage.mpndSilenced.Checked)
            {
                Creature mpndTarget = this.SelectAttackTarget(enemyPage, creatureList, "A/M/PND");
                return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, mpndTarget, _autoStaffSwitch, false);
            }
            if (!this._isSilenced || !enemyPage.mspgSilenced.Checked || this.SelectAttackTarget(enemyPage, creatureList, "MSPG") == null)
            {
                return result;
            }
            if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
            {
                this._manaLessThanEightyPct = true;
                return Client.UseSpell("fas spiorad", null, this._autoStaffSwitch, false); ;
            }
            Client.UseSpell("mor strioch pian gar", Client.Player, this._autoStaffSwitch, true); //Adam check this
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
            foreach (var creature in creatureList.OrderBy(c => this.DistanceFromClientLocation(c)))
            {

                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(new Func<Creature, int>(this.DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (creature == null)
                {
                    continue; 
                }
                this.creature = creature;

                if (enemyPage.pramhFirstRbtn.Checked && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, this._autoStaffSwitch, false);
                    return true;
                }
                if (enemyPage.spellFirstRbtn.Checked && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (this.CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.pramhFirstRbtn.Checked && (!enemyPage.spellsControlCbox.Checked || creature.IsAsleep) && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (this.CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.spellFirstRbtn.Checked && (!enemyPage.spellsFasCbox.Checked || creature.IsFassed) && (!enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, this._autoStaffSwitch, false);
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
                this.bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, this._autoStaffSwitch, false);
                return true;
            }
            if (enemyPage.curseFirstRbtn.Checked && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                this.bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, this._autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.fasFirstRbtn.Checked || !enemyPage.spellsFasCbox.Checked || creature.IsFassed) && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                this.bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, this._autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.curseFirstRbtn.Checked || !enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                this.bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, this._autoStaffSwitch, false);
                return true;
            }
            return false;
        }


        private bool DecideAndExecuteEngagementStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            if (enemyPage.pramhFirstRbtn.Checked)
            {
                if (enemyPage.spellsControlCbox.Checked)
                    if (this.ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (this.ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            else if (enemyPage.spellFirstRbtn.Checked)
            {
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (this.ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsControlCbox.Checked)
                    if (this.ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            return false;
        }

        private bool ExecuteDebuffStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            List<Creature> eligibleCreatures = creatureList.Where(Bot.Delegates.IsNotFassedOrNotCursed).ToList();
            if (eligibleCreatures != null && eligibleCreatures.Any() && (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked))
            {
                if (enemyPage.fasFirstRbtn.Checked)
                {
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (this.CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (this.CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                }
                else if (enemyPage.curseFirstRbtn.Checked)
                {
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (this.CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (this.CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            this.bool_13 = true;
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        private bool CastCurseIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (Bot._lock)
            {
                Console.WriteLine($"[CastCurseIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not cursed
                var eligibleCreatures = creatures.Where(Bot.Delegates.IsNotCursed);

                Console.WriteLine($"[CastCurseIfApplicable] Eligible creatures (not cursed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting curses is enabled, cast the curse spell
                if (targetCreature != null && enemyPage.spellsCurseCbox.Checked)
                {
                    Console.WriteLine($"[CastCurseIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastCursed: {targetCreature.LastCursed}, IsCursed: {targetCreature.IsCursed}");
                    Client.UseSpell(enemyPage.spellsCurseCombox.Text, targetCreature, this._autoStaffSwitch, false);
                    this.bool_13 = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken

            }
        }

        private bool CastFasIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (Bot._lock)
            {
                Console.WriteLine($"[CastFasIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not 'fassed'
                var eligibleCreatures = creatures.Where(Bot.Delegates.IsNotFassed);

                Console.WriteLine($"[CastFasIfApplicable] Eligible creatures (not fassed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting the 'fas' spell is enabled, cast the spell
                if (targetCreature != null && enemyPage.spellsFasCbox.Checked)
                {
                    Console.WriteLine($"[CastFasIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastFassed: {targetCreature.LastFassed}, IsFassed: {targetCreature.IsFassed}");
                    Client.UseSpell(enemyPage.spellsFasCombox.Text, targetCreature, this._autoStaffSwitch, false);
                    this.bool_13 = true; // Indicate that an action was taken
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
                            attackList = attackList.Where(new Func<Creature, bool>(Bot.Delegates.IsNotDioned)).ToList<Creature>();
                        }
                        if (enemyPage.expectedHitsNum.Value > 0m)
                        {
                            attackList = attackList.Where(creature => CalculateHitCounter(creature, enemyPage)).ToList<Creature>();
                        }
                    }
                }
                else
                {
                    attackList = attackList.Where(new Func<Creature, bool>(Bot.Delegates.IsNotPoisoned)).ToList<Creature>();
                }
            }
            else
            {
                attackList = attackList.Where(new Func<Creature, bool>(Bot.Delegates.IsNotFrozen)).ToList<Creature>();
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
                                if (c.Location.DistanceFrom(loc) <= maxDistance || this.IsDiagonallyAdjacent(c.Location.Point, loc.Point, maxDistance))
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
                return dictionary.OrderByDescending(new Func<KeyValuePair<Creature, int>, int>(Bot.Delegates.KVPMatch)).ThenBy(new Func<KeyValuePair<Creature, int>, DateTime>(Bot.Delegates.KVPCreation)).First<KeyValuePair<Creature, int>>().Key;
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
            if (creature.Health == 0 && creature.SpellAnimationHistory.Count != 0 && creature._animation != 33 && DateTime.UtcNow.Subtract(creature._lastUpdate).TotalSeconds <= 1.5)
            {
                return creature._hitCounter < enemyPage.expectedHitsNum.Value;
            }
            if (creature.Health != 0 && creature._hitCounter > enemyPage.expectedHitsNum.Value)
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
                List<Client> nearbyClients = Client.GetNearbyPlayers()
                    .Select(player => Server.FindClientByName(player?.Name))
                    .Where(client => client != null)
                    .ToList();

                if (nearbyClients.Any())
                {
                    Client furthestClient = nearbyClients
                        .OrderByDescending(client => this.CalculateDistanceFromBaseClient(client))
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
            return client._serverLocation.DistanceFrom(base.Client._serverLocation);
        }

        private bool ExecutePramhStrategy(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> creatureList = this.FilterCreaturesByControlStatus(enemyPage, creatures);
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
                Client.UseSpell(enemyPage.spellsControlCombox.Text, creatureList.FirstOrDefault<Creature>(), this._autoStaffSwitch, false);
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
            return this.EnemyPage.ignoreLbox.Items.Contains(creature.SpriteID.ToString());
        }

        private int DistanceFromServerLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._serverLocation);
        }
        private int DistanceFromClientLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._clientLocation);
        }



        private bool CastDefensiveSpells()
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
                        Client.UseSpell("fas spiorad", null, this._autoStaffSwitch, false);
                    }

                    bool hasManaReachedHalf  = Client.CurrentMP >= Client.Stats.MaximumMP / 2U;
                    bool hasManaIncreasedByFivePercent  = Client.CurrentMP - currentMP >= Client.Stats.MaximumMP / 20U;
                    bool isDurationExceeded = DateTime.UtcNow.Subtract(startTime).TotalSeconds > 20.0;

                    if (hasManaReachedHalf  || hasManaIncreasedByFivePercent  || isDurationExceeded)
                    {
                        _needFasSpiorad = false;
                    }

                    Thread.Sleep(10);
                }
            }

            return true;
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
            _playersExistingOver250ms = !IsStrangerNearby() ? _playersExistingOver250ms : _playersExistingOver250ms.Where(Bot.Delegates.HasPlayerExistedForOver2s).ToList();
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
            if (DateTime.UtcNow.Subtract(this._lastCast).TotalSeconds > 1.0)
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
            lock (Bot._lock)
            {
                return _allyListName.Contains(name, StringComparer.CurrentCultureIgnoreCase);
            }
        }

        internal bool IsEnemyAlreadyListed(ushort sprite)
        {

            lock (Bot._lock)
            {
                return _enemyListID.Contains(sprite);
            }
        }

        internal void UpdateAllyList(Ally ally)
        {
            lock (Bot._lock)
            {
                this._allyList.Add(ally);
                this._allyListName.Add(ally.Name);
            }
        }

        internal void UpdateEnemyList(Enemy enemy)
        {
            lock (Bot._lock)
            {
                this._enemyList.Add(enemy);
                this._enemyListID.Add(enemy.SpriteID);
            }
        }

        internal void RemoveAlly(string name)
        {
            if (Monitor.TryEnter(Bot._lock, 1000))
            {
                try
                {
                    foreach (Ally ally in this._allyList)
                    {
                        if (ally.Name == name)
                        {
                            this._allyList.Remove(ally);
                            this._allyListName.Remove(ally.Name);
                            break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(Bot._lock);
                }
            }
        }

        internal List<Ally> ReturnAllyList()
        {
            lock (Bot._lock)
            {
                return new List<Ally>(_allyList);
            }
        }

        internal List<Enemy> ReturnEnemyList()
        {
            lock (Bot._lock)
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
                    if (Monitor.TryEnter(Bot._lock, 1000))
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
                            Monitor.Exit(Bot._lock);
                        }
                    }
                }
            }
        }

        internal bool IsStrangerNearby()
        {
            return _client.GetNearbyPlayers().Any(new Func<Player, bool>(this.ReturnNotInFriendList));
        }

        private bool ReturnNotInFriendList(Player player)
        {
            return !_client.ClientTab.friendList.Items.OfType<string>().Contains(player.Name, StringComparer.OrdinalIgnoreCase);
        }

        private void Loot()
        {
            //Console.WriteLine("Loot method running");
            if (!Client.ClientTab.pickupGoldCbox.Checked && !Client.ClientTab.pickupItemsCbox.Checked && !Client.ClientTab.dropTrashCbox.Checked)
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
            if (Client.ClientTab.pickupGoldCbox.Checked)
            {
                foreach (var obj in nearbyObjects.Where(obj => IsGold(obj, lootArea)))
                {
                    Client.Pickup(0, obj.Location);
                }
            }

            if (Client.ClientTab.pickupItemsCbox.Checked)
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

        private sealed class AllyProxy
        {

            internal bool IsAllyInNeed(Player player)
            {
                return player != this.bot._client.Player && this.bot.IsAllyAlreadyListed(player.Name) && (player.Health < 20 || player.NeedsHeal);
            }


            internal bool IsAllyAffectedByPramhOrAsleep(Player player)
            {
                if (!player.IsAsleep)
                {
                    Client client = this.bot._server.FindClientByName(player.Name);
                    return client != null && client.HasEffect(EffectsBar.Pramh);
                }
                return true;
            }


            internal bool IsAllyLowOnMP(Client client)
            {
                return client != null && !string.IsNullOrEmpty(client.Name) && !client.Name.Contains("[") && this.bot._client.AllyListHashSet.Any(name => name.ToUpper() == client.Name.ToUpper()) && (ulong)client.ManaPct < (ulong)((long)this.mpThreshold);
            }


            public Bot bot;

            public int mpThreshold;

            public Func<Player, bool> func_0;
        }


        private sealed class AllyMatcher
        {

            internal bool IsMatchedAlly(Ally ally)
            {
                return ally.Name == this.player.Name;
            }


            public Player player;


            public AllyProxy allyProxy;
        }


        private sealed class PlayerExcluder
        {

            internal bool ShouldExcludePlayer(Player player)
            {
                return !this.allyMatcher.allyProxy.bot._client.ClientTab.friendList.Items.OfType<string>().Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player != this.player && (Point.Equals(player.Location, this.player.Location) | this.player._isHidden);
            }


            public Player player;


            public AllyMatcher allyMatcher;
        }


        private sealed class AllyNameMatcher
        {

            internal bool IsNameMatch(string name)
            {
                return name.ToUpper() == this.client.Name.ToUpper();
            }


            public Client client;
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
