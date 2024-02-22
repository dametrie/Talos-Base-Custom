using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Forms.UI;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Base
{
    internal class Bot : BotBase
    {

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
        private int dropCounter;

        private static object LockObject { get; set; } = new object();



        internal Bot(Client client, Server server) : base(client, server) 
        {
            base.AddTask(new TaskDelegate(BotLoop));
        }

        private void BotLoop()
        {
            this.Loot();
        }

        internal bool IsAllyAlreadyListed(string name)
        {
            lock (Bot.LockObject)
            {
                return _allyListName.Contains(name, StringComparer.CurrentCultureIgnoreCase);
            }
        }

        internal void UpdateAllyList(Ally ally)
        {
            lock (Bot.LockObject)
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
                        //make sure objects are not of type Creature or Player
                        ProcessLoot(nearbyObjects, lootArea);
                    }

                    HandleTrashItems();
                }
            }
            catch
            {
                Console.WriteLine("Error in Loot()");
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
                if (dropCounter >= 15)
                {
                    foreach (Item item in Client.Inventory.ToList())
                    {
                        if (Client.ClientTab.trashToDrop.Contains(item.Name, StringComparer.CurrentCultureIgnoreCase))
                        {
                            Client.Drop(item.Slot, Client._serverLocation, item.Quantity);
                        }
                    }
                    dropCounter = 0;
                }
                else
                {
                    dropCounter++;
                }
            }
        }

    }
}
