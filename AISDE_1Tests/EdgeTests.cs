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
    public class EdgeTests
    {
        [TestMethod()]
        public void JoinCablesTest()
        {
            GraphVertex v1 = new GraphVertex();
            GraphVertex v2 = new GraphVertex();
            Edge e = new Edge(v1, v2, 0);

            e.AddCable(0);
            e.AddCable(0);
            e.AddCable(0);
            e.AddCable(0);

            e.JoinCables();
            Assert.AreEqual(e.GetCables()[0], 3);

            e.AddCable(0);
            e.AddCable(0);
            e.AddCable(0);
            e.JoinCables();

            Assert.AreEqual(e.GetCables()[0], 3);
            Assert.AreEqual(e.GetCables()[1], 2);

            e.AddCable(0);
            e.JoinCables();

            Assert.AreEqual(e.GetCables()[0], 4);

            e.AddCable(0);
            e.JoinCables();

            Assert.AreEqual(e.GetCables()[0], 4);
            Assert.AreEqual(e.GetCables()[1], 0);

            e.AddCable(0);
            e.AddCable(0);

            e.JoinCables();

            Assert.AreEqual(e.GetCables()[0], 4); //22
            Assert.AreEqual(e.GetCables()[1], 2);

            e.AddCable(3);
            e.JoinCables();

            Assert.AreEqual(e.GetCables()[0], 5); //30
            Assert.AreEqual(e.GetCables()[1], 2);
        }

    }
}