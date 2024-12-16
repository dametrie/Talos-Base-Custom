using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Objects;

namespace Talos.Helper
{
    internal delegate void CommandHandler(Client client, string fullMessage, string[] args);

    internal sealed class CommandManager
    {
        private static readonly CommandManager _instance = new CommandManager();
        private readonly Dictionary<string, CommandHandler> _commands = new Dictionary<string, CommandHandler>();

        public static CommandManager Instance => _instance;
        public CommandManager()
        {
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            AddCommand("/form", CommandHandler_Form);
            AddCommand("/effect", CommandHandler_Effect);
            AddCommand("/stat", CommandHandler_Stat);
            AddCommand("/hp", CommandHandler_HP);
            AddCommand("/mp", CommandHandler_MP);
            AddCommand("/repair", CommandHandler_Repair);
            AddCommand("/withdraw", CommandHandler_Withdraw);
            AddCommand("/deposit", CommandHandler_Deposit);
            AddCommand("/g", CommandHandler_Group);
            AddCommand("/fg", CommandHandler_ForceGroup);
            AddCommand("/f", CommandHandler_Follow);
            AddCommand("/af", CommandHandler_AllFollow);
            AddCommand("/assails", CommandHandler_Assails);
            AddCommand("/help", CommandHandler_Help);
            AddCommand("/send", CommandHandler_Send);
            AddCommand("/receive", CommandHandler_Receive);
            AddCommand("/kills", CommandHandler_arenaKills);
            AddCommand("/search", CommandHandler_search);
            AddCommand("/killcount", CommandHandler_killCount);
            AddCommand("/ladder", CommandHandler_ladder);
            AddCommand("/sprint", CommandHandler_sprint);
            AddCommand("/nosg", CommandHandler_SpikeGame);
            AddCommand("/wd", CommandHandler_waterDungeon);
            AddCommand("/strup", CommandHandler_StrUp);
            AddCommand("/ss", CommandHandler_SapphireStream);
            AddCommand("/sw", CommandHandler_ShinewoodHome);
            AddCommand("/loures", CommandHandler_Loures);
            AddCommand("/canals", CommandHandler_Canals);
            AddCommand("/bm", CommandHandler_BlackMarket);
            AddCommand("/ac", CommandHandler_GladArena);
            AddCommand("/fc", CommandHandler_FC);
            AddCommand("/plamboss", CommandHandler_PlamBoss);
            AddCommand("/clr", CommandHandler_Clear);
            AddCommand("/walk", CommandHandler_Walk);
            AddCommand("/chests", CommandHandler_Chests);
            AddCommand("/raffle", CommandHandler_Raffle);
            AddCommand("/ascendstate", CommandHandler_AscendState);
            AddCommand("/load", CommandHandler_LoadProfile);
        }

        private void CommandHandler_Form(Client client, string fullMessage, string[] args)
        {
            if (args.Length == 0)
            {
                client.ClientTab.SetMonsterForm(!client.SpriteOverrideEnabled, client._spriteOverride);
                return;
            }

            if (args.Length == 1 && ushort.TryParse(args[0], out ushort spriteNumber))
            {
                if (spriteNumber >= 1 && spriteNumber <= 1000)
                {
                    client.ClientTab.SetMonsterForm(true, spriteNumber);
                }
                else
                {
                    client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Please choose a monster sprite between 1 and 1000.");
                }
            }
            else
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid input. Please provide a valid sprite number.");
            }
        }
        private void CommandHandler_Effect(Client client, string fullMessage, string[] args)
        {
            if (args.Length != 1 || !ushort.TryParse(args[0], out ushort animation))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid input. Please provide a valid animation ID.");
                return;
            }

            client.SendAnimation(animation, 100);
        }
        private void CommandHandler_Stat(Client client, string fullMessage, string[] args)
        {

            Creature npc = client.GetNearbyNPC("Aoife");
            if (npc == null)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "No nearby NPC named 'Aoife' found.");
                return;
            }

            // Validate input arguments
            if (args.Length != 1)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid input. Please specify a stat (str, int, wis, con, dex).");
                return;
            }

            // Map the stat argument to an option
            string stat = args[0].ToLower();
            byte option = stat switch
            {
                "str" => 1,
                "int" => 3,
                "wis" => 4,
                "con" => 2,
                "dex" => 5,
                _ => 0 // Invalid stat
            };

            // If the option is invalid, provide feedback and exit
            if (option == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid stat. Please specify one of the following: str, int, wis, con, dex.");
                return;
            }

            client.PursuitRequest(1, npc.ID, 1693);
            // Execute the dialog and pursuit interactions
            if (client.WaitForDialog())
            {
                client.ReplyDialog(1, npc.ID, 669, 38);
                client.ReplyDialog(1, npc.ID, 669, 39);
                client.ReplyDialog(1, npc.ID, 669, 40);
                client.ReplyDialog(1, npc.ID, 669, 41, 1);
                client.ReplyDialog(1, npc.ID, 669, 237, option);
            }
        }
        private void CommandHandler_HP(Client client, string fullMessage, string[] args)
        {
            const int maxRetries = 3; // Maximum retries to prevent stack overflow
            int retryCount = 0;

        RetryCommand:

            Creature nearbyNPC = client.GetNearbyNPC("Deoch");
            if (nearbyNPC == null)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Could not find the NPC 'Deoch' nearby.");
                return;
            }

            // Check for valid arguments
            if (args.Length != 1 || (!int.TryParse(args[0], out int num1) && args[0]?.ToUpper() != "ALL"))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid argument. Please provide a number or 'ALL'.");
                return;
            }

            // Interact with the NPC
            client.ClickObject(nearbyNPC.ID);
            client.ReplyDialog(1, nearbyNPC.ID, 626, 2, 1);
            client.ReplyDialog(1, nearbyNPC.ID, 626, 28);

            DateTime startTime = DateTime.UtcNow;

            while (client._npcDialog == null || !client._npcDialog.Contains("your maximum health to raise your health by"))
            {
                if (DateTime.UtcNow.Subtract(startTime).TotalSeconds > 2.0)
                {
                    if (++retryCount > maxRetries)
                    {
                        client.ServerMessage((byte)ServerMessageType.Whisper, "Max retries reached. Aborting command.");
                        return;
                    }

                    client.ServerMessage((byte)ServerMessageType.Whisper, "Timeout waiting for NPC response. Retrying...");
                    goto RetryCommand;
                }

                Thread.Sleep(50); // Slightly increased delay
            }

            // Parse the health cost from the NPC dialog
            Match match = Regex.Match(client._npcDialog, @"\(\(It costs ([0-9]+) health");
            if (!match.Success)
            {
                client.ServerMessage((byte)ServerMessageType.Whisper, "Failed to parse health cost from NPC dialog.");
                return;
            }

            if (!uint.TryParse(match.Groups[1].Value, out uint baseHP) || baseHP == 0)
            {
                client.ServerMessage((byte)ServerMessageType.Whisper, "Invalid health cost parsed from NPC dialog.");
                return;
            }

            client.BaseHP = baseHP;

            // Begin dialog response process
            client.ReplyDialog(1, nearbyNPC.ID, 626, 30);
            client.ReplyDialog(1, nearbyNPC.ID, 626, 51, 2);

            // If "ALL" is specified, calculate the number of upgrades
            if (num1 == 0 && args[0]?.ToUpper() == "ALL")
            {
                num1 = AscendCalc(client, "HP");
            }

            for (int i = 1; i < num1; i++)
            {
                client.ReplyDialog(1, nearbyNPC.ID, 626, 80, 2);
                client.ReplyDialog(1, nearbyNPC.ID, 626, 51, 2);
                Thread.Sleep(5);
            }

            Thread.Sleep(1000);
            client.ReplyDialog(1, nearbyNPC.ID, 626, 80, 1);
            client.ReplyDialog(1, nearbyNPC.ID, 626, 85, 3);
        }
        private void CommandHandler_MP(Client client, string fullMessage, string[] args)
        {
            const int maxRetries = 3; // Maximum retries to prevent infinite recursion
            int retryCount = 0;

        RetryCommand:

            Creature nearbyNPC = client.GetNearbyNPC("Gramail");
            if (nearbyNPC == null)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Could not find the NPC 'Gramail' nearby.");
                return;
            }

            // Check for valid arguments
            if (args.Length != 1 || (!int.TryParse(args[0], out int num1) && args[0]?.ToUpper() != "ALL"))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid argument. Please provide a number or 'ALL'.");
                return;
            }

            // Interact with the NPC
            client.ClickObject(nearbyNPC.ID);
            client.ReplyDialog(1, nearbyNPC.ID, 627, 2, 1);
            client.ReplyDialog(1, nearbyNPC.ID, 627, 28);

            DateTime startTime = DateTime.UtcNow;

            while (client._npcDialog == null || !client._npcDialog.Contains("maximum mana to raise your mana by"))
            {
                if (DateTime.UtcNow.Subtract(startTime).TotalSeconds > 2.0)
                {
                    if (++retryCount > maxRetries)
                    {
                        client.ServerMessage((byte)ServerMessageType.Whisper, "Max retries reached. Aborting command.");
                        return;
                    }

                    client.ServerMessage((byte)ServerMessageType.Whisper, "Timeout waiting for NPC response. Retrying...");
                    goto RetryCommand;
                }

                Thread.Sleep(50); // Slightly increased delay
            }

            // Parse the mana cost from the NPC dialog
            Match match = Regex.Match(client._npcDialog, @"\(\(It costs ([0-9]+) mana");
            if (!match.Success)
            {
                client.ServerMessage((byte)ServerMessageType.Whisper, "Failed to parse mana cost from NPC dialog.");
                return;
            }

            if (!uint.TryParse(match.Groups[1].Value, out uint baseMP) || baseMP == 0)
            {
                client.ServerMessage((byte)ServerMessageType.Whisper, "Invalid mana cost parsed from NPC dialog.");
                return;
            }

            client.BaseMP = baseMP;

            // Begin dialog response process
            client.ReplyDialog(1, nearbyNPC.ID, 627, 30);
            client.ReplyDialog(1, nearbyNPC.ID, 627, 47, 2);

            // If "ALL" is specified, calculate the number of upgrades
            if (num1 == 0 && args[0]?.ToUpper() == "ALL")
            {
                num1 = AscendCalc(client, "MP");
            }

            for (int i = 1; i < num1; i++)
            {
                client.ReplyDialog(1, nearbyNPC.ID, 627, 73, 2);
                client.ReplyDialog(1, nearbyNPC.ID, 627, 47, 2);
                Thread.Sleep(5);
            }

            Thread.Sleep(1000);
            client.ReplyDialog(1, nearbyNPC.ID, 627, 73, 1);
            client.ReplyDialog(1, nearbyNPC.ID, 627, 78, 3);
        }
        private int AscendCalc(Client client, string HPorMP)
        {
            int increment = HPorMP == "HP" ? 50 : 25;
            uint baseValue = HPorMP == "HP" ? client.BaseHP : client.BaseMP;
            float experience = client.Experience;
            int count = 0;

            while (experience >= ((baseValue + (increment * count)) * 500))
            {
                experience -= (baseValue + (increment * count)) * 500;
                count++;
            }

            return count;
        }
        private void CommandHandler_Repair(Client client, string fullMessage, string[] args)
        {
            // Combine arguments into a single string
            string itemName = string.Join(" ", args).Trim();

            if (string.IsNullOrWhiteSpace(itemName))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Please specify an item to repair or use 'all'.");
                return;
            }

            // Get the nearest NPC
            Creature nearestNPC = client.GetNearbyNPCs()
                .OrderBy(npc => npc.Location.DistanceFrom(client._serverLocation))
                .FirstOrDefault();

            if (nearestNPC == null || nearestNPC.Location.DistanceFrom(client._serverLocation) > 12)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You need a merchant nearby to use this command.");
                return;
            }

            if (itemName.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                // Repair all items
                client.PursuitRequest(1, nearestNPC.ID, 92);
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Repairing all items.");
                return;
            }

            // Check for the specified item in inventory
            Item itemToRepair = client.Inventory
                .FirstOrDefault(item => item != null && item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (itemToRepair == null)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"You do not have \"{itemName}\" in your inventory.");
                return;
            }

            // Repair the specific item
            client.PursuitRequest(1, nearestNPC.ID, 90, 1, itemToRepair.Slot);
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Repairing \"{itemName}\".");
        }
        private void CommandHandler_Withdraw(Client client, string fullMessage, string[] args)
        {
            // Validate arguments
            if (args.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /withdraw <money|items|item_name> [quantity]");
                return;
            }

            // Find the nearest NPC
            Creature npc = client.GetNearbyNPCs()
                .OrderBy(npc => npc.Location.DistanceFrom(client._serverLocation))
                .FirstOrDefault();

            if (npc == null || npc.Location.DistanceFrom(client._serverLocation) > 12)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You need a merchant nearby to use this command.");
                return;
            }

            // Determine action type
            string action = args[0].ToLower();
            switch (action)
            {
                case "money":
                    client.PursuitRequest(1, npc.ID, 68); // Withdraw money
                    return;

                case "items":
                    client.PursuitRequest(1, npc.ID, 69); // Withdraw items
                    return;

                default:
                    break;
            }

            // Handle specific item or money withdrawals
            string itemName = string.Join(" ", args).Trim();
            if (int.TryParse(itemName, out int moneyAmount))
            {
                // Withdraw money by amount
                client.WithdrawMoney(npc.ID, moneyAmount);
            }
            else if (args.Length > 1 && int.TryParse(args[args.Length - 1], out int itemQuantity))
            {
                // Withdraw specific item with quantity
                string cleanItemName = itemName.Replace(" " + args[args.Length - 1], "").Trim();
                client.WithdrawItem(npc.ID, cleanItemName, itemQuantity);
            }
            else
            {
                // Withdraw specific item without quantity
                client.WithdrawItem(npc.ID, itemName);
            }
        }
        private void CommandHandler_Deposit(Client client, string fullMessage, string[] args)
        {
            // Validate arguments
            if (args.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /deposit <money|items|item_name> [quantity]");
                return;
            }

            // Find the nearest NPC
            Creature npc = client.GetNearbyNPCs()
                .OrderBy(npc => npc.Location.DistanceFrom(client._serverLocation))
                .FirstOrDefault();

            if (npc == null || npc.Location.DistanceFrom(client._serverLocation) > 12)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You need a merchant nearby to use this command.");
                return;
            }

            // Determine action type
            string action = args[0].ToLower();
            switch (action)
            {
                case "money":
                    client.PursuitRequest(1, npc.ID, 66); // Deposit money
                    return;

                case "items":
                    client.PursuitRequest(1, npc.ID, 67); // Deposit items
                    return;

                default:
                    break;
            }

            // Handle specific item or money deposits
            string itemName = string.Join(" ", args);
            if (int.TryParse(itemName, out int moneyAmount))
            {
                // Deposit specified money amount
                client.DepositMoney(npc.ID, moneyAmount);
                return;
            }

            if (args.Length > 1 && int.TryParse(args[args.Length - 1], out int itemQuantity))
            {
                // Deposit specific item with quantity
                string cleanItemName = itemName.Replace(" " + args[args.Length - 1], "").Trim();
                client.DepositItem(npc.ID, cleanItemName, itemQuantity);
            }
            else
            {
                // Deposit specific item without quantity
                client.DepositItem(npc.ID, itemName); 
            }
        }
        private void CommandHandler_Group(Client client, string fullMessage, string[] args)
        {
            // Ensure there are arguments
            if (args.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /g <alts|name>");
                return;
            }

            string target = args[0].Trim();

            // Handle "alts" argument
            if (string.Equals(target, "alts", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Client otherClient in client._server.Clients)
                {
                    if (otherClient.ClientTab != null && !string.IsNullOrEmpty(otherClient.Name) && client != otherClient)
                    {
                        client.RequestGroup(otherClient.Name);
                    }
                }
                return;
            }

            // Check against known rangers
            if (CONSTANTS.KNOWN_RANGERS.Any(ranger => ranger.Equals(target, StringComparison.OrdinalIgnoreCase)))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You cannot group rangers!");
                return;
            }

            // Send invite
            client.RequestGroup(target);
        }
        private void CommandHandler_ForceGroup(Client client, string fullMessage, string[] args)
        {
            // Ensure there is exactly one argument
            if (args.Length != 1)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /fg <name>");
                return;
            }

            string target = args[0].Trim().ToUpper();

            // Check if the target is a ranger
            if (CONSTANTS.KNOWN_RANGERS.Any(ranger => ranger.Equals(target, StringComparison.OrdinalIgnoreCase)))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You cannot force group rangers!");
                return;
            }

            // Force join the target group
            client.RequestGroupForced(target);

        }
        private void CommandHandler_Follow(Client client, string fullMessage, string[] args)
        {
            // Check if no arguments are provided
            if (args.Length == 0)
            {
                client.ClientTab.followCbox.Checked = false;
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Follow disabled.");
                return;
            }

            // Enable follow checkbox and set the follow target
            client.ClientTab.followCbox.Checked = true;
            client.ClientTab.followText.Text = args[0];
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Following: {args[0]}");

            // If a distance is provided, try to parse it
            if (args.Length > 1)
            {
                if (decimal.TryParse(args[1], out decimal followDistance))
                {
                    client.ClientTab.followDistanceNum.Value = followDistance;
                    client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Follow distance set to: {followDistance}");
                }
                else
                {
                    client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /f <target> [distance]");
                }
            }
        }
        private void CommandHandler_AllFollow(Client client, string fullMessage, string[] args)
        {
            // Check for too many arguments
            if (args.Length > 1)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /af <target_name>");
                return;
            }

            if (args.Length == 0)
            {
                // Disable follow for all other clients
                foreach (Client otherClient in client._server.Clients)
                {
                    if (!ReferenceEquals(otherClient, client))
                    {
                        otherClient.ClientTab.followCbox.Checked = false;
                        otherClient.ServerMessage((byte)ServerMessageType.ActiveMessage, "Follow disabled.");
                    }
                }
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Follow disabled for all clients.");
            }
            else
            {
                int followDistance = 2;

                foreach (Client otherClient in client._server.Clients)
                {
                    if (!ReferenceEquals(otherClient, client))
                    {
                        otherClient.ClientTab.followCbox.Checked = true;
                        otherClient.ClientTab.followText.Text = args[0];

                        // Set a unique follow distance for each client
                        otherClient.ClientTab.followDistanceNum.Value = followDistance++;
                        otherClient.ServerMessage((byte)ServerMessageType.ActiveMessage,
                            $"Following: {args[0]} at distance {followDistance - 1}");
                    }
                }

                client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"All clients are now following: {args[0]}.");
            }
        }
        private void CommandHandler_Assails(Client client, string fullMessage, string[] args)
        {
            client._assailNoise = !client._assailNoise;

            string status = client._assailNoise ? "disabled" : "enabled";
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Assail sounds have been {status}.");
        }

        private void CommandHandler_Help(Client client, string fullMessage, string[] args)
        {
            client.ServerMessage((byte)ServerMessageType.Whisper, "Available commands:");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            // General Gameplay
            client.ServerMessage((byte)ServerMessageType.Whisper, "General Gameplay:");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/form      <number>          Toggle or sets a monster form.");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/sprint       -              Sprint potion becomes a Charge skill");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/load      <name> [all]      Load single profile or for all clients");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/clr        [all]            Clear profile or for all clients");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            // Movement and Groups
            client.ServerMessage((byte)ServerMessageType.Whisper, "Movement and Groups:");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/f         <name> [dist]      Follow target at a specific distance");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/af        <name>             All clients follow the given target");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/g         <name>, 'alts'     Invite target or all alts to a group");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/fg        <name>             Force group invite to the target");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            // Inventory Management
            client.ServerMessage((byte)ServerMessageType.Whisper, "Inventory Management: (all but search requires an NPC on screen)");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/repair    <item>, 'all'      Repairs specific item or all items");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/withdraw  'money', 'items'   Withdraw money or items from bank");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/deposit   'money', 'items'   Deposit money or items to the bank");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/send        -                Open the send items menu");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/receive     -                Open the receive menu");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/search    <item>             Search inventory and bank for an item");//MAX at 67 chars
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            // Stats and Progression
            client.ServerMessage((byte)ServerMessageType.Whisper, "Stats and Progression:");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/stat      <stat>             Increase a stat by 1 in the ToC");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/hp        <num>, 'all'       Raise HP # of times or max possible");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/mp        <num>, 'all'       Raise MP # of times or max possible");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/strup     <number>           Allocate # of points to Strength");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            // Miscellaneous
            client.ServerMessage((byte)ServerMessageType.Whisper, "Miscellaneous:");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/assails     -                Toggle assail sounds on or off");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/effect    <number>           Displays a visual effect animation.");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/ladder      -                Toggle auto-click for ladders");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/wd        'all', 'exit'      Move alts to next floor or exit WD");
            client.ServerMessage((byte)ServerMessageType.Whisper, "/kills       -                Show your top five kills");
            client.ServerMessage((byte)ServerMessageType.Whisper, "-------------------------------------------------------------------");
        }

        private void CommandHandler_Send(Client client, string fullMessage, string[] args)
        {
            Creature npc = client.GetNearbyNPCs()
                .OrderBy(npc => npc.Location.DistanceFrom(client._serverLocation))
                .FirstOrDefault();

            if (npc == null || npc.Location.DistanceFrom(client._serverLocation) > 12)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You need an NPC nearby to use this command.");
                return;
            }

            client.PursuitRequest(1, npc.ID, 96);
        }
        private void CommandHandler_Receive(Client client, string fullMessage, string[] args)
        {
            Creature npc = client.GetNearbyNPCs()
                .OrderBy(npc => npc.Location.DistanceFrom(client._serverLocation))
                .FirstOrDefault();

            if (npc == null || npc.Location.DistanceFrom(client._serverLocation) > 12)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You need an NPC nearby to use this command.");
                return;
            }

            client.PursuitRequest(1, npc.ID, 100);
        }

        private void CommandHandler_arenaKills(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_search(Client client, string fullMessage, string[] args)
        {
            // Ensure a search term is provided
            if (args.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /search <item_name>");
                return;
            }

            // Combine arguments into a single search string
            string searchQuery = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid search query. Please provide a valid item name.");
                return;
            }

            // Define search parameters
            string searchPattern = "*.txt";
            string inventoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory");

            // Check if the inventory directory exists
            if (!Directory.Exists(inventoryPath))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Inventory directory not found.");
                return;
            }

            // Search for matching items
            bool itemFound = false;
            foreach (string filePath in Directory.GetFiles(inventoryPath, searchPattern, SearchOption.AllDirectories))
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    if (line.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Extract inventory and file information
                        string[] pathParts = filePath.Replace(inventoryPath, "").Trim('\\').Split('\\');
                        if (pathParts.Length < 2)
                        {
                            continue; // Skip invalid file paths
                        }

                        string owner = pathParts[0];
                        string inventoryName = Path.GetFileNameWithoutExtension(pathParts[1]);

                        // Send a message for the found item
                        client.ServerMessage(
                            (byte)ServerMessageType.ActiveMessage,
                            $"{Utility.UppercaseFirst(owner)}'s {inventoryName} contains {line}."
                        );

                        itemFound = true;
                    }
                }
            }

            // Notify if no matches were found
            if (!itemFound)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"No matches found for '{searchQuery}'.");
            }
        }
       
        private void CommandHandler_killCount(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_ladder(Client client, string fullMessage, string[] args)
        {
            client._ladder = !client._ladder;

            string status = client._ladder ? "disabled" : "enabled";
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Auto click ladder has been {status}.");
        }

        private void CommandHandler_sprint(Client client, string fullMessage, string[] args)
        {
            // Check if the client has a Sprint Potion in inventory or already has the Charge skill
            if (!client.Inventory.Contains("Sprint Potion") || client.HasSkill("Charge"))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You either lack a Sprint Potion or already have the 'Charge' skill.");
                return;
            }

            // Find the first available skill slot
            byte availableSlot = 0;
            for (byte i = 1; i <= client.Skillbook.MaxSkills; i++)
            {
                if (!client.Skillbook.SkillbookDictionary.Values.Any(skill => skill.Slot == i))
                {
                    availableSlot = i;
                    break;
                }
            }

            if (availableSlot == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "No available skill slots to add the 'Charge' skill.");
                return;
            }

            // Create the 'Charge' skill and add it to the client's SkillBook
            Skill chargeSkill = new Skill(availableSlot, "Charge", 49, 100, 100)
            {
                Ticks = 16.0
            };

            client.Skillbook.AddOrUpdateSkill(chargeSkill);
            client.AddSkill(chargeSkill);

            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"The 'Charge' skill has been added to your skill book in slot {availableSlot}.");
        }


        private void CommandHandler_SpikeGame(Client client, string fullMessage, string[] args)
        {
            client.Bot._spikeGameToggle = !client.Bot._spikeGameToggle;

            string status = client.Bot._spikeGameToggle ? "off" : "on";
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Auto spike game has been toggled {status}.");
        }

        private void CommandHandler_waterDungeon(Client client, string fullMessage, string[] args)
        {
            // Check if arguments are provided
            if (args.Length > 0)
            {
                string action = args[0].ToLower();

                if (action == "all")
                {
                    foreach (var otherClient in client._server.Clients)
                    {
                        if (otherClient._map.Name.Contains("Water Dungeon"))
                        {
                            otherClient.PublicMessage(1, "Water Spirit, I have done what you have asked of me.");
                        }
                    }
                    return;
                }

                if (action == "exit")
                {
                    foreach (var otherClient in client._server.Clients)
                    {
                        if (otherClient._map.Name.Contains("Water Dungeon"))
                        {
                            otherClient.PublicMessage(1, "Water Spirit, I cannot continue, I must rest.");
                        }
                    }
                    return;
                }

                // Invalid argument message
                client.ServerMessage((byte)ServerMessageType.ActiveMessage,
                    "Invalid argument. Use '/wd all' or '/wd exit'.");
                return;
            }

            // Default behavior: check if the client is in the Water Dungeon
            if (client._map.Name.Contains("Water Dungeon"))
            {
                client.PublicMessage(1, "Water Spirit, I have done what you have asked of me.");
            }
            else
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "You are not in the Water Dungeon.");
            }
        }

        private void CommandHandler_StrUp(Client client, string fullMessage, string[] args)
        {
            // Validate input
            if (args.Length != 1 || !ushort.TryParse(args[0], out ushort pointsToAllocate))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Usage: /strup <number_of_points>");
                return;
            }

            int unspentPoints = client.UnspentPoints;

            // Check if the client has enough unspent points
            if (pointsToAllocate > unspentPoints)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage,
                    $"Insufficient unspent points. You have {unspentPoints} unspent points.");
                return;
            }

            // Allocate points
            for (int i = 0; i < pointsToAllocate; i++)
            {
                client.RaiseStrStat();
                Thread.Sleep(5);
            }

            // Notify the client
            client.ServerMessage((byte)ServerMessageType.ActiveMessage,
                $"Successfully raised Strength by {pointsToAllocate} points.");
        }


        private void CommandHandler_SapphireStream(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_ShinewoodHome(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_Loures(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_Canals(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_BlackMarket(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_GladArena(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_FC(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_PlamBoss(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_Clear(Client client, string fullMessage, string[] args)
        {
            if (args.Length > 0 && args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Client otherClient in client._server.Clients)
                {
                    if (otherClient.ClientTab != null)
                    {
                        otherClient.ClientTab.ClearOptions();
                    }
                }

                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Cleared profile for all clients.");
            }
            else
            {
                if (client.ClientTab != null)
                {
                    client.ClientTab.ClearOptions();
                    client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Cleared profile for this client.");
                }
            }
        }


        private void CommandHandler_Walk(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_Chests(Client client, string fullMessage, string[] args)
        {
            client._chestToggle = !client._chestToggle;

            string stateMessage = client._chestToggle ? "Chest opening is now toggled on." : "Chest opening is now toggled off.";
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, stateMessage);
        }

        private void CommandHandler_Raffle(Client client, string fullMessage, string[] args)
        {
            client._raffleToggle = !client._raffleToggle;

            string stateMessage = client._raffleToggle ? "Raffle opening is now toggled on." : "Raffle opening is now toggled off.";
            client.ServerMessage((byte)ServerMessageType.ActiveMessage, stateMessage);
        }

        private void CommandHandler_AscendState(Client client, string fullMessage, string[] args)
        {
            throw new NotImplementedException();
        }

        private void CommandHandler_LoadProfile(Client client, string fullMessage, string[] args)
        {
            // Check if profile name is provided
            if (args.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Please specify a profile to load.");
                return;
            }

            string profileName = args[0];

            // Handle "all" argument to load profile for all clients
            if (args.Length > 1 && args[1].Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var otherClient in client._server.Clients)
                {
                    _ = otherClient.ClientTab.LoadProfileAsync(profileName);
                    otherClient.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Loading profile '{profileName}' for all clients.");
                }
                return;
            }
            else
            {
                // Load profile for the current client
                _ = client.ClientTab.LoadProfileAsync(profileName);
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Loading profile '{profileName}' for this client.");
            }

        }


        private void AddCommand(string command, CommandHandler handler)
        {
            if (!_commands.ContainsKey(command))
            {
                _commands[command] = handler;
            }
            else
            {
                throw new ArgumentException($"Command '{command}' is already registered.");
            }
        }

        internal void ExecuteCommand(Client client, string message)
        {

            if (client.ClientTab.safeScreenCbox.Checked)
            {
                return;
            }

            // Split the command into parts
            string[] parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, "Invalid command. Please provide a command.");
                return;
            }

            // Extract the command and arguments
            string command = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            // Check if the command exists
            if (!_commands.TryGetValue(command, out var handler))
            {
                client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"Unknown command: {command}");
                return;
            }

            // Run the command asynchronously using Task.Run
            Task.Run(() =>
            {
                try
                {
                    handler(client, message, args);
                }
                catch (Exception ex)
                {
                    client.ServerMessage((byte)ServerMessageType.ActiveMessage, $"An error occurred while executing the command: {ex.Message}");
                }
            });
        }

    }
}
