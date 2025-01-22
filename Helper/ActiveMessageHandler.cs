using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using Talos.Base;
using Talos.Enumerations;
using Talos.Objects;
using Talos.Properties;
using Talos.Structs;

namespace Talos.Helper
{
    internal class ActiveMessageHandler
    {
        private static readonly ActiveMessageHandler _instance = new ActiveMessageHandler();
        private Dictionary<string, Action<Client, string>> stringMessageHandlers;
        private Dictionary<Regex, Action<Client, Match>> regexMessageHandlers;
        private static readonly object _lock = new object();

        // Public static property to provide access to the single instance
        public static ActiveMessageHandler Instance => _instance;

        // Private constructor to prevent external instantiation
        private ActiveMessageHandler()
        {
            stringMessageHandlers = new Dictionary<string, Action<Client, string>>
            {
                { "You just sent a broadcast, you must wait a few more minutes.", HandleArenaBroadcastMessage },
                { "Your AP is too high", HandleLastKillMessage },
                { "You have reached level 99, the maximum for the free trial.", HandleLastKillMessage },
                { "You can't have more.", HandleInventoryMessage },
                { "You lost 50 vitality", HandleRecentlyDiedMessage },
                { "Your items are ripped from your body.", HandleRecentlyDiedMessage },
                { "You are not a member of the Training Grounds", HandleTrainingGroundsMessage },
                { "You feel better.", HandlePoisonMessage },
                { "Poison", HandlePoisonMessage },
                { "Your armor is strengthened.", HandleArmachdMessage },
                { "Your armor spell wore off.", HandleArmachdMessage },
                { "The magic has been deflected.", HandleResistMessage },
                { "You can't cast that spell right now.", HandleCantCastMessage },
                { "You can't cast a spell.", HandleCantCastMessage },
                { "You can't use skills here.", HandleCantCastMessage },
                { "That doesn't work here.", HandleCantCastMessage },
                { "Your Will is too weak.", HandleNoManaMessage },
                { "Cannot find group member.", HandleGroupMessage },
                { "Already a member of another group.", HandleGroupMessage },
                { "Group disbanded.", HandleGroupMessage },
                { "You can't perceive the invisible.", HandleCatsMessage },
                { "You can perceive the invisible.", HandleCatsMessage },
                { "In sleep", HandlePramhMessage },
                { "beag pramh", HandlePramhMessage },
                { "pramh", HandlePramhMessage },
                { "Awake", HandlePramhMessage },
                { "Pause", HandlePauseMessage },
                { "Pause end", HandlePauseMessage },
                { "Normal power.", HandleFasDeireasMessage },
                { "You feel more powerful.", HandleFasDeireasMessage },
                { "End of blessing.", HandleBeannaichMessage },
                { "You have been blessed.", HandleBeannaichMessage },
                { "Inner warmth of regeneration dissipates.", HandleInnerFireMessage },
                { "Inner warmth begins to regenerate you.", HandleInnerFireMessage },
                { "Purify", HandlePurifyMessage },
                { "Purify end", HandlePurifyMessage },
                { "You won't die from any spell.", HandlePerfectDefenseMessage },
                { "You become normal.", HandlePerfectDefenseMessage },
                { "You cast Spell/Skill Level Bonus.", HandleBonusMessage },
                { "Something went wrong.", HandleWrongMessage },
                { "beag cradh end.", HandleCurseEndMessage },
                { "cradh end.", HandleCurseEndMessage },
                { "mor cradh end.", HandleCurseEndMessage },
                { "ard cradh end.", HandleCurseEndMessage },
                { "Dark Seal end.", HandleCurseEndMessage },
                { "Darker Seal end.", HandleCurseEndMessage },
                { "Demon Seal end.", HandleCurseEndMessage },
                { "Demise end.", HandleCurseEndMessage },
                { "beag cradh", HandleCurseBeginMessage },
                { "cradh", HandleCurseBeginMessage },
                { "mor cradh", HandleCurseBeginMessage },
                { "ard cradh", HandleCurseBeginMessage },
                { "Dark Seal", HandleCurseBeginMessage },
                { "Darker Seal", HandleCurseBeginMessage },
                { "Demise", HandleCurseBeginMessage },
                { "Demon Seal", HandleCurseBeginMessage },
                { "double attribute", HandleFasMessage },
                { "normal nature.", HandleFasMessage },
                { "Stunned", HandleBeagSuainMessage },
                { "You can move again.", HandleBeagSuainMessage },
                { "You have found sanctuary.", HandleAiteMessage },
                { "You feel vulnerable again.", HandleAiteMessage },
                { "You cast Disenchanter.", HandleDisenchanterMessage },
                { "You canot see anything.", HandleDallMessage },
                { "You can see again.", HandleDallMessage },
                { "Invisible.", HandleHideMessage },
                { "You are no longer invisible.", HandleHideMessage },
                { "Failed.", HandleFailedMessage },
                { "You cannot attack.", HandleCantAttackMessage },
                { "asgall faileas", HandleAsgallMessage },
                { "asgall faileas end.", HandleAsgallMessage },
                { "You feel quicker.", HandleMistMessage },
                { "Your reflexes return to normal.", HandleMistMessage },
                { "Reflect.", HandleDeireasFaileasMessage },
                { "Reflect end.", HandleDeireasFaileasMessage },
                { "Halt", HandleHaltMessage },
                { "Halt end.", HandleHaltMessage },
                { "You were distracted", HandleDistractedMessage },
                { "Your body thaws.", HandleSuainMessage },
                { "Your body is freezing.", HandleSuainMessage },
                { "You are in hibernation.", HandleSuainMessage },
                { "You already cast that spell.", HandleAlreadyCastMessage },
                { "Harden body spell", HandleDionMessage },
                { "Your skin turns back to flesh.", HandleDionMessage },
                { "You are not well.", HandleDragonMessage },
                { "No longer dragon.", HandleDragonMessage },
                { "It does not touch the spirit world.", HandleSpiritWorldMessage },
                { "You are stuck.", HandleStuckMessage },
            };

            regexMessageHandlers = new Dictionary<Regex, Action<Client, Match>>
            {
                { new Regex(@"^([a-zA-Z]+) is (?:joining|leaving) this group.$"), HandleJoinLeaveGroupMessage },
                { new Regex(@"^You cast (.*?)\.$"), HandleSpellCastMessage },
                { new Regex(@"The durability of ([a-zA-Z]+) is now (\d+)%$"), HandleItemDamageMessage },
                { new Regex(@"(\d+) experience!"), HandleExperienceMessage },//Adam fix this
                { new Regex("AP went up"), HandleExperienceMessage }, //Adam fix this
                { new Regex(@"^\(\( 4 Temuairan days = 12 (Terran|real-life) hours \)\)$"), HandleLaborMessage },//Adam check
                { new Regex(@"You do not have time for these 4 Temuairan days"), HandleLaborMessage },//Adam check
                { new Regex("[a-zA-Z] works for you for 1 day"), HandleLaborMessage },//Adam check
                { new Regex(@"You work for \w+, although the Aisling didn't need much done"), HandleLaborMessage },//Adam check
                { new Regex(@"n:Necklace:([a-zA-Z \'0-9]+)\t Armor class"), HandleNecklaceMessage },
                { new Regex("([a-zA-Z0-9 ]+), You can't have more than [0-9]+"), HandleInventoryFullMessage },
                { new Regex("^.*? ao sith .*?"), HandleAoSithMessage },
                { new Regex("(You have no bow|You do not have a bow equipped)"), HandleBowMessage },
                { new Regex(@"^Another curse afflicts thee\. \[(.*)\]$"), HandleCurseMessage },
                { new Regex(@"^\w+ attacks you with PreventAffliction spell\.$"), HandlePreventAfflictionMessage },
                { new Regex("PreventAffliction end."), HandlePreventAfflictionMessage },
                { new Regex("botcheck (?:attacks|casts)", RegexOptions.IgnoreCase), HandleBotChecks },
                { new Regex("error (?:attacks|casts)", RegexOptions.IgnoreCase), HandleBotChecks },
            };

        }



