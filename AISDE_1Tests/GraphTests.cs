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

        [TestMethod()]
        public void SimulatedAnnealingTestCaseGeneration()
        {
            var nodes = 300;

            Random random = new Random();
            List<string> lines = new List<string>();

            for (int i = 0; i < nodes; i++)
            {
                var first = random.Next(800) + 1;
                var second = random.Next(800) + 1;
                var clients = random.Next(25);

                string str = first.ToString() + " " + second.ToString();
                if (!lines.Contains(str))
                    lines.Add(str + " " + clients.ToString());
                else
                {
                    i--;
                    continue;
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = (i + 1).ToString() + " " + lines[i];
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\Paweł Kulig\\Desktop\\Studia\\AISDE\\AISDE_1\\AISDE_1\\test.txt");
            file.WriteLine("WEZLY");
            lines[0] = "1 10 50 -1";
            lines.ForEach(l => file.WriteLine(l));
            file.WriteLine("KRAWEDZIE");
            lines = new List<string>();

            for (int i = 0; i < nodes * 5; i++)
            {
                var first = random.Next(nodes - 1) + 1;
                var second = random.Next(nodes - 1) + 1;

                if (first == second)
                    while (first == second)
                    {
                        first = random.Next(nodes -1) + 1;
                        second = random.Next(nodes - 1) + 1;
                    }

                string str = first.ToString() + " " + second.ToString();
                string str2 = second.ToString() + " " + first.ToString();

                if (!lines.Contains(str) && !lines.Contains(str2))
                    lines.Add(str);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = (i + 1).ToString() + " " + lines[i] + " " + (random.Next(9) + 1) * 10;
            }
           


            lines.ForEach(l => file.WriteLine(l));

            file.WriteLine("KABLE");
            file.WriteLine("1 1 3");
            file.WriteLine("2 2 5");
            file.WriteLine("3 4 8");
            file.WriteLine("4 10 18");
            file.WriteLine("5 20 35");
            file.WriteLine("6 50 70");

            file.Close();
        }
    }
}