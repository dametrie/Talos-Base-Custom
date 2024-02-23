using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Forms.UI;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
        private static object _lockObject { get; set; } = new object();
        private int _dropCounter;

        internal Client _client;
        internal Server _server;

        internal bool _needFasSpiorad = true;
        internal bool _manaLessThanEightyPct = true;
        internal bool _shouldBotStop = false;
        internal byte _fowlCount;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;

        internal DateTime _lastKill = DateTime.MinValue;
        internal DateTime _lastDisenchanterCast;
        internal DateTime _lastGrimeScentCast;

        internal List<Ally> _allyList = new List<Ally>();
        internal HashSet<string> _allyListName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);


        internal Bot(Client client, Server server) : base(client, server) 
        {
            base.AddTask(new TaskDelegate(BotLoop));
        }

        private void BotLoop()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("BotLoop running");
                    this.Loot();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in BotLoop: {ex.Message}");
                }
                Thread.Sleep(1000); //Adam check this delay
            }
        }

        internal bool IsAllyAlreadyListed(string name)
        {
            lock (Bot._lockObject)
            {
                return _allyListName.Contains(name, StringComparer.CurrentCultureIgnoreCase);
            }
        }

        internal void UpdateAllyList(Ally ally)
        {
            lock (Bot._lockObject)
            {
                this._allyList.Add(ally);
                this._allyListName.Add(ally.Name);
            }
        }

        internal bool IsStrangerNearby()
        {
            return false;
        }

        private void Loot()
        {
            Console.WriteLine("Loot method running");
            if (!Client.ClientTab.pickupGoldCbox.Checked && !Client.ClientTab.pickupItemsCbox.Checked && !Client.ClientTab.dropTrashCbox.Checked)
            {
                return;
            }

            try
            {
                if (!IsStrangerNearby())
                {
                    var lootArea = new Structs.Rectangle(new Point(Client._serverLocation.X - 2, Client._serverLocation.Y - 2), new Point(5, 5));
                    List<Objects.Object> nearbyObjects = Client.GetNearbyLootableObjects(4);

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

        private void ProcessLoot(List<Objects.Object> nearbyObjects, Structs.Rectangle lootArea)
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

        private bool IsGold(Objects.Object obj, Structs.Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.Sprite == 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private bool IsLootableItem(Objects.Object obj, Structs.Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.Sprite != 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
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

    }
}
