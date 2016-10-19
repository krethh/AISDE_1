using System;
using System.Collections.Generic;

namespace AISDE_1
{
    /// <summary>
    /// Prymitywnie zaimplementowana kolejka priorytetowa, używana w algorytmie Dijkstry i MST.
    /// </summary>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public bool IsEmpty() => (data.Count == 0);

        /// <summary>
        /// Dodaje element do kolejki.
        /// </summary>
        public bool Enqueue(T toAdd)
        {
            data.Add(toAdd);
            return true;
        }

        public T Dequeue()
        {
            T smallest = Peek();

            data.Remove(smallest);
            return smallest;
        }

        public T Peek()
        {
            T smallest = data[0];
            foreach (T obj in data)
            {
                if (obj.CompareTo(smallest) < 0)
                    smallest = obj;
            }

            return smallest;
        }

        public int Count()
        {
            return data.Count;
        }

    }
}
