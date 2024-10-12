using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Structs;

namespace Talos.Base
{
    internal delegate void BotLoop();
    internal abstract class BotBase
    {
        internal Server Server { get; set; }
        internal Client Client { get; set; }
        internal List<Thread> BotThreads { get; set; }
        internal List<Location> Waypoints { get; set; }
        internal List<BotLoop> BotLoops { get; set; }

        internal volatile bool _shouldThreadStop = false;


        internal BotBase()
        {
            BotThreads = new List<Thread>();
            Waypoints = new List<Location>();
            BotLoops = new List<BotLoop>();
        }

        internal BotBase(Client client, Server server) : this()
        {
            Client = client;
            Server = server;
        }
        internal void Start()
        {
            _shouldThreadStop = false;
            BotThreads.Clear();
            foreach (var botLoop in BotLoops)
            {
                BotLoop taskDelegate = botLoop;
                Thread thread = new Thread(() => taskDelegate());
                BotThreads.Add(thread);
                thread.Start();
            }
        }

        internal void Stop()
        {
            _shouldThreadStop = true;
            foreach (var thread in BotThreads)
            {
                thread.Join(); //previously using Thread.Abort() which is not recommended... but maybe it would stop the bot faster?
            }
            BotThreads.Clear();

        }

        internal void AddTask(BotLoop task)
        {
            BotLoops.Add(task);
        }

        internal void RemoveTask(BotLoop task) 
        { 
            BotLoops.Remove(task);
        }

    }

}
