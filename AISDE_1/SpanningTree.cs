using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1
{
    /// <summary>
    /// Klasa drzewa rozpinającego. Różni się od grafu tym, że explicité podane są krawędzie należące do drzewa - w przeciwieństwie
    /// do grafu, w którym zbiór krawędzi to suma zbiorów sąsiadów wierzchołków tego grafu. Pomaga to w łatwym wyświetlaniu drzewa rozpinającego.
    /// </summary>
    public class SpanningTree : Graph
    {
        public List<Edge> Edges;

        public SpanningTree()
        {
            Edges = new List<Edge>();
        }

        /// <summary>
        /// Zwraca ilość krawędzi które zawiera drzewo.
        /// </summary>
        public new int GetNumEdges()
        {
            return Edges.Count();
        }
    }
}
