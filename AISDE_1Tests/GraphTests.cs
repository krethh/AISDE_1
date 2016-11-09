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
    public class GraphTests
    {

        [TestMethod()]
        public void FloydTest()
        {
            Graph testGraph = Graph.ReadGraph("C:\\Users\\Paweł Kulig\\Desktop\\test_graf_pełny.txt");
            for (int i = 0; i < 50000; i++)
            {
                Random random = new Random();
                foreach (var v in testGraph.Vertices)
                    foreach (var n in v.GetNeighbors())
                        v.GetEdge(n).Cost = random.Next(58) + 1;

                testGraph.Floyd();

                int index1 = random.Next(testGraph.Vertices.Count);
                int index2 = random.Next(testGraph.Vertices.Count);

                try
                {
                    Assert.AreEqual(testGraph.FloydPaths[new Tuple<GraphVertex, GraphVertex>(testGraph.Vertices[index1],
                        testGraph.Vertices[index2])], testGraph.Dijkstra(testGraph.Vertices[index1],
                        testGraph.Vertices[index2]));
                }
                catch(Exception)
                {
                    var path1 = testGraph.FloydPaths[new Tuple<GraphVertex, GraphVertex>(testGraph.Vertices[index1],
                        testGraph.Vertices[index2])];
                    var path2 = testGraph.Dijkstra(testGraph.Vertices[index1],
                        testGraph.Vertices[index2]);

                    Assert.AreEqual(path1.TotalCost, path2.TotalCost);
                }
            }
        }

        [TestMethod()]  
        public void DijkstraTest()
        {
            Graph testGraph = new Graph();

            for (int i = 0; i < 6; i++)
                testGraph.AddVertex(new GraphVertex());

            testGraph.AddEdge(testGraph.Vertices[0], testGraph.Vertices[1], 4);
            testGraph.AddEdge(testGraph.Vertices[0], testGraph.Vertices[3], 1);
            testGraph.AddEdge(testGraph.Vertices[3], testGraph.Vertices[2], 3);
            testGraph.AddEdge(testGraph.Vertices[1], testGraph.Vertices[2], 2);
            testGraph.AddEdge(testGraph.Vertices[2], testGraph.Vertices[1], 2);
            testGraph.AddEdge(testGraph.Vertices[4], testGraph.Vertices[2], 3);
            testGraph.AddEdge(testGraph.Vertices[1], testGraph.Vertices[4], 2);
            testGraph.AddEdge(testGraph.Vertices[4], testGraph.Vertices[5], 5);

            GraphPath onefour = new GraphPath { testGraph.Vertices[1], testGraph.Vertices[4] };
            GraphPath zerofour = new GraphPath { testGraph.Vertices[0], testGraph.Vertices[1], testGraph.Vertices[4] };
            GraphPath threeone = new GraphPath { testGraph.Vertices[3], testGraph.Vertices[2], testGraph.Vertices[1] };
            GraphPath fourzero = new GraphPath { TotalCost = Double.PositiveInfinity };
            GraphPath zerozero = new GraphPath { TotalCost = 0 };

            Assert.AreEqual(testGraph.Dijkstra(testGraph.Vertices[1], testGraph.Vertices[4]), onefour);
            Assert.AreEqual(testGraph.Dijkstra(testGraph.Vertices[0], testGraph.Vertices[4]), zerofour);
            Assert.AreEqual(testGraph.Dijkstra(testGraph.Vertices[3], testGraph.Vertices[1]), threeone);
            Assert.AreEqual(testGraph.Dijkstra(testGraph.Vertices[4], testGraph.Vertices[0]), fourzero);
            Assert.AreEqual(testGraph.Dijkstra(testGraph.Vertices[0], testGraph.Vertices[0]), zerozero);


        }

    }
}