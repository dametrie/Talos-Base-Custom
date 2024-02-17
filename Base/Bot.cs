using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
        internal bool _needFasSpiorad = true;
        internal bool _manaLessThanEightyPct = true;
        internal bool _shouldBotStop = false;
        internal byte _fowlCount;
        internal DateTime _lastKill = DateTime.MinValue;
        internal DateTime _lastDisenchanterCast;
        internal DateTime _lastGrimeScentCast;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;

        internal Bot(Client client, Server server) : base(client, server) 
        { 
            
        }
    }
}
