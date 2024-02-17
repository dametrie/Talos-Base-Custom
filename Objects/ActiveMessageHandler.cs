using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Enumerations;
using Talos.Forms;

namespace Talos.Objects
{
    internal class ActiveMessageHandler
    {
        private static readonly ActiveMessageHandler instance = new ActiveMessageHandler();
        private Dictionary<string, Action<Client, string>> stringMessageHandlers;
        private Dictionary<Regex, Action<Client, Match>> regexMessageHandlers;

        // Private constructor to prevent external instantiation
        private ActiveMessageHandler()
        {
            stringMessageHandlers = new Dictionary<string, Action<Client, string>>
            {
                { "You just sent a broadcast, you must wait a few more minutes.", HandleArenaBroadcastMessage },
                { "Your AP is too high", HandleLastKillMessage },
                { "You have reached level 99, the maximum for the free trial.", HandleLastKillMessage },
                { "You can't have more.", HandleInventoryMessage },
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
                { new Regex("experience!"), HandleExperienceMessage },
                { new Regex("AP went up"), HandleExperienceMessage },
                { new Regex(@"^\(\( 4 Temuairan days = 12 (Terran|real-life) hours \)\)$"), HandleLaborMessage },
                { new Regex(@"You do not have time for these 4 Temuairan days"), HandleLaborMessage },
                { new Regex("[a-zA-Z] works for you for 1 day"), HandleLaborMessage },
                { new Regex(@"You work for \w+, although the Aisling didn't need much done"), HandleLaborMessage },
                { new Regex(@"n:Necklace:([a-zA-Z \'0-9]+)\t Armor class"), HandleNecklaceMessage },
                { new Regex("([a-zA-Z0-9 ]+), You can't have more than [0-9]+"), HandleInventoryFullMessage },
                { new Regex("^.*? ao sith .*?"), HandleAoSithMessage },
                { new Regex("(You have no bow|You do not have a bow equipped)"), HandleBowMessage },
                { new Regex(@"^Another curse afflicts thee\. \[(.*)\]$"), HandleCurseMessage },
                { new Regex(@"^\w+ attacks you with PreventAffliction spell\.$"), HandlePreventAfflictionMessage },
                { new Regex("PreventAffliction end."), HandlePreventAfflictionMessage },
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
            Creature creature = client._creatureToSpellList[0].Creature;
            string spell = match.Groups[1].Value;

            //adam add spells - pramh, ards?
            //You cast Master Karura Form
            switch (spell)
            {
                case "fas spiorad":
                    client.Bot._needFasSpiorad = false;
                    client.Bot._manaLessThanEightyPct = false;
                    break;
                case "mor strioch pian gar":
                    client.Bot._needFasSpiorad = true;
                    break;
                case "io dia armachd comlhaLR":
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
                    creature = client._creatureToSpellList[0].Creature;
                    creature.SpellAnimationHistory[20] = DateTime.UtcNow;
                    client._creatureToSpellList.RemoveAt(0);
                    break;
                case "Mesmerize":
                    creature = client._creatureToSpellList[0].Creature;
                    creature.SpellAnimationHistory[117] = DateTime.UtcNow;
                    client._creatureToSpellList.RemoveAt(0);
                    break;
                case "suain":
                    creature = client._creatureToSpellList[0].Creature;
                    creature.SpellAnimationHistory[40] = DateTime.UtcNow;
                    client._creatureToSpellList.RemoveAt(0);
                    break;
                case "Master Karurua Form"://Adam add others
                    client.AddEffect(EffectsBar.BirdForm);//need to figure out how to clear it because there is no orange message when it drops
                    break;
                default:
                    if (client._creatureToSpellList.Count <= 0)
                        client._currentSpell = null;
                    break;
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
            //Adam update necks
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

            client.Bot._lastKill = DateTime.UtcNow;
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
            client.ServerMessage(0, durabilityMessage);
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

        private void HandleStuckMessage(Client client, string message)
        {
            if (client._creatureToSpellList.Count > 0)
                client._creatureToSpellList.RemoveAt(0);
            client._currentSpell = null;
            client.CreatureTarget = null;
            client.stuckCounter++;
            if ((client.stuckCounter > 4) && (!client.Bot._shouldBotStop || !client.ClientTab.rangerStopCbox.Checked))
            {
                //ADAM insert logic to get a list of points around the carachter
                //check for walls, check for creatures etc.
            }
            if (client.stuckCounter > 6)
                SystemSounds.Beep.Play();
        }

        private void HandleSpiritWorldMessage(Client client, string message)
        {
            if ((client._creatureToSpellList.Count > 0) && client.CreatureHashSet.Contains(client._creatureToSpellList[0].Creature.ID))
            {
                client.CreatureHashSet.Remove(client._creatureToSpellList[0].Creature.ID);
                client._creatureToSpellList.RemoveAt(0);
            }
        }

        private void HandleDragonMessage(Client client, string message)
        {
            if (message == "No longer dragon.")
                client.ClearEffect(EffectsBar.DragonsFire);
            else
                client.AddEffect(EffectsBar.DragonsFire);
        }

        private void HandleDionMessage(Client client, string message)
        {
            if (message == "Your skin turns back to flesh.")
            {
                client.ClearEffect(EffectsBar.Dion);
                client.Player.Dion = "";
                client.Player.DionDuration = 0.0;
            }
            else
            {
                client.AddEffect(EffectsBar.Dion);
                client.Player.Dion = message;
                client.Player.LastDioned = DateTime.UtcNow;
                client.Player.DionDuration = Spell.GetSpellDuration(message);
            }

        }

        private void HandleAlreadyCastMessage(Client client, string message)
        {
            if (client._creatureToSpellList.Count > 0)
                client._server.RemoveFirstCreatureToSpell(client);
        }

        private void HandleSuainMessage(Client client, string message)
        {
            if (message == "Your body thaws.")
                client.ClearEffect(EffectsBar.Suain);
            else if (message == "Your body is freezing.")
            {
                if (DateTime.UtcNow.Subtract(client._lastFrozen).TotalSeconds < 6.0)
                {
                    client.AddEffect(EffectsBar.Suain);
                    client._lastFrozen = DateTime.UtcNow;
                }

            }
            else if (message == "You are in hibernation.")
            {
                client.AddEffect(EffectsBar.Suain);
                client._lastFrozen = DateTime.UtcNow;
            }
        }

        private void HandleDistractedMessage(Client client, string message)
        {
            client._currentSpell = null;
        }

        private void HandleHaltMessage(Client client, string message)
        {
            if (message == "Halt end.")
                client.ClearEffect(EffectsBar.Halt);
            else
                client.AddEffect(EffectsBar.Halt);
        }

        private void HandleDeireasFaileasMessage(Client client, string message)
        {
            if (message == "Reflect end.")
                client.ClearEffect(EffectsBar.DeireasFaileas);
            else
                client.AddEffect(EffectsBar.DeireasFaileas);
        }

        private void HandleMistMessage(Client client, string message)
        {
            if (message == "Your reflexes return to normal.")
                client.ClearEffect(EffectsBar.Mist);
            else
                client.AddEffect(EffectsBar.Mist);
        }

        private void HandleAsgallMessage(Client client, string message)
        {
            if (message == "asgall faileas end.")
                client.ClearEffect(EffectsBar.AsgallFaileas);
            else
                client.AddEffect(EffectsBar.AsgallFaileas);
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
                client.ClearEffect(EffectsBar.Hide);
            else
                client.AddEffect(EffectsBar.Hide);
        }

        private void HandleDallMessage(Client client, string message)
        {
            if (message == "You can see again.")
                client.ClearEffect(EffectsBar.Dall);
            else
                client.AddEffect(EffectsBar.Dall);
        }

        private void HandleDisenchanterMessage(Client client, string message)
        {
            client.Bot._lastDisenchanterCast = DateTime.UtcNow;
        }


        private void HandleAiteMessage(Client client, string message)
        {
            if (message == "You feel vulnerable again.")
                client.ClearEffect(EffectsBar.Aite);
            else
                client.AddEffect(EffectsBar.Aite);
        }

        private void HandleBeagSuainMessage(Client client, string message)
        {
            if (message == "You can move again.")
                client.ClearEffect(EffectsBar.BeagSuain);
            else
                client.AddEffect(EffectsBar.BeagSuain);
        }

        private void HandleFasMessage(Client client, string message)
        {
            if (message == "normal nature.")
                client.ClearEffect(EffectsBar.FasNadur);
            else
                client.AddEffect(EffectsBar.FasNadur);
        }

        private void HandleCurseBeginMessage(Client client, string message)
        {
            client.Player.Curse = message;
            client.Player.CurseDuration = Spell.GetSpellDuration(message);
            client.Player.LastCursed = DateTime.UtcNow;
        }

        private void HandleCurseEndMessage(Client client, string message)
        {
            client.Player.Curse = "";
            client.Player.CurseDuration = 0.0;
        }

        private void HandleWrongMessage(Client client, string message)
        {
            return;
        }

        private void HandleBonusMessage(Client client, string message)
        {
            //Adam check other bonuses - stars/vdays double shrooms
            client.AddEffect(EffectsBar.SpellSkillBonus1);
        }

        private void HandlePerfectDefenseMessage(Client client, string message)
        {
            if (message == "You become normal.")
                client.ClearEffect(EffectsBar.PerfectDefense);
            else
                client.AddEffect(EffectsBar.PerfectDefense);
        }

        private void HandlePurifyMessage(Client client, string message)
        {
            if (message == "Purify end")
                client.ClearEffect(EffectsBar.Purify);
            else
                client.AddEffect(EffectsBar.Purify);
        }

        private void HandleInnerFireMessage(Client client, string message)
        {
            if (message == "Inner warmth of regeneration dissipates.")
                client.ClearEffect(EffectsBar.InnerFire);
            else
                client.AddEffect(EffectsBar.InnerFire);
        }

        private void HandleBeannaichMessage(Client client, string message)
        {
            if (message == "End of blessing.")
                client.ClearEffect(EffectsBar.Beannaich);
            else
                client.AddEffect(EffectsBar.Beannaich);
        }

        private void HandleFasDeireasMessage(Client client, string message)
        {
            if (message == "Normal power.")
                client.ClearEffect(EffectsBar.FasDeireas);
            else
                client.AddEffect(EffectsBar.FasDeireas);
        }

        private void HandlePauseMessage(Client client, string message)
        {
            if (message == "Pause")
                client.AddEffect(EffectsBar.Pause);
            else
                client.ClearEffect(EffectsBar.Pause);
        }

        private void HandlePramhMessage(Client client, string message)
        {
            if (message == "Awake")
                client.ClearEffect(EffectsBar.Pramh);
            else
                client.AddEffect(EffectsBar.Pramh);
        }

        private void HandleCatsMessage(Client client, string message)
        {
            if (message == "You can perceive the invisible.")
                client.AddEffect(EffectsBar.EisdCreature);
            else
                client.ClearEffect(EffectsBar.EisdCreature);
        }

        private void HandleGroupMessage(Client client, string message)
        {
            if (message == "Group disbanded.")
            {
                //Adam add logic to clear group member vars allyhashes
                //update group list
                //update stranger list
            }
            else if (message == "Already a member of another group.")
            {
                //Cannot find group member.
            }
            else if (message == "Cannot find group member.")
            {
                //Cannot find group member.
            }
        }

        private void HandleNoManaMessage(Client client, string message)
        {
            if (client._creatureToSpellList.Count > 0)
                client._creatureToSpellList.RemoveAt(0);
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
                if (client._creatureToSpellList.Count > 0)
                    client._creatureToSpellList.RemoveAt(0);
            }
        }

        private void HandleResistMessage(Client client, string message)
        {
            return;
        }

        private void HandlePreventAfflictionMessage(Client client, Match match)
        {
            if (match.Value.Equals("PreventAffliction end."))
                client.ClearEffect(EffectsBar.PreventAffliction);
            else
                client.AddEffect(EffectsBar.PreventAffliction);
        }
        private void HandleCurseMessage(Client client, Match match)
        {
            if ((client._creatureToSpellList.Count > 0) && (client._creatureToSpellList[0].Creature != null))
            {
                client._creatureToSpellList[0].Creature.LastCursed = DateTime.UtcNow;
                client._creatureToSpellList[0].Creature.CurseDuration = 30.0;
                client._creatureToSpellList[0].Creature.Curse = match.Groups[1].Value;
                client.UpdateCurseTargets(client, client._creatureToSpellList[0].Creature.ID, match.Groups[1].Value);
                client._creatureToSpellList.RemoveAt(0);
            }
            client._currentSpell = null;
        }

        private void HandleArmachdMessage(Client client, string message)
        {
            if (message == "Your armor is strengthened.")
                client.AddEffect(EffectsBar.Armachd);
            else
                client.ClearEffect(EffectsBar.Armachd);
        }

        private void HandlePoisonMessage(Client client, string message)
        {
            if (message == "You feel better.")
                client.ClearEffect(EffectsBar.Poison);
            else
                client.AddEffect(EffectsBar.Poison);
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
                client.ServerMessage(3, messageToSend);
        }

        private void HandleLastKillMessage(Client client, string message)
        {
            client.Bot._lastKill = DateTime.UtcNow;
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

