using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Talos.Structs;

namespace Talos.Base
{

    internal abstract class BotBase
    {
        internal Server Server { get; set; }
        internal Client Client { get; set; }

        protected CancellationTokenSource _cts;

        public CancellationToken CancellationToken => _cts.Token;

        protected List<Task> _tasks;

        private readonly List<Func<CancellationToken, Task>> _botLoops = new List<Func<CancellationToken, Task>>();


        internal BotBase()
        {
            _cts = new CancellationTokenSource();
            _tasks = new List<Task>();
        }

        internal BotBase(Client client, Server server) : this()
        {
            Client = client;
            Server = server;
        }

        protected void AddTask(Func<CancellationToken, Task> loopMethod)
        {
            _botLoops.Add(loopMethod);
        }


        public virtual void Start()
        {
            // If already running, optionally stop first (or simply return).
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                // Optionally call StopAsync() here to support restart.
                // For now, assume we want to restart:
                StopAsync().Wait();
            }

            // Reinitialize the cancellation token and tasks list.
            _cts = new CancellationTokenSource();
            _tasks = new List<Task>();

            // Start each loop as a Task.
            foreach (var loop in _botLoops)
            {
                _tasks.Add(Task.Run(() => loop(_cts.Token), _cts.Token));
            }
        }

        public virtual async Task StopAsync()
        {
            // Signal all loops to stop.
            _cts.Cancel();

            try
            {
                // Wait for all tasks to complete.
                await Task.WhenAll(_tasks);
            }
            catch (OperationCanceledException)
            {
                // This is expected when cancellation occurs.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BotBase] Exception during stop: {ex}");
            }
        }


    }

}
