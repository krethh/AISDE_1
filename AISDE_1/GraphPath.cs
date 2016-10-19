using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1
{
    /// <summary>
    ///  Klasa opisująca ścieżkę w grafie. Sama wylicza swój koszt.
    /// </summary>
    public class GraphPath : List<GraphVertex>
    {
        private double _totalCost;

        /// <summary>
        /// Równy nieskończoność dla nieistniejącej ścieżki.
        /// </summary>
        public double TotalCost
        {
            get
            {
                return (_totalCost == 0 ? _totalCost = CalculateCost() : _totalCost);
            }
            set
            {
                _totalCost = value;
            }
        }

        public int CompareTo(GraphPath other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string toReturn = "";
            foreach (var v in this)
                toReturn += v.ID + ", ";

            toReturn += "\nPath cost: " + TotalCost;
            return toReturn;
        }

        public override bool Equals(object obj)
        {
            var other = obj as GraphPath;
            if (this.Count != other.Count) return false;
            if (this.TotalCost == Double.PositiveInfinity && other.TotalCost == Double.PositiveInfinity)
                return true;

            bool areEqual = true;
            for (int i = 0; i < Count; i++)
                if (this[i] != other[i])
                    areEqual = false;

            if (TotalCost != other.TotalCost) areEqual = false;

            return areEqual;         
        }

        // wylicza całkowity koszt przejścia ścieżką
        private double CalculateCost()
        {
            double cost = 0;
            try
            {
                for (int i = 0; i < Count-1; i++)
                    cost += this[i].CostToVertex(this[i + 1]);
            }
            catch (KeyNotFoundException)
            {
                // jeżeli pomiędzy wierzchołkami nie ma ścieżki, to rzuca wyjątek: ścieżka niespójna
                throw new Exception("Path not coherent.");
            }
            return cost;
        }
    }
}