        internal bool Handle(Client client, string message)
        {

            if (ShouldRemoveSpam(message))
            {
                return false;
            }

            if (stringMessageHandlers.TryGetValue(message, out var stringHandler))
            {
                stringHandler(client, message);
                return true;
            }

            foreach (var kvp in regexMessageHandlers)
            {
                if (kvp.Key.IsMatch(message))
                {
                    kvp.Value(client, kvp.Key.Match(message));
                    return true;
                }
            }

            return true; // allow any message we aren't handling to come through
        }
        private static void HandleSpellCastMessage(Client client, Match match)
        {
            lock (_lock)
            {
                Creature creature = client.SpellHistory.Count > 0 ? client.SpellHistory[0].Creature : null;
                if (creature != null)
                {
                    //Console.WriteLine($"[HandleSpellCastMessage] Creature ID: {creature?.ID}. _creatureToSpellList[0] Creature ID: {client._creatureToSpellList[0].Creature.ID}");
                }

                string spellName = match.Groups[1].Value;

                //adam add spells - pramh, ards?
                //You cast Master Karura Form
                switch (spellName)
                {
                    case "beag naomh aite":
                    case "naomh aite":
                    case "mor naomh aite":
                    case "ard naomh aite":
                        if (creature != null)
                        {
                            var stateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.AiteDuration, Spell.GetSpellDuration(spellName) },
                                { CreatureState.LastAited, DateTime.UtcNow }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, stateUpdates);
                        }
                        break;

                    case "beag cradh":
                    case "cradh":
                    case "mor cradh":
                    case "ard cradh":
                    case "Dark Seal":
                    case "Darker Seal":
                    case "Demise":
                    case "Demon Seal":
                        if (creature != null)
                        {
                            //creature.Curse = spellName;
                            //creature.CurseDuration = Spell.GetSpellDuration(spellName);
                            //creature.LastCursed = DateTime.UtcNow;
                            //if (creature.ID != client.Player.ID)
                            //    client.UpdateCurseTargets(client, creature.ID, spellName);

                            double curseDuration = Spell.GetSpellDuration(spellName);

                            var curseStateUpdates = new Dictionary<CreatureState, object> //Adam New
                            {
                                { CreatureState.IsCursed, true },
                                { CreatureState.LastCursed, DateTime.UtcNow },
                                { CreatureState.CurseName, spellName },
                                { CreatureState.CurseDuration, curseDuration } // Duration in seconds
                            };
                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, curseStateUpdates);

                            Console.WriteLine($"[HandleSpellCastMessage] {spellName} cast on Creature ID: {creature?.ID}, Creature Name: {creature?.Name}, Hash: {creature.GetHashCode()}, LastCursed updated to {creature?.GetState<DateTime>(CreatureState.LastCursed)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "beag fas nadur":
                    case "fas nadur":
                    case "mor fas nadur":
                    case "ard fas nadur":
                        if (creature != null)
                        {
                            //creature.FasDuration = Spell.GetSpellDuration(spellName);
                            //creature.LastFassed = DateTime.UtcNow;
                            //if (creature.ID != client.Player.ID)
                            //    client.UpdateFasTargets(client, creature.ID, creature.FasDuration);

                            double fasDuration = Spell.GetSpellDuration(spellName);

                            var fasStateUpdates = new Dictionary<CreatureState, object> //Adam new
                            {
                                { CreatureState.IsFassed, true },
                                { CreatureState.LastFassed, DateTime.UtcNow },
                                { CreatureState.FasName, spellName },
                                { CreatureState.FasDuration, fasDuration } // Duration in seconds
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, fasStateUpdates);

                            //Console.WriteLine($"[HandleSpellCastMessage] {spellName} cast on Creature ID: {creature?.ID}. LastFassed updated to {creature?.GetState<DateTime>(CreatureState.LastFassed)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "dion":
                    case "Draco Stance":
                    case "Stone Skin":
                    case "mor dion":
                    case "Iron Skin":
                    case "Wings of Protection":
                    case "dionLR":
                        if (creature != null)
                        {
                            double dionDuration = Spell.GetSpellDuration(spellName);

                            var dionStateUpdates = new Dictionary<CreatureState, object> //Adam new
                            {
                                { CreatureState.IsDioned, true },
                                { CreatureState.LastDioned, DateTime.UtcNow },
                                { CreatureState.DionName, spellName },
                                { CreatureState.DionDuration, dionDuration } // Duration in seconds
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, dionStateUpdates);
                        }
                        break;

                    case "fas spiorad":
                        client.Bot._needFasSpiorad = false;
                        client.Bot._manaLessThanEightyPct = false;
                        client.Player.NeedsHeal = true;
                        break;

                    case "mor strioch pian gar":
                        client.Bot._needFasSpiorad = true;
                        break;

                    case "io dia armachd comlhaLR":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.Armachd] = DateTime.UtcNow;

                            var armachdStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastArmachd, DateTime.UtcNow },
                                { CreatureState.ArmachdDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, armachdStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Armachd' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Armachd Duration: {creature.GetState<double>(CreatureState.ArmachdDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "io ao dallLR":
                    case "io ao dall comlhaLR":
                        //update ao dall spell animation?
                        //remove dall from players and clients?
                        break;

                    case "Light SealLR":
                        client.Bot._lastGrimeScentCast = DateTime.UtcNow;
                        break;

                    case "Gem Polishing":
                        client.CastedSpell = client.Spellbook["Gem Polishing"];
                        break;

                    case "armachd":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.Armachd] = DateTime.UtcNow;

                            var armachdStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastArmachd, DateTime.UtcNow },
                                { CreatureState.ArmachdDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, armachdStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Armachd' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Armachd Duration: {creature.GetState<double>(CreatureState.ArmachdDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "suain":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.Suain] = DateTime.UtcNow;

                            var suainStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastSuained, DateTime.UtcNow },
                                { CreatureState.SuainDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, suainStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Suain' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Suain Duration: {creature.GetState<double>(CreatureState.SuainDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Master Karurua Form"://Adam add others //need to figure out how to clear it because there is no orange message when it drops
                        break;

                    case "beag pramh":
                    case "pramh":
                    case "Mesmerize":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.Pramh] = DateTime.UtcNow;

