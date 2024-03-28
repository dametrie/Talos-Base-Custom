using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Structs;

namespace Talos.Base
{
    internal delegate void TaskDelegate();
    internal abstract class BotBase
    {
        internal Server Server { get; set; }
        internal Client Client { get; set; }
        internal List<Thread> BotThreadList { get; set; }
        internal List<Location> LocationList { get; set; }
        internal List<TaskDelegate> TaskList { get; set; }

        internal BotBase()
        {
            BotThreadList = new List<Thread>();
            LocationList = new List<Location>();
            TaskList = new List<TaskDelegate>();
        }

        internal BotBase(Client client, Server server) : this()
        {
            Client = client;
            Server = server;
        }
        internal void Start()
        {
            BotThreadList.Clear();
            foreach (var task in TaskList)
            {
                TaskDelegate taskDelegate = task;
                Thread thread = new Thread(() => taskDelegate());
                BotThreadList.Add(thread);
                thread.Start();
            }
        }

        internal void Stop()
        {
            foreach (var thread in BotThreadList)
            {
                thread.Abort();
            }
            BotThreadList.Clear();

        }

        internal void AddTask(TaskDelegate task)
        {
            TaskList.Add(task);
        }

        internal void RemoveTask(TaskDelegate task) 
        { 
            TaskList.Remove(task);
        }

    }

}
