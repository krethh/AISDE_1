using System;
using System.Drawing;

namespace AISDE_1
{/// <summary>
/// Reprezentuje krawędź grafu.
/// </summary>
    public class Edge : IComparable<Edge>
    {
        /// <summary>
        /// Wierzchołek początkowy krawędzi.
        /// </summary>
        public GraphVertex End1 { get; set; } 

        /// <summary>
        /// Wierzchołek końcowy krawędzi.
        /// </summary>
        public GraphVertex End2 { get; set; }

        /// <summary>
        /// Waga krawędzi.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Kolor krawędzi wyświetlany na grafie.
        /// </summary>
        public System.Windows.Media.Brush Color { get; set; }

        public Edge(GraphVertex end1, GraphVertex end2, double cost)
        {
            this.End1 = end1;
            this.End2 = end2;
            this.Cost = cost;
        }

        // porównujemy krawędzie za pomocą ich wag
        public int CompareTo(Edge other)
        {
            if (Cost <= other.Cost) return -1;
            else return 1;
        }

        /// <summary>
        /// Zwraca true jeżeli krawędź jest nieskierowana (tzn. istnieje analogiczna krawędź w drugą stronę, niekoniecznie o tej samej wadze).
        /// </summary>
        public bool IsUndirected() => (End2.HasEdgeTo(End1));

        public override string ToString()
        {
            return End1.ID + " => " + End2.ID;
        }
    }
}
