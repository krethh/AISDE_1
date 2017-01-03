using System;
using System.Collections.Generic;
using System.Windows;

namespace AISDE_1
{
    public class GraphVertex : IComparable<GraphVertex>
    {
        private double _distance;

        /// <summary>
        /// Haszmapa zawierająca sąsiadów danego wierzchołka jako klucze oraz ścieżki łączące
        /// ten wierzchołek z sąsiadami jako wartościl
        /// </summary>
        public Dictionary<GraphVertex, Edge> Neighbors { get; }

        /// <summary>
        /// Dystans od startu - etykieta w algorytmie Dijkstry.
        /// </summary>
        public double DistanceFromStart
        {
            get { return _distance; }
            set { _distance = value;
                OnDistanceChanged(new EventArgs()); }
        }

        /// <summary>
        /// Mówi, czy dany wierzchołek jest centralą w sieci dostępowej.
        /// </summary>
        public bool IsCentral { get; set; } = false;

        /// <summary>
        /// Mówi, ile klientów jest w węźle sieci dostępowej.
        /// </summary>
        public int ClientsNumber { get; set; } = 0;

        /// <summary>
        /// Położenie danego wierzchołka na mapie.
        /// </summary>
        public Point Coordinates { get; set; }

        /// <summary>
        /// ID wierzchołka w danym grafie.
        /// </summary>
        public int ID { get; set; }

        public static EventHandler<EventArgs> CostToVertexChanged;

        public GraphVertex()
        {
            Neighbors = new Dictionary<GraphVertex, Edge>();
            DistanceFromStart = double.PositiveInfinity;
            Coordinates = new Point();
        }

        /// <summary>
        /// Dodaje krawędź do danego wierzchołka. Nie powinna być wykorzystywana bez potrzeby, zamiast tego należy użyć Graph.AddEdge
        /// </summary>
        public bool AddEdge(GraphVertex end2, double cost)
        {
            Edge newEdge = new Edge(this, end2, cost);
            if (Neighbors.ContainsKey(end2))
                return false;
            Neighbors.Add(end2, newEdge);
            return true;
        }

        /// <summary>
        /// Zwraca wszystkich sąsiadów danego wierzchołka.
        /// </summary>
        public HashSet<GraphVertex> GetNeighbors()
        {
            return new HashSet<GraphVertex>(Neighbors.Keys);
        }

        /// <summary>
        /// Zwraca krawędź łączącą ten wierzchołek z podanym.
        /// </summary>
        public Edge GetEdge(GraphVertex vertex)
        {
            return Neighbors[vertex];
        }

        /// <summary>
        /// Zwraca ilość krawędzi wychodzących z wierzchołka.
        /// </summary>
        public int GetNumOutgoingEdges()
        {
            return Neighbors.Count;
        }

        /// <summary>
        /// Zwraca true jeżeli dany wierzchołek ma krawędź łączącą z podanym wierzchołkiem.
        /// </summary>
        public bool HasEdgeTo(GraphVertex end) => (Neighbors.ContainsKey(end));

        /// <summary>
        /// Zwraca koszt dojścia do danego wierzchołka.
        /// </summary>
        public double CostToVertex(GraphVertex end2)
        {
            if (end2 == this)
                return 0;
            if (!Neighbors.ContainsKey(end2))
                return Double.PositiveInfinity;
            else return Neighbors[end2].DiggingCost;
        }

        public int CompareTo(GraphVertex other)
        {
            if (DistanceFromStart > other.DistanceFromStart)
                return 1;
            return -1;
        }

        public override string ToString()
        {
            string toReturn = "";
            toReturn += ID;
            toReturn += " ";
            //foreach (var n in neighbors.Keys)
            //    toReturn += Label + ", ";

            return toReturn;
        }

        private void OnDistanceChanged(EventArgs e)
        {
            CostToVertexChanged?.Invoke(this, e);
        }

        public double DistanceToVertex(GraphVertex other)
        {
            return Math.Sqrt(Math.Pow(Coordinates.X - other.Coordinates.X, 2) + Math.Pow(Coordinates.Y - other.Coordinates.Y, 2));
        }
    }
}
