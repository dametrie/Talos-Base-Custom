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

            };

            regexMessageHandlers = new Dictionary<Regex, Action<Client, Match>>
            {
                { new Regex(@"^([a-zA-Z]+) is (?:joining|leaving) this group.$"), HandleGroupMessage },
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
            };
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

        private void HandleGroupMessage(Client client, Match match)
        {
            client.RequestProfile();
            client.ClientTab.UpdateStrangerList();
        }


    }
}

