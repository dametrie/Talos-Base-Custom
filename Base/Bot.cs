using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Forms.UI;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
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
        private static object LockObject { get; set; } = new object();



        internal Bot(Client client, Server server) : base(client, server) 
        { 
            
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
    }
}
