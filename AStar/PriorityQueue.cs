using System;
using System.Collections.Generic;

namespace Talos.AStar
{
    class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _heap;

        public PriorityQueue()
        {
            _heap = new List<T>();
        }

        public void Enqueue(T item)
        {
            _heap.Add(item);
            HeapifyUp(_heap.Count - 1);
        }

        public T Dequeue()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("The queue is empty.");

            T root = _heap[0];
            T lastItem = _heap[_heap.Count - 1];
            _heap[0] = lastItem;
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return root;
        }

        public int Count => _heap.Count;

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_heap[index].CompareTo(_heap[parentIndex]) >= 0)
                    break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (true)
            {
                int leftChild = index * 2 + 1;
                int rightChild = index * 2 + 2;
                int smallest = index;

                if (leftChild <= lastIndex && _heap[leftChild].CompareTo(_heap[smallest]) < 0)
                    smallest = leftChild;

                if (rightChild <= lastIndex && _heap[rightChild].CompareTo(_heap[smallest]) < 0)
                    smallest = rightChild;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }

        public bool Contains(T item)
        {
            return _heap.Contains(item);
        }

        // Optional: Implement a method to update an item's priority
        public void UpdatePriority(T item)
        {
            int index = _heap.IndexOf(item);
            if (index == -1)
                return;

            HeapifyUp(index);
            HeapifyDown(index);
        }
    }
}
