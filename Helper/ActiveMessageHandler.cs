using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Enumerations;
using Talos.Forms;
using Talos.Objects;

namespace Talos.Helper
{
    internal class ActiveMessageHandler
    {
        private static readonly ActiveMessageHandler instance = new ActiveMessageHandler();
        private Dictionary<string, Action<Client, string>> stringMessageHandlers;
        private Dictionary<Regex, Action<Client, Match>> regexMessageHandlers;
        private static readonly object _lock = new object();

        // Private constructor to prevent external instantiation
        private ActiveMessageHandler()
        {
            stringMessageHandlers = new Dictionary<string, Action<Client, string>>
            {
                { "You just sent a broadcast, you must wait a few more minutes.", HandleArenaBroadcastMessage },
                { "Your AP is too high", HandleLastKillMessage },
                { "You have reached level 99, the maximum for the free trial.", HandleLastKillMessage },
                { "You can't have more.", HandleInventoryMessage },
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
                { "Demise end.", HandleCurseEndMessage },
                { "beag cradh", HandleCurseBeginMessage },
                { "cradh", HandleCurseBeginMessage },
                { "mor cradh", HandleCurseBeginMessage },
                { "ard cradh", HandleCurseBeginMessage },
                { "Dark Seal", HandleCurseBeginMessage },
                { "Darker Seal", HandleCurseBeginMessage },
                { "Demise", HandleCurseBeginMessage },
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





        // Public static property to provide access to the single instance
        public static ActiveMessageHandler Instance => instance;

        internal bool Handle(Client client, string message)
        {
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

            return false;
        }
        private static void HandleSpellCastMessage(Client client, Match match)
        {
            lock (_lock)
            {
                Creature creature = client._spellHistory.Count > 0 ? client._spellHistory[0].Creature : null;
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
                            creature.AiteDuration = Spell.GetSpellDuration(spellName);
                            creature.LastAited = DateTime.UtcNow;
                        }
                        break;

                    case "beag cradh":
                    case "cradh":
                    case "mor cradh":
                    case "ard cradh":
                    case "Dark Seal":
                    case "Darker Seal":
                    case "Demise":
                        if (creature != null)
                        {
                            creature.Curse = spellName;
                            creature.CurseDuration = Spell.GetSpellDuration(spellName);
                            creature.LastCursed = DateTime.UtcNow;
                            if (creature.ID != client.Player.ID)
                                client.UpdateCurseTargets(client, creature.ID, spellName);//Adam check
                            Console.WriteLine($"[HandleSpellCastMessage] {spellName} cast on Creature ID: {creature?.ID}, Creature Name: {creature?.Name}, Hash: {creature.GetHashCode()}, LastCursed updated to {creature?.LastCursed}");
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "beag fas nadur":
                    case "fas nadur":
                    case "mor fas nadur":
                    case "ard fas nadur":
                        if (creature != null)
                        {
                            creature.FasDuration = Spell.GetSpellDuration(spellName);
                            creature.LastFassed = DateTime.UtcNow;
                            if (creature.ID != client.Player.ID)
                                client.UpdateFasTargets(client, creature.ID, creature.FasDuration);//Adam check
                            Console.WriteLine($"[HandleSpellCastMessage] {spellName} cast on Creature ID: {creature?.ID}. LastFassed updated to {creature?.LastFassed}");
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "dion":
                    case "Draco Stance":
                    case "Stone Skin":
                    case "mor dion":
                    case "Iron Skin":
                    case "Wings of Protection":
                    case "dionLR":
                        client.Player.Dion = spellName;
                        client.Player.LastDioned = DateTime.UtcNow;
                        client.Player.DionDuration = Spell.GetSpellDuration(spellName);
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
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.Armachd] = DateTime.UtcNow;
                            creature.LastArmachd = DateTime.UtcNow;
                            creature.ArmachdDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Armachd' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Sleep Duration: {creature.PramhDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
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
                        client._currentSpell = client.Spellbook["Gem Polishing"];
                        break;

                    case "armachd":
                        if (creature != null)
                        {
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.Armachd] = DateTime.UtcNow;
                            creature.LastArmachd = DateTime.UtcNow;
                            creature.ArmachdDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Armachd' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Sleep Duration: {creature.PramhDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "suain":
                        if (creature != null)
                        {
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.Suain] = DateTime.UtcNow;
                            creature.LastSuained = DateTime.UtcNow;
                            creature.SuainDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Suain' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Suain Duration: {creature.SuainDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Master Karurua Form"://Adam add others //need to figure out how to clear it because there is no orange message when it drops
                        break;

                    case "beag pramh":
                    case "pramh":
                    case "Mesmerize":
                        if (creature != null)
                        {
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.Pramh] = DateTime.UtcNow;
                            creature.LastPramhed = DateTime.UtcNow;
                            creature.PramhDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Sleep' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Sleep Duration: {creature.PramhDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
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
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.FrostArrow] = DateTime.UtcNow;
                            creature.LastFrostArrow = DateTime.UtcNow;
                            creature.FrostArrowDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Frost Arrow' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, Sleep Duration: {creature.FrostArrowDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
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
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.CursedTunePoison] = DateTime.UtcNow;
                            creature.LastCursedTune = DateTime.UtcNow;
                            creature.CursedTuneDuration = Spell.GetSpellDuration(spellName);
                            Console.WriteLine($"[UpdateSpellAnimationHistory] 'Cursed Tune' cast on Creature ID: {creature.ID}, Time: {DateTime.UtcNow}, CT Duration: {creature.CursedTuneDuration}");
                            client._server.RemoveFirstCreatureToSpell(client);
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
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.Regeneration] = DateTime.UtcNow;
                            creature.LastRegen = DateTime.UtcNow;
                            creature.RegenDuration = Spell.GetSpellDuration(spellName);
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    case "Increased Regeneration":
                        if (creature != null)
                        {
                            creature.SpellAnimationHistory[(ushort)SpellAnimation.IncreasedRegeneration] = DateTime.UtcNow;
                            creature.LastIncreasedRegen = DateTime.UtcNow;
                            creature.IncreasedRegenDuration = Spell.GetSpellDuration(spellName);
                            client._server.RemoveFirstCreatureToSpell(client);
                        }
                        break;

                    default:
                        //Console.WriteLine($"[HandleSpellCastMessage] default case encountered");
                        //Console.WriteLine($"[HandleSpellCastMessage] _creatureToSpellList.Count {client._creatureToSpellList.Count}");
                        if (client._spellHistory.Count <= 0)
                            client._currentSpell = null;
                        break;
                }
            }
           
        }

        private void HandleBowMessage(Client client, Match match)
        {
            client._shouldEquipBow = true;
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
                client._inventoryFull = true;
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
                    client._offenseElement = "Dark";
                    break;
                case "Lumen Amulet":
                case "Light Necklace":
                case "Ragged Holy Danaan":
                case "Lannair Amulet":
                case "Lionnear Amulet":
                case "Laise Amulet":
                    client._offenseElement = "Light";
                    break;
                default:
                    client._offenseElement = "Unknown";
                    break;
            }
        }

        private void HandleLaborMessage(Client client, Match match)
        {
            if (match.Value.Contains("4 Temuairan days"))
            {
                client._currentSpell = null;
                client._hasLabor = false;
            }
            else if (match.Value.Contains("You do not have time"))
            {
                client._currentSpell = null;
                client._hasLabor = false;
            }
            else if (match.Value.Contains("You work for"))
            {
                client.ClientTab.laborBtn.Text = "Labor";
            }
            else if (match.Value.Contains("works for you"))
            {
                client._hasLabor = true;
            }
        }


        private void HandleExperienceMessage(Client client, Match match)
        {
            int experience;
            if (int.TryParse(match.Groups[1].Value, out experience))
            {
                client.ClientTab._sessionExperience += (ulong)experience;

                if (client._map.MapID == 511)//Balanced Arena
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
                //client.Bot.bool_8 = new bool[] { true, true, true, true, true };
            }

            if (itemName == "Insect Net" && durabilityPercent == 10)
            {
                //client.Bot.bool_16 = true;
            }
        }
        private void HandleTrainingGroundsMessage(Client client, string message)
        {
            client._trainingGroundsMember = false;
        }
        private void HandleStuckMessage(Client client, string message)
        {
            Console.WriteLine("Stuck message received");
            client._server.RemoveFirstCreatureToSpell(client);
            client._currentSpell = null;
            client.CreatureTarget = null;
            client._stuckCounter++;
            if ((client._stuckCounter > 4) && (!client.Bot._shouldBotStop || !client.ClientTab.rangerStopCbox.Checked))
            {
                //ADAM insert logic to get a list of points around the carachter
                //check for walls, check for creatures etc.
            }
            if (client._stuckCounter > 6)
                SystemSounds.Beep.Play();
        }

        private void HandleSpiritWorldMessage(Client client, string message)
        {
            if ((client._spellHistory.Count > 0) && client.CreatureHashSet.Contains(client._spellHistory[0].Creature.ID))
            {
                client.CreatureHashSet.Remove(client._spellHistory[0].Creature.ID);
                client._server.RemoveFirstCreatureToSpell(client);
            }
        }

        private void HandleDragonMessage(Client client, string message)
        {
            if (message == "No longer dragon.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.DragonsFire);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.DragonsFire);
        }

        private void HandleDionMessage(Client client, string message)
        {
            if (message == "Your skin turns back to flesh.")
            {
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Dion);
                client.Player.Dion = "";
                client.Player.DionDuration = 0.0;
            }
            else if (message == "Harden body spell")
            {
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Dion);
                client.Player.LastDioned = DateTime.UtcNow;
                client.Player.DionDuration = Spell.GetSpellDuration("mor dion");//if we login and have dion there is no way to know the duration so we assume it is max

            }

        }

        private void HandleAlreadyCastMessage(Client client, string message)
        {

            if (client._spellHistory.Count > 0)
            {
                //Console.WriteLine($"[HandleAlreadyCastMessage] Already cast message received on {client._creatureToSpellList[0].Creature.ID}");
                //Console.WriteLine($"[HandleAlreadyCastMessage] _creatureToSpellList.Count before removal: {client._creatureToSpellList.Count}");

                if (client._currentSpell != null && client._currentSpell.Name.Contains("fas"))
                {
                    client._spellHistory[0].Creature.LastFassed = DateTime.UtcNow;
                    client._spellHistory[0].Creature.FasDuration = Spell.GetSpellDuration(client._currentSpell.Name);
                    if (client._spellHistory[0].Creature.ID != client.Player.ID)
                        client.UpdateFasTargets(client, client._spellHistory[0].Creature.ID, client._spellHistory[0].Creature.FasDuration);
                    client._currentSpell = null;
                }
                if (client._currentSpell != null && client._currentSpell.Name.Contains("pramh"))
                {
                    client._spellHistory[0].Creature.SpellAnimationHistory[(ushort)SpellAnimation.Pramh] = DateTime.UtcNow;
                    //-Console.WriteLine($"[UpdateSpellAnimationHistory] 'pramh' cast on Creature ID: {client._creatureToSpellList[0].Creature.ID}, Time: {DateTime.UtcNow}");
                    client._currentSpell = null;
                }
                if (client._currentSpell != null && client._currentSpell.Name.Contains("suain"))
                {
                    client._spellHistory[0].Creature.SpellAnimationHistory[(ushort)SpellAnimation.Suain] = DateTime.UtcNow;
                    //-Console.WriteLine($"[UpdateSpellAnimationHistory] 'pramh' cast on Creature ID: {client._creatureToSpellList[0].Creature.ID}, Time: {DateTime.UtcNow}");
                    client._currentSpell = null;
                }


                client._server.RemoveFirstCreatureToSpell(client);
            }

        }

        private void HandleSuainMessage(Client client, string message)
        {
            if (message == "Your body thaws.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Suain);
            else if (message == "Your body is freezing.")
            {
                if (DateTime.UtcNow.Subtract(client.Player.LastSuained).TotalSeconds < 6.0)
                {
                    client.EffectsBarHashSet.Add((ushort)EffectsBar.Suain);
                    client.Player.LastSuained = DateTime.UtcNow;
                }

            }
            else if (message == "You are in hibernation.")
            {
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Suain);
                client.Player.LastSuained = DateTime.UtcNow;
            }
        }

        private void HandleDistractedMessage(Client client, string message)
        {
            client._currentSpell = null;
        }

        private void HandleHaltMessage(Client client, string message)
        {
            if (message == "Halt end.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Halt);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Halt);
        }

        private void HandleDeireasFaileasMessage(Client client, string message)
        {
            if (message == "Reflect end.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.DeireasFaileas);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.DeireasFaileas);
        }

        private void HandleMistMessage(Client client, string message)
        {
            if (message == "Your reflexes return to normal.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Mist);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Mist);
        }

        private void HandleAsgallMessage(Client client, string message)
        {
            if (message == "asgall faileas end.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.AsgallFaileas);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.AsgallFaileas);
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
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Hide);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Hide);
        }

        private void HandleDallMessage(Client client, string message)
        {
            if (message == "You can see again.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Dall);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Dall);
        }

        private void HandleDisenchanterMessage(Client client, string message)
        {
            client.Bot._lastDisenchanterCast = DateTime.UtcNow;
        }


        private void HandleAiteMessage(Client client, string message)
        {
            if (message == "You feel vulnerable again.")
            {
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.NaomhAite);
                client.Player.AiteDuration = 0.0;
            }
               
            else
            {
                client.EffectsBarHashSet.Add((ushort)EffectsBar.NaomhAite);
                client.Player.LastAited = DateTime.UtcNow;
                client.Player.AiteDuration = Spell.GetSpellDuration("ard naomh aite");//if we login and have aite there is no way to know the duration so we assume it is max
            }    
        }

        private void HandleBeagSuainMessage(Client client, string message)
        {
            if (message == "You can move again.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.BeagSuain);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.BeagSuain);
        }

        private void HandleFasMessage(Client client, string message)
        {
            if (message == "normal nature.")
            {
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.FasNadur);
                client.Player.FasDuration = 0.0;
            }               
            else
            {
                client.EffectsBarHashSet.Add((ushort)EffectsBar.FasNadur);
                client.Player.LastFassed = DateTime.UtcNow;
                client.Player.FasDuration = Spell.GetSpellDuration("mor fas nadur");//if we login and have fas there is no way to know the duration so we assume it is max
            }             
        }

        private void HandleCurseBeginMessage(Client client, string message)
        {
            lock (_lock)
            {
                client.Player.Curse = message;
                client.Player.CurseDuration = Spell.GetSpellDuration(message);
                client.Player.LastCursed = DateTime.UtcNow;
            }
        }

        private void HandleCurseEndMessage(Client client, string message)
        {
            lock (_lock)
            {
                client.Player.Curse = "";
                client.Player.CurseDuration = 0.0;
                client.Player.LastCursed = DateTime.MinValue;
                Console.WriteLine($"[HandleCurseEndMessage] Curse ended Player ID: {client.Player.ID} on {client.Player.Name}, Hash: {client.Player.GetHashCode()}. Curse: {client.Player.Curse}, CurseDuration: {client.Player.CurseDuration}, Last Cursed: {client.Player.LastCursed}");
            }

        }

        private void HandleWrongMessage(Client client, string message)
        {
            return;
        }

        private void HandleBonusMessage(Client client, string message)
        {
            //Adam check other bonuses - stars/vdays double shrooms
            client.EffectsBarHashSet.Add((ushort)EffectsBar.SpellSkillBonus1);
        }

        private void HandlePerfectDefenseMessage(Client client, string message)
        {
            if (message == "You become normal.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.PerfectDefense);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.PerfectDefense);
        }

        private void HandlePurifyMessage(Client client, string message)
        {
            if (message == "Purify end")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Purify);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Purify);
        }

        private void HandleInnerFireMessage(Client client, string message)
        {
            if (message == "Inner warmth of regeneration dissipates.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.InnerFire);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.InnerFire);
        }

        private void HandleBeannaichMessage(Client client, string message)
        {
            if (message == "End of blessing.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Beannaich);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Beannaich);
        }

        private void HandleFasDeireasMessage(Client client, string message)
        {
            if (message == "Normal power.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.FasDeireas);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.FasDeireas);
        }

        private void HandlePauseMessage(Client client, string message)
        {
            if (message == "Pause")
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Pause);
            else
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Pause);
        }

        private void HandlePramhMessage(Client client, string message)
        {
            if (message == "Awake")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Pramh);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Pramh);
        }

        private void HandleCatsMessage(Client client, string message)
        {
            if (message == "You can perceive the invisible.")
                client.EffectsBarHashSet.Add((ushort)EffectsBar.EisdCreature);
            else
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.EisdCreature);
        }

        private void HandleGroupMessage(Client client, string message)
        {
            if (message == "Group disbanded.")
            {
                client.AllyListHashSet.Clear();
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
            client._server.RemoveFirstCreatureToSpell(client);
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
                client._map.CanUseSkills = false;
            }
            else if (message == "That doesn't work here.")
            {
                client._map.CanUseSpells = false;
                client._currentSpell = null;
                client.CreatureTarget = null;
                client._server.RemoveFirstCreatureToSpell(client);
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
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.PreventAffliction);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.PreventAffliction);
        }
        private void HandleCurseMessage(Client client, Match match)
        {
            if (client._currentSpell != null)
            {
                if ((client._spellHistory.Count > 0) && (client._spellHistory[0].Creature != null))
                {
                    Console.WriteLine($"[HandleCurseMessage] Received 'another curse afflicts thee' message for {match.Groups[1].Value} on Creature ID: {client._spellHistory[0].Creature?.ID}. Updating LastCursed.");

                    client._spellHistory[0].Creature.LastCursed = DateTime.UtcNow;
                    client._spellHistory[0].Creature.CurseDuration = Spell.GetSpellDuration(match.Groups[1].Value);
                    client._spellHistory[0].Creature.Curse = match.Groups[1].Value;
                    client.UpdateCurseTargets(client, client._spellHistory[0].Creature.ID, match.Groups[1].Value);
                    client._server.RemoveFirstCreatureToSpell(client);
                }
                client._currentSpell = null;
            }
        }

        private void HandleArmachdMessage(Client client, string message)
        {
            if (message == "Your armor is strengthened.")
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Armachd);
            else
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Armachd);
        }

        private void HandlePoisonMessage(Client client, string message)
        {
            if (message == "You feel better.")
                client.EffectsBarHashSet.Remove((ushort)EffectsBar.Poison);
            else
                client.EffectsBarHashSet.Add((ushort)EffectsBar.Poison);
        }

        private void HandleArenaBroadcastMessage(Client client, string message)
        {
            string messageToSend = "";
            TimeSpan span = new TimeSpan(0, 6, 0) - DateTime.UtcNow.Subtract(client._arenaAnnounceTimer);

            messageToSend = "You may broadcast again in ";

            if (client._arenaAnnounceTimer != DateTime.MinValue)
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
            client._inventoryFull = true;
        }

        private void HandleJoinLeaveGroupMessage(Client client, Match match)
        {
            client.RequestProfile();
            client.ClientTab.UpdateStrangerList();
        }


    }
}

