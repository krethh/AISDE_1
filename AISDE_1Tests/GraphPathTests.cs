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
    public class GraphPathTests
    {
        [TestMethod()]
        public void TotalCostTest()
        {
            Graph graph = new Graph();
            for (int i = 0; i < 3; i++)
                graph.AddVertex(new GraphVertex());

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1], 2);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2], 3);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2], 5);

            GraphPath zeroone = new GraphPath { graph.Vertices[0], graph.Vertices[1] };
            GraphPath onetwo = new GraphPath { graph.Vertices[1], graph.Vertices[2] };
            GraphPath zerotwo = new GraphPath { graph.Vertices[0], graph.Vertices[1], graph.Vertices[2] };

            Assert.AreEqual(zeroone.TotalCost, 2);
            Assert.AreEqual(onetwo.TotalCost, 5);
            Assert.AreEqual(zerotwo.TotalCost, 7);

        }
    }
}