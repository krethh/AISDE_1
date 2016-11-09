using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1
{
    /// <summary>
    /// Kolejka priorytetowa implementująca stos.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueueHeap<T> where T : IComparable<T>
    {
        protected T[] elements { get; set; }
        public int Count { get; set; }
        private const int DEFAULT_COUNT = 20;

        public PriorityQueueHeap()
        {
            elements = new T[DEFAULT_COUNT];
            Count = 0;
        }

        public T Peek()
        {
            if (Count == 0) throw new Exception("Heap empty.");
            else return elements[1];
        }
 
        public T Dequeue()
        {
            T element = Peek();

            elements[1] = elements[Count];
            elements[Count] = default(T);
            Count--;
            PushDown();

            return element;

        }

        public void Enqueue(T t)
        {
            if (Count >= elements.Length - 1)
                IncreaseSize();

            Count++;
            elements[Count] = t;
            PushUp();
        }

        private void IncreaseSize()
        {
            T[] newArray = new T[Count * 2];
            for (int i = 1; i <= Count; i++)
            {
                newArray[i] = elements[i];
            }
            elements = newArray;
        }

        private void PushDown()
        {
            int index = 1;

            while (index * 2 <= Count) // dopóki sprawdzany element ma syna z lewej strony
            {
                int smaller = index * 2; // zakładamy że mniejszy syn jest z lewej strony
                if(index * 2 + 1 <= Count && elements[index *2 ].CompareTo(elements[index*2 + 1 ]) > 0)
                    smaller = index * 2 + 1; // jeżeli jest to nieprawda, to zmieniamy założenie xDD

                if (elements[index].CompareTo(elements[smaller]) > 0)
                    Swap(index, smaller);
                else break; // jeżeli nie jest mniejszy, to koniec pętli]

                index = smaller; // jeżeli nie przerwaliśmy pętli, to szukamy dalej
            }
        }

        private void PushUp()
        {
            int index = Count;           
            while(index > 1 && elements[index/2].CompareTo(elements[index]) > 0 )
            {
                Swap(index, index / 2);
                index = index / 2;
            }
           
        }

        private void Swap(int i1, int i2)
        {
            T tmp = elements[i1];
            elements[i1] = elements[i2];
            elements[i2] = tmp;
        }



    }
}
