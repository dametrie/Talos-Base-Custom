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
        internal List<Thread> BotThreadList { get; set; }
        internal List<Location> LocationList { get; set; }
        internal List<BotLoop> TaskList { get; set; }
        internal volatile bool _shouldThreadStop = false;

        internal BotBase()
        {
            BotThreadList = new List<Thread>();
            LocationList = new List<Location>();
            TaskList = new List<BotLoop>();
        }

        internal BotBase(Client client, Server server) : this()
        {
            Client = client;
            Server = server;
        }
        internal void Start()
        {
            _shouldThreadStop = false;
            BotThreadList.Clear();
            foreach (var task in TaskList)
            {
                BotLoop taskDelegate = task;
                Thread thread = new Thread(() => taskDelegate());
                BotThreadList.Add(thread);
                thread.Start();
            }
        }

        internal void Stop()
        {
            _shouldThreadStop = true;
            foreach (var thread in BotThreadList)
            {
                thread.Join(); //previously using Thread.Abort() which is not recommended... but maybe it would stop the bot faster?
            }
            BotThreadList.Clear();

        }

        internal void AddTask(BotLoop task)
        {
            TaskList.Add(task);
        }

        internal void RemoveTask(BotLoop task) 
        { 
            TaskList.Remove(task);
        }

    }

}
