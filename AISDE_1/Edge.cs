using System;
using System.Collections.Generic;
using System.Drawing;

namespace AISDE_1
{/// <summary>
/// Reprezentuje krawędź grafu.
/// </summary>
    public class Edge : IComparable<Edge>
    {
        private double _cost;

        public static EventHandler<EdgeChangedEventArgs> CostChanged;

        public static int[] CableCosts { get; set; } = { 4, 7, 10, 12, 20, 30, 35, 60 };
        public static int[] CableCounts { get; set; } = { 2, 4, 6, 8, 16, 24, 32, 64 };

        /// <summary>
        /// Lista kabli, które są położone na danej krawędzi - kabel jest reprezentowany przez indeks w CableCosts i CableCounts, tzn. dodanie "0" do listy odpowiada dodaniu kabla o dwóch włóknach i koszcie 4.
        /// </summary>
        private List<int> Cables { get; set; }

        public List<int> GetCables()
        {
            return new List<int>(Cables);
        }

        public void AddCable(int index)
        {
            if (index > CableCosts.Length - 1 || index < 0)
                throw new Exception("Zbyt duży lub zbyt mały index kabla");
            Cables.Add(index);
        }

        /// <summary>
        /// Wierzchołek początkowy krawędzi.
        /// </summary>
        public GraphVertex End1 { get; set; }

        /// <summary>
        /// Wierzchołek końcowy krawędzi.
        /// </summary>
        public GraphVertex End2 { get; set; }

        /// <summary>
        /// Koszt rozkopania krawędzi + koszt wszystkich kabli położonych na krawędzi.
        /// </summary>
        public double TotalCost()
        {
            double sum = 0;
            Cables.ForEach(c => sum += CableCosts[c]);
            return sum +DiggingCost;
        }

        /// <summary>
        /// Liczba włókien położonych na danej krawędzi.
        /// </summary>
        public int WireCount()
        {
            int count = 0;
            Cables.ForEach(c => count += CableCounts[c]);
            return count;
        }

        /// <summary>
        /// Waga krawędzi. Przy zmianie rzuca zdarzenie informujące graf o tym, że trzeba znowu przeliczyć tabelę najkrótszych ścieżek.
        /// </summary>
        public double DiggingCost {
            get { return _cost; }
            set
            {
                _cost = value;
                OnCostChanged(new EdgeChangedEventArgs(this)); // powiadamia graf o tym, że zmieniono wartość jakiejś krawędzi
            }
        }

        /// <summary>
        /// Kolor krawędzi wyświetlany na grafie.
        /// </summary>
        public System.Windows.Media.Brush Color { get; set; }

        /// <summary>
        /// Optymalizuje rodzaje kabli położone na danej krawędzi - jeżeli można dwa mniejsze kable połączyć w większy, to to robi.
        /// </summary>
        public void JoinCables()
        {
            List<int> newCables = new List<int>();
            int wiresLeft = WireCount();
            int lastIndex = CableCounts.Length - 1;
            while( wiresLeft > 0)
            {
                for (int i = lastIndex; i >= 0; i--, lastIndex--)
                {
                    if (CableCounts[i] <= wiresLeft)
                        break;
                }
                newCables.Add(lastIndex);
                wiresLeft -= CableCounts[lastIndex];
            }
            Cables = newCables;
        }

        public Edge(GraphVertex end1, GraphVertex end2, double cost)
        {
            if (CableCosts.Length != CableCounts.Length)
                throw new Exception("Długości tablic z kablami nie są takie same");
            Cables = new List<int>();   
            this.End1 = end1;
            this.End2 = end2;
            this.DiggingCost = cost;
        }

        // porównujemy krawędzie za pomocą ich wag
        public int CompareTo(Edge other)
        {
            if (DiggingCost <= other.DiggingCost) return -1;
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

        protected virtual void OnCostChanged(EdgeChangedEventArgs e)
        {
            CostChanged?.Invoke(this, e);
        }
    }
}