                            var pramhStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastPramhed, DateTime.UtcNow },
                                { CreatureState.PramhDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, pramhStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Sleep' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Sleep Duration: {creature.GetState<double>(CreatureState.PramhDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Frost Arrow 1":
                    case "Frost Arrow 2":
                    case "Frost Arrow 3":
                    case "Frost Arrow 4":
                    case "Frost Arrow 5":
                    case "Frost Arrow 6":
                    case "Frost Arrow 7":
                    case "Frost Arrow 8":
                    case "Frost Arrow 9":
                    case "Frost Arrow 10":
                    case "Frost Arrow 11":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.FrostArrow] = DateTime.UtcNow;

                            var frostArrowStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastFrostArrow, DateTime.UtcNow },
                                { CreatureState.FrostArrowDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, frostArrowStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Frost Arrow' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Frost Arrow Duration: {creature.GetState<double>(CreatureState.FrostArrowDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Cursed Tune 1":
                    case "Cursed Tune 2":
                    case "Cursed Tune 3":
                    case "Cursed Tune 4":
                    case "Cursed Tune 5":
                    case "Cursed Tune 6":
                    case "Cursed Tune 7":
                    case "Cursed Tune 8":
                    case "Cursed Tune 9":
                    case "Cursed Tune 10":
                    case "Cursed Tune 11":
                    case "Cursed Tune 12":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.CursedTunePoison] = DateTime.UtcNow;

                            var cursedTuneStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastCursedTune, DateTime.UtcNow },
                                { CreatureState.CursedTuneDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, cursedTuneStateUpdates);

                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Cursed Tune' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, CT Duration: {creature.GetState<double>(CreatureState.CursedTuneDuration)}");
                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Regeneration 1":
                    case "Regeneration 2":
                    case "Regeneration 3":
                    case "Regeneration 4":
                    case "Regeneration 5":
                    case "Regeneration 6":
                    case "Regeneration 7":
                    case "Regeneration 8":
                    case "Regeneration 9":
                    case "Regeneration 10":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.Regeneration] = DateTime.UtcNow;

                            var regenStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastRegen, DateTime.UtcNow },
                                { CreatureState.RegenDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, regenStateUpdates);

                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Increased Regeneration":
                        if (creature != null)
                        {
                            creature.AnimationHistory[(ushort)SpellAnimation.IncreasedRegeneration] = DateTime.UtcNow;

                            var increasedRegenStateUpdates = new Dictionary<CreatureState, object>
                            {
                                { CreatureState.LastIncreasedRegen, DateTime.UtcNow },
                                { CreatureState.IncreasedRegenDuration, Spell.GetSpellDuration(spellName) }
                            };

                            CreatureStateHelper.UpdateCreatureStates(client, creature.ID, increasedRegenStateUpdates);

                            client.Server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    default:
                        //Console.WriteLine($"[HandleSpellCastMessage] default case encountered");
                        //Console.WriteLine($"[HandleSpellCastMessage] _creatureToSpellList.Count {client._creatureToSpellList.Count}");
                        if (client.SpellHistory.Count <= 0)
                            client.CastedSpell = null;
                        break;
                }
            }

        }
        private bool ShouldRemoveSpam(string message)
        {
            if (!Settings.Default.removeSpam) return false;

            string[] spamKeywords = new[]
            {
                "Feathers", "Groo", "Keeter", "Torch", "Mermaid", "cradh", "Seal", "fas",
                "ioc", "nuadhaich", "searg", "concentrate", "gar", "snare", "trap", "comlha",
                "pramh", "suain", "Arrow", "dion", "lamh", "find", "ABILITY_"
            };

            return spamKeywords.Any(keyword => message.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void HandleBowMessage(Client client, Match match)
        {
            client.ShouldEquipBow = true;
        }

        private void HandleAoSithMessage(Client client, Match match)
        {
            client.Bot._recentlyAoSithed = true;
        }
        private void HandleInventoryFullMessage(Client client, Match match)
        {
            client.Bot._shouldAlertItemCap = true;
            if (client.ClientTab.toggleFarmBtn.Text == "Disable")
            {
                client.InventoryFull = true;
            }
        }
        private void HandleNecklaceMessage(Client client, Match match)
        {

            string necklaceName = match.Groups[1].Value;

            switch (necklaceName)
            {
                case "Thief's Dark Necklace":
                case "Dark Amber Necklace":
                case "Dark Necklace":
                case "Royal Baem Scale Pendant":
                case "Dark Gold Jade Necklace":
                    client.OffenseElement = "Dark";
                    break;
                case "Lumen Amulet":
                case "Light Necklace":
                case "Ragged Holy Danaan":
                case "Lannair Amulet":
                case "Lionnear Amulet":
                case "Laise Amulet":
                    client.OffenseElement = "Light";
                    break;
                default:
                    client.OffenseElement = "Unknown";
                    break;
            }
        }

        private void HandleLaborMessage(Client client, Match match)
        {
            if (match.Value.Contains("4 Temuairan days"))
            {
                client.CastedSpell = null;
                client.HasLabor = false;
            }
            else if (match.Value.Contains("You do not have time"))
            {
                client.CastedSpell = null;
                client.HasLabor = false;
            }
            else if (match.Value.Contains("You work for"))
            {
                client.ClientTab.laborBtn.Text = "Labor";
            }
            else if (match.Value.Contains("works for you"))
            {
                client.HasLabor = true;
            }
        }


        private void HandleExperienceMessage(Client client, Match match)
        {
            int experience;
            if (int.TryParse(match.Groups[1].Value, out experience))
            {
                client.ClientTab._sessionExperience += (ulong)experience;

                if (client.Map.MapID == 511)//Balanced Arena
                {
                    client.Bot._fowlCount++;
                }
            }

            client.Bot._lastEXP = DateTime.UtcNow;
        }


        private void HandleItemDamageMessage(Client client, Match match)
        {
            string itemName = match.Groups[1].Value;
            int durabilityPercent;
            if (int.TryParse(match.Groups[2].Value, out durabilityPercent))
            {
                HandleDurabilityMessage(client, itemName, durabilityPercent);
            }
        }
        private static void HandleDurabilityMessage(Client client, string itemName, int durabilityPercent)
        {
            string durabilityMessage = $"Your {itemName} durability is at {durabilityPercent}%.";
            client.ServerMessage((byte)ServerMessageType.Whisper, durabilityMessage);
            client.ClientTab.AddMessageToChatPanel(System.Drawing.Color.Crimson, durabilityMessage);

            if (client.ClientTab.alertDuraCbox.Checked && durabilityPercent == 30)
            {
                client.Bot.itemDurabilityAlerts = new bool[] { true, true, true, true, true };
            }

            if (itemName == "Insect Net" && durabilityPercent == 10)
            {
                client.Bot._netRepair = true;
            }

            if (client.ClientTab.equipmentrepairCbox.Checked && durabilityPercent == 10)
            {
                client.NeedsToRepair = true;
            }
        }

        private void HandleTrainingGroundsMessage(Client client, string message)
        {
            client.TrainingGroundsMember = false;
        }
        private void HandleStuckMessage(Client client, string message)
        {
            Console.WriteLine("Stuck message received. Current stuck counter: {0}", client.StuckCounter);
            Console.WriteLine("Message: {0}", message);

            client.Server.RemoveFirstCreatureToSpell(client);
            client.CastedSpell = null;
            client.CastedTarget = null;
            client.StuckCounter++;

            Console.WriteLine("Incremented stuck counter to: {0}", client.StuckCounter);

            // Check conditions to attempt moving out of a stuck position
            if ((client.StuckCounter > 4) && (!client.Bot._rangerNear || !client.ClientTab.rangerStopCbox.Checked))
            {
                Console.WriteLine("Stuck counter > 4 and conditions met to attempt moving.");

                Location serverLocation = client.ServerLocation;
                Console.WriteLine("Current server location: MapID={0}, X={1}, Y={2}", serverLocation.MapID, serverLocation.X, serverLocation.Y);

                // Define the four adjacent tiles (up, right, down, left)
                var adjacentLocations = new List<Location>
                {
                    new Location(serverLocation.MapID, serverLocation.X, (short)(serverLocation.Y - 1)),
                    new Location(serverLocation.MapID, (short)(serverLocation.X + 1), serverLocation.Y),
                    new Location(serverLocation.MapID, serverLocation.X, (short)(serverLocation.Y + 1)),
                    new Location(serverLocation.MapID, (short)(serverLocation.X - 1), serverLocation.Y)
                };

                Console.WriteLine("Checking adjacent tiles for possible path:");
                foreach (var loc in adjacentLocations)
                {
                    Console.WriteLine("  Checking location: MapID={0}, X={1}, Y={2}", loc.MapID, loc.X, loc.Y);
                }

                // Get all creatures nearby
                var creatures = client.GetNearbyObjects().OfType<Creature>().ToList();
                Console.WriteLine("Number of nearby creatures: {0}", creatures.Count);

                // Check each adjacent location
                foreach (var loc in adjacentLocations)
                {
                    bool isWall = client.Map.Tiles[loc.Point].IsWall;
                    bool creatureAtLoc = creatures.Any(c => c.Location == loc);

                    Console.WriteLine("Evaluating location MapID={0}, X={1}, Y={2}: IsWall={3}, CreatureThere={4}", loc.MapID, loc.X, loc.Y, isWall, creatureAtLoc);

                    // Ensure the tile is not a wall and no creature stands there
                    if (!isWall && !creatureAtLoc)
                    {
                        Console.WriteLine("  Attempting pathfind to {0},{1}", loc.X, loc.Y);
                        // Attempt pathfinding to that point
                        if (client.Pathfind(loc, 0))
                        {
                            Console.WriteLine("  Pathfind successful. Breaking out of loop.");
                            // If successful, stop checking other points
                            break;
                        }
                        else
                        {
                            Console.WriteLine("  Pathfind failed at {0},{1}. Trying next location.", loc.X, loc.Y);
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Cannot pathfind here. IsWall={0}, CreatureThere={1}", isWall, creatureAtLoc);
                    }
                }
            }
            else
            {
                Console.WriteLine("Stuck counter <= 4 or conditions not met to attempt moving.");
            }

            if (client.StuckCounter > 6)
            {
                Console.WriteLine("Stuck counter > 6, playing system beep.");
                SystemSounds.Beep.Play();
            }
        }


        private void HandleSpiritWorldMessage(Client client, string message)
        {
            if ((client.SpellHistory.Count > 0) && client.NearbyObjects.Contains(client.SpellHistory[0].Creature.ID))
            {
                client.NearbyObjects.Remove(client.SpellHistory[0].Creature.ID);
                client.Server.RemoveFirstCreatureToSpell(client);
            }
        }

        private void HandleDragonMessage(Client client, string message)
        {
            if (message == "No longer dragon.")
                client.EffectsBar.Remove((ushort)EffectsBar.DragonsFire);
            else
                client.EffectsBar.Add((ushort)EffectsBar.DragonsFire);
        }

        private void HandleDionMessage(Client client, string message)
        {
            if (message == "Your skin turns back to flesh.")
            {
                client.EffectsBar.Remove((ushort)EffectsBar.Dion);

                var dionStateUpdates = new Dictionary<CreatureState, object>
                {
                    { CreatureState.IsDioned, false },
                    { CreatureState.LastDioned, DateTime.MinValue },
                    { CreatureState.DionName, string.Empty },
                    { CreatureState.DionDuration, 0.0 } // Duration in seconds
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, dionStateUpdates);

            }
            else if (message == "Harden body spell")
            {
                client.EffectsBar.Add((ushort)EffectsBar.Dion);

                var dionStateUpdates = new Dictionary<CreatureState, object>
                {
                    { CreatureState.IsDioned, true },
                    { CreatureState.LastDioned, DateTime.UtcNow },
                    { CreatureState.DionDuration, Spell.GetSpellDuration("mor dion") }//if we login and have dion there is no way to know the duration so we assume it is max
                };
                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, dionStateUpdates);

            }

        }

        private void HandleAlreadyCastMessage(Client client, string message)
        {

            if (client.SpellHistory.Count > 0)
            {
                //Console.WriteLine($"[HandleAlreadyCastMessage] Already cast message received on {client._creatureToSpellList[0].Creature.ID}");
                //Console.WriteLine($"[HandleAlreadyCastMessage] _creatureToSpellList.Count before removal: {client._creatureToSpellList.Count}");

                if (client.CastedSpell != null && client.CastedSpell.Name.Contains("fas"))
                {
                    //client._spellHistory[0].Creature.LastFassed = DateTime.UtcNow;
                    //client._spellHistory[0].Creature.FasDuration = Spell.GetSpellDuration(client.CastedSpell.Name);
                    //if (client._spellHistory[0].Creature.ID != client.Player.ID)
                    //    client.UpdateFasTargets(client, client._spellHistory[0].Creature.ID, client._spellHistory[0].Creature.FasDuration);
                    Creature creature = client.SpellHistory[0].Creature;
                    string fasName = client.CastedSpell.Name;

                    //Console.WriteLine($"[HandleAlreadyCastMessage] Received 'You already cast' message for {fasName} on Creature ID: {client._spellHistory[0].Creature?.ID}, Hash: {client._spellHistory[0].Creature?.GetHashCode()} Updating LastFassed.");

                    double fasDuration = Spell.GetSpellDuration(fasName);

                    var fasStateUpdates = new Dictionary<CreatureState, object>
                    {
                        { CreatureState.IsFassed, true },
                        { CreatureState.LastFassed, DateTime.UtcNow },
                        { CreatureState.FasDuration, fasDuration },
                        { CreatureState.FasName, fasName },
                    };

                    // Update the creature's state across all clients
                    CreatureStateHelper.UpdateCreatureStates(client, creature.ID, fasStateUpdates);

                    //Console.WriteLine($"[HandleAlreadyCastMessage] Fas state updated for Creature ID: {creature.ID}");

                    client.CastedSpell = null;
                }
                if (client.CastedSpell != null && client.CastedSpell.Name.Contains("pramh"))
                {
                    client.SpellHistory[0].Creature.AnimationHistory[(ushort)SpellAnimation.Pramh] = DateTime.UtcNow;
                    //-Console.WriteLine($"[UpdateSpellAnimationHistory] 'pramh' cast on Creature ID: {client._creatureToSpellList[0].Creature.ID}, Time: {DateTime.UtcNow}");
                    client.CastedSpell = null;
                }
                if (client.CastedSpell != null && client.CastedSpell.Name.Contains("suain"))
                {
                    client.SpellHistory[0].Creature.AnimationHistory[(ushort)SpellAnimation.Suain] = DateTime.UtcNow;
                    //-Console.WriteLine($"[UpdateSpellAnimationHistory] 'pramh' cast on Creature ID: {client._creatureToSpellList[0].Creature.ID}, Time: {DateTime.UtcNow}");
                    client.CastedSpell = null;
                }


                client.Server.RemoveFirstCreatureToSpell(client);
            }

        }

        private void HandleSuainMessage(Client client, string message)
        {
            if (message == "Your body thaws.")
                client.EffectsBar.Remove((ushort)EffectsBar.Suain);
            else if (message == "Your body is freezing.")
            {
                if (DateTime.UtcNow.Subtract(client.Player.GetState<DateTime>(CreatureState.LastSuained)).TotalSeconds < 6.0)
                {
                    client.EffectsBar.Add((ushort)EffectsBar.Suain);
                    CreatureStateHelper.UpdateCreatureState(client, client.Player.ID, CreatureState.LastSuained, DateTime.UtcNow);
                }

            }
            else if (message == "You are in hibernation.")
            {
                client.EffectsBar.Add((ushort)EffectsBar.Suain);
                CreatureStateHelper.UpdateCreatureState(client, client.Player.ID, CreatureState.LastSuained, DateTime.UtcNow);
            }
        }

        private void HandleDistractedMessage(Client client, string message)
        {
            client.CastedSpell = null;
        }

        private void HandleHaltMessage(Client client, string message)
        {
            if (message == "Halt end.")
                client.EffectsBar.Remove((ushort)EffectsBar.Halt);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Halt);
        }

        private void HandleDeireasFaileasMessage(Client client, string message)
        {
            if (message == "Reflect end.")
                client.EffectsBar.Remove((ushort)EffectsBar.DeireasFaileas);
            else
                client.EffectsBar.Add((ushort)EffectsBar.DeireasFaileas);
        }

        private void HandleMistMessage(Client client, string message)
        {
            if (message == "Your reflexes return to normal.")
                client.EffectsBar.Remove((ushort)EffectsBar.Mist);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Mist);
        }

        private void HandleAsgallMessage(Client client, string message)
        {
            if (message == "asgall faileas end.")
                client.EffectsBar.Remove((ushort)EffectsBar.AsgallFaileas);
            else
                client.EffectsBar.Add((ushort)EffectsBar.AsgallFaileas);
        }

        private void HandleCantAttackMessage(Client client, string message)
        {
            return;
        }

        private void HandleFailedMessage(Client client, string message)
        {
            return;
        }

        private void HandleHideMessage(Client client, string message)
        {
            if (message == "You are no longer invisible.")
                client.EffectsBar.Remove((ushort)EffectsBar.Hide);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Hide);
        }

        private void HandleDallMessage(Client client, string message)
        {
            if (message == "You can see again.")
                client.EffectsBar.Remove((ushort)EffectsBar.Dall);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Dall);
        }

        private void HandleDisenchanterMessage(Client client, string message)
        {
            client.Bot._lastDisenchanterCast = DateTime.UtcNow;
        }


        private void HandleAiteMessage(Client client, string message)
        {
            if (message == "You feel vulnerable again.")
            {
                client.EffectsBar.Remove((ushort)EffectsBar.NaomhAite);

                var aiteStateUpdates = new Dictionary<CreatureState, object>
                {
                    { CreatureState.IsAited, false },
                    { CreatureState.LastAited, DateTime.MinValue },
                    { CreatureState.AiteName, string.Empty },
                    { CreatureState.AiteDuration, 0.0 }
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, aiteStateUpdates);
            }

            else
            {
                client.EffectsBar.Add((ushort)EffectsBar.NaomhAite);

                var aiteStateUpdates = new Dictionary<CreatureState, object>
                {
                    { CreatureState.IsAited, true },
                    { CreatureState.LastAited, DateTime.UtcNow },
                    { CreatureState.AiteName, "ard naomh aite" },// if we login and have aite there is no way to know the duration so we assume it is max
                    { CreatureState.AiteDuration, Spell.GetSpellDuration("ard naomh aite") } // Duration in seconds
                };
            }
        }

        private void HandleBeagSuainMessage(Client client, string message)
        {
            if (message == "You can move again.")
                client.EffectsBar.Remove((ushort)EffectsBar.BeagSuain);
            else
                client.EffectsBar.Add((ushort)EffectsBar.BeagSuain);
        }

        private void HandleFasMessage(Client client, string message)
        {
            if (message == "normal nature.")
            {
                client.EffectsBar.Remove((ushort)EffectsBar.FasNadur);

                var fasStateUpdates = new Dictionary<CreatureState, object> // Adam new
                {
                    { CreatureState.IsFassed, false },
                    { CreatureState.LastFassed, DateTime.MinValue },
                    { CreatureState.FasName, string.Empty },
                    { CreatureState.FasDuration, 0.0 }
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, fasStateUpdates);
            }
            else
            {
                client.EffectsBar.Add((ushort)EffectsBar.FasNadur);

                double fasDuration = Spell.GetSpellDuration("mor fas nadur");

                var fasStateUpdates = new Dictionary<CreatureState, object> //Adam new
                {
                    { CreatureState.IsFassed, true },
                    { CreatureState.LastFassed, DateTime.UtcNow },
                    { CreatureState.FasName, "mor fas nadur" }, // if we login and have fas there is no way to know the duration so we assume it is max
                    { CreatureState.FasDuration, fasDuration }
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, fasStateUpdates);

            }
        }

        private void HandleCurseBeginMessage(Client client, string message)
        {
            lock (_lock)
            {
                double curseDuration = Spell.GetSpellDuration(message);

                var curseStateUpdates = new Dictionary<CreatureState, object> //Adam new
                {
                    { CreatureState.IsCursed, true },
                    { CreatureState.LastCursed, DateTime.UtcNow },
                    { CreatureState.CurseName, message },
                    { CreatureState.CurseDuration, curseDuration } // Duration in seconds
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, curseStateUpdates);
            }
        }

        private void HandleCurseEndMessage(Client client, string message)
        {
            lock (_lock)
            {

                var curseStateUpdates = new Dictionary<CreatureState, object> //Adam new
                {
                    { CreatureState.IsCursed, false },
                    { CreatureState.LastCursed, DateTime.MinValue },
                    { CreatureState.CurseName, String.Empty },
                    { CreatureState.CurseDuration, 0.0 } // Duration in seconds
                };

                CreatureStateHelper.UpdateCreatureStates(client, client.Player.ID, curseStateUpdates);

                Console.WriteLine($"[HandleCurseEndMessage] Curse ended Player ID: {client.Player.ID} on {client.Player.Name}, " +
                    $"Hash: {client.Player.GetHashCode()}. Curse: {client.Player.GetState<DateTime>(CreatureState.CurseName)}, " +
                    $"CurseDuration: {client.Player.GetState<DateTime>(CreatureState.CurseDuration)}, Last Cursed: {client.Player.GetState<DateTime>(CreatureState.LastCursed)}");
            }

        }

        private void HandleWrongMessage(Client client, string message)
        {
            return;
        }

        private void HandleBonusMessage(Client client, string message)
        {
            //Adam check other bonuses - stars/vdays double shrooms
            client.EffectsBar.Add((ushort)EffectsBar.SpellSkillBonus1);
        }

        private void HandlePerfectDefenseMessage(Client client, string message)
        {
            if (message == "You become normal.")
                client.EffectsBar.Remove((ushort)EffectsBar.PerfectDefense);
            else
                client.EffectsBar.Add((ushort)EffectsBar.PerfectDefense);
        }

        private void HandlePurifyMessage(Client client, string message)
        {
            if (message == "Purify end")
                client.EffectsBar.Remove((ushort)EffectsBar.Purify);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Purify);
        }

        private void HandleInnerFireMessage(Client client, string message)
        {
            if (message == "Inner warmth of regeneration dissipates.")
                client.EffectsBar.Remove((ushort)EffectsBar.InnerFire);
            else
                client.EffectsBar.Add((ushort)EffectsBar.InnerFire);
        }

        private void HandleBeannaichMessage(Client client, string message)
        {
            if (message == "End of blessing.")
                client.EffectsBar.Remove((ushort)EffectsBar.Beannaich);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Beannaich);
        }

        private void HandleFasDeireasMessage(Client client, string message)
        {
            if (message == "Normal power.")
                client.EffectsBar.Remove((ushort)EffectsBar.FasDeireas);
            else
                client.EffectsBar.Add((ushort)EffectsBar.FasDeireas);
        }

        private void HandlePauseMessage(Client client, string message)
        {
            if (message == "Pause")
                client.EffectsBar.Add((ushort)EffectsBar.Pause);
            else
                client.EffectsBar.Remove((ushort)EffectsBar.Pause);
        }

        private void HandlePramhMessage(Client client, string message)
        {
            if (message == "Awake")
                client.EffectsBar.Remove((ushort)EffectsBar.Pramh);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Pramh);
        }

        private void HandleCatsMessage(Client client, string message)
        {
            if (message == "You can perceive the invisible.")
                client.EffectsBar.Add((ushort)EffectsBar.EisdCreature);
            else
                client.EffectsBar.Remove((ushort)EffectsBar.EisdCreature);
        }

        private void HandleGroupMessage(Client client, string message)
        {
            if (message == "Group disbanded.")
            {
                client.GroupedPlayers.Clear();
                client.ClientTab.UpdateGroupList();
                client.ClientTab.UpdateStrangerList();
            }
            else if (message == "Already a member of another group.")
            {
                //Already a member of another group.
            }
            else if (message == "Cannot find group member.")
            {
                //Cannot find group member.
            }
        }

        private void HandleNoManaMessage(Client client, string message)
        {
            client.Server.RemoveFirstCreatureToSpell(client);
            client.Bot._needFasSpiorad = true;
        }

        private void HandleCantCastMessage(Client client, string message)
        {
            if (message == "You can't cast a spell.")
            {

            }
            else if (message == "You can't cast that spell right now.")
            {

            }
            else if (message == "You can't use skills here.")
            {
                client.Map.CanUseSkills = false;
            }
            else if (message == "That doesn't work here.")
            {
                client.Map.CanUseSpells = false;
                client.CastedSpell = null;
                client.CastedTarget = null;
                client.Server.RemoveFirstCreatureToSpell(client);
            }
        }

        private void HandleResistMessage(Client client, string message)
        {
            return;
        }
        private void HandleBotChecks(Client client, Match match)
        {
            client.Bot._botChecks = DateTime.UtcNow;
            SystemSounds.Beep.Play();
        }


        private void HandlePreventAfflictionMessage(Client client, Match match)
        {
            if (match.Value.Equals("PreventAffliction end."))
                client.EffectsBar.Remove((ushort)EffectsBar.PreventAffliction);
            else
                client.EffectsBar.Add((ushort)EffectsBar.PreventAffliction);
        }

        private void HandleCurseMessage(Client client, Match match)
        {
            if (client.CastedSpell != null)
            {
                if ((client.SpellHistory.Count > 0) && (client.SpellHistory[0].Creature != null))
                {

                    Creature creature = client.SpellHistory[0].Creature;
                    string curseName = match.Groups[1].Value;

                    Console.WriteLine($"[HandleCurseMessage] Received 'another curse afflicts thee' message for {match.Groups[1].Value} on Creature ID: {client.SpellHistory[0].Creature?.ID}, Hash: {client.SpellHistory[0].Creature?.GetHashCode()} Updating LastCursed.");

                    double curseDuration = Spell.GetSpellDuration(curseName);

                    var curseStateUpdates = new Dictionary<CreatureState, object>
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseDuration, curseDuration },
                        { CreatureState.CurseName, curseName },
                    };

                    // Update the creature's state across all clients
                    CreatureStateHelper.UpdateCreatureStates(client, creature.ID, curseStateUpdates);

                    Console.WriteLine($"[HandleCurseMessage] Curse state updated for Creature ID: {creature.ID}");

                    //client._spellHistory[0].Creature.LastCursed = DateTime.UtcNow;
                    //client._spellHistory[0].Creature.CurseDuration = Spell.GetSpellDuration(match.Groups[1].Value);
                    //client._spellHistory[0].Creature.Curse = match.Groups[1].Value;
                    //Console.WriteLine($"[HandleCurseMessage] LastCursed Updated to {client._spellHistory[0].Creature?.LastCursed}");
                    //client.UpdateCurseTargets(client, client._spellHistory[0].Creature.ID, match.Groups[1].Value);

                    client.Server.RemoveFirstCreatureToSpell(client);
                }
                client.CastedSpell = null;
            }
        }

        private void HandleArmachdMessage(Client client, string message)
        {
            if (message == "Your armor is strengthened.")
                client.EffectsBar.Add((ushort)EffectsBar.Armachd);
            else
                client.EffectsBar.Remove((ushort)EffectsBar.Armachd);
        }

        private void HandlePoisonMessage(Client client, string message)
        {
            if (message == "You feel better.")
                client.EffectsBar.Remove((ushort)EffectsBar.Poison);
            else
                client.EffectsBar.Add((ushort)EffectsBar.Poison);
        }

        private void HandleArenaBroadcastMessage(Client client, string message)
        {
            string messageToSend = "";
            TimeSpan span = new TimeSpan(0, 6, 0) - DateTime.UtcNow.Subtract(client.Server.ArenaAnnounceTimer);

            messageToSend = "You may broadcast again in ";

            if (client.Server.ArenaAnnounceTimer != DateTime.MinValue)
            {
                messageToSend += (span.Minutes > 1) ? $"{span.Minutes} minutes and " :
                                (span.Minutes > 0) ? $"{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")} and " :
                                string.Empty;
            }

            messageToSend += $"{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}.";

            if (!client.ClientTab.safeScreenCbox.Checked)
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, messageToSend);
        }

        private void HandleLastKillMessage(Client client, string message)
        {
            client.Bot._lastEXP = DateTime.UtcNow;
        }
        private void HandleInventoryMessage(Client client, string message)
        {
            client.InventoryFull = true;
        }
        private void HandleRecentlyDiedMessage(Client client, string arg2)
        {
            client.RecentlyDied = true;
        }
        private void HandleJoinLeaveGroupMessage(Client client, Match match)
        {
            client.RequestProfile();
            client.ClientTab.UpdateStrangerList();
        }


    }
}

