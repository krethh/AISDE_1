using Microsoft.VisualStudio.TestTools.UnitTesting;
using AISDE_1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1.Tests
{
    [TestClass()]
    public class PriorityQueueTests
    {
        [TestMethod()]
        public void EnqueueTest()
        {
            PriorityQueueHeap<int> testQueue = new PriorityQueueHeap<int>();

            testQueue.Enqueue(1);
            testQueue.Enqueue(4);
            testQueue.Enqueue(7);
            testQueue.Enqueue(0);
            testQueue.Enqueue(3);

            Assert.AreEqual(testQueue.Dequeue(), 0);
            Assert.AreEqual(testQueue.Dequeue(), 1);
            Assert.AreEqual(testQueue.Dequeue(), 3);
            Assert.AreEqual(testQueue.Dequeue(), 4);
            Assert.AreEqual(testQueue.Dequeue(), 7);

        }

        [TestMethod()]
        public void DequeueTest()
        {
            PriorityQueueHeap<int> testQueue = new PriorityQueueHeap<int>();
            testQueue.Enqueue(1);
            testQueue.Enqueue(3);
            testQueue.Enqueue(2);

            Assert.AreEqual(testQueue.Dequeue(), 1);
            Assert.AreEqual(testQueue.Dequeue(), 2);
            Assert.AreEqual(testQueue.Dequeue(), 3);

            Assert.AreEqual(testQueue.Count, 0);

            testQueue.Enqueue(0);
            testQueue.Enqueue(2);
            testQueue.Enqueue(4);
            testQueue.Enqueue(0);
            testQueue.Enqueue(5);
            testQueue.Enqueue(1);

            Assert.AreEqual(testQueue.Dequeue(), 0);
            Assert.AreEqual(testQueue.Dequeue(), 0);
            Assert.AreEqual(testQueue.Dequeue(), 1);
            Assert.AreEqual(testQueue.Dequeue(), 2);
            Assert.AreEqual(testQueue.Dequeue(), 4);
            Assert.AreEqual(testQueue.Dequeue(), 5);


        }

        [TestMethod()]
        public void CountTest()
        {
            PriorityQueueHeap<int> testQueue = new PriorityQueueHeap<int>();
            Assert.AreEqual(testQueue.Count, 0);

            testQueue.Enqueue(1);
            Assert.AreEqual(testQueue.Count, 1);

            int dequeued = testQueue.Dequeue();
            Assert.AreEqual(dequeued, 1);
            Assert.AreEqual(testQueue.Count, 0);        
        }

        [TestMethod()]
        public void PeekTest()
        {
            PriorityQueueHeap<int> testQueue = new PriorityQueueHeap<int>();

            testQueue.Enqueue(1);
            Assert.AreEqual(testQueue.Peek(), 1);
            testQueue.Enqueue(3);
            Assert.AreEqual(testQueue.Peek(), 1);
            testQueue.Dequeue();
            testQueue.Enqueue(0);
            Assert.AreEqual(testQueue.Peek(), 0); 
        }
    }
}