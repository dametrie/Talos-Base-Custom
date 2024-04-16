using System;
using System.Collections.Generic;
using System.Linq;

namespace Talos.AStar
{

    enum QueuePriorityEnum
    {
        Low = 0,
        High = 1
    }

    class PriorityQueue<T>
    {
        private Queue<T>[] _queues;

        // Constructor that allows specifying the number of priority levels dynamically
        public PriorityQueue(int levels = -1)
        {
            if (levels == -1)
            {
                levels = Enum.GetValues(typeof(QueuePriorityEnum)).Length;
            }
            _queues = new Queue<T>[levels];
            for (int i = 0; i < levels; i++)
            {
                _queues[i] = new Queue<T>();
            }
        }

        public void Enqueue(QueuePriorityEnum priority, T item)
        {
            _queues[(int)priority].Enqueue(item);
        }

        // A method to dequeue with a fail-safe mechanism
        public bool TryDequeue(out T item)
        {
            int levels = _queues.Length;
            for (int i = levels - 1; i >= 0; i--)
            {
                if (_queues[i].Count > 0)
                {
                    item = _queues[i].Dequeue();
                    return true;
                }
            }
            item = default;
            return false;
        }

        // Method to safely peek at the next item
        public T Peek()
        {
            int levels = _queues.Length;
            for (int i = levels - 1; i >= 0; i--)
            {
                if (_queues[i].Count > 0)
                {
                    return _queues[i].Peek();
                }
            }
            throw new InvalidOperationException("The Queue is empty.");
        }

        // A property to get the total count of items in the queue
        public int Count
        {
            get
            {
                return _queues.Sum(q => q.Count);
            }
        }

        // Standard Dequeue method, using TryDequeue to handle empty queues
        public T Dequeue()
        {
            if (TryDequeue(out T item))
            {
                return item;
            }
            throw new InvalidOperationException("The Queue is empty.");
        }

        public bool TryRemove(QueuePriorityEnum priority, T item)
        {
            Queue<T> queue = _queues[(int)priority];
            if (queue.Contains(item))
            {
                queue = new Queue<T>(queue.Where(x => !EqualityComparer<T>.Default.Equals(x, item)));
                return true;
            }
            return false;
        }
    }
}
