using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace AISDE_1
{
    public class Graph
    {
        public static EventHandler<EventArgs> EdgeAdded;

        /// <summary>
        /// Zbiór wierzchołków należących do grafu.
        /// </summary>
        public List<GraphVertex> Vertices { get; set; }

        /// <summary>
        /// Zbiór ścieżek wyliczonych w algorytmie Floyda. Każda para wierzchołków jest kluczem dla najkrótszej ścieżki.
        /// </summary>
        public Dictionary<Tuple<GraphVertex, GraphVertex>, GraphPath> FloydPaths { get; set; }

        /// <summary>
        /// Koszt pierwszego rozwiązania sieci, wyliczonego przed algorytmem symulowanego wyżarzania.
        /// </summary>
        public double IntialSolutionCost { get; set; } = 0;

        public double[,] FloydDistances { get; set; }

        /// <summary>
        /// Mówi, czy był już wywoływany algorytm Floyda.
        /// </summary>
        public bool WasFloydCalculated { get; set; }

        /// <summary>
        /// Wierzchołek będący centralą sieci dostępowej.
        /// </summary>
        public GraphVertex CentralVertex { get; set; } = null;

        public Graph()
        {
            Vertices = new List<GraphVertex>();
            FloydPaths = new Dictionary<Tuple<GraphVertex, GraphVertex>, GraphPath>();
            WasFloydCalculated = false;
        }

        /// <summary>
        /// Wylicza całkowitą liczbę skierowanych ścieżek w grafie (ścieżki nieskierowane są liczone podwójnie).
        /// </summary>
        public int GetNumEdges()
        {
            int numEdges = 0;
            foreach (var graphVertex in Vertices)
            {
                numEdges += graphVertex.GetNumOutgoingEdges();
            }
            return numEdges;
        }

        /// <summary>
        /// Dodaje wierzchołek do grafu oraz przypisuje mu ID.
        /// </summary>
        public bool AddVertex(GraphVertex v)
        {
            if (!Vertices.Contains(v))
            {
                Vertices.Add(v);
                if (v.ID == 0) // jeżeli już raz przypisano jakiemuś wierzchołkowi ID, to nie rób tego drugi raz (np. przy dodawaniu do drzewa)
                    v.ID = Vertices.Count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Dodaje skierowaną krawędź o podanej wadze pomiędzy dwoma wierzchołkami. Wierzchołki muszą być wcześniej dodane do grafu.
        /// </summary>
        /// <param name="end1">Wierzchołek początkowy.</param>
        /// <param name="end2">Wierzchołek końcowy.</param>
        /// <param name="cost">Waga krawędzi.</param>
        public void AddEdge(GraphVertex end1, GraphVertex end2, double cost)
        {
            if (end1 == null || end2 == null)
                throw new ArgumentException();

            if (end1 == end2)
                return;

            int fromIndex = -1, toIndex = -1;
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].Equals(end1))
                    fromIndex = i;
                if (Vertices[i].Equals(end2))
                    toIndex = i;
            }

            if (toIndex == -1 || fromIndex == -1)
                throw new ArgumentException();

            if(!end1.Neighbors.ContainsKey(end2))
                Vertices[fromIndex].AddEdge(Vertices[toIndex], cost);
            OnEdgeAdded(new EventArgs());
        }

        /// <summary>
        /// Dodaje nieskierowaną krawędź pomiędzy dwoma wierzchołkami.
        /// </summary>
        public void AddTwoWayEdge(GraphVertex end1, GraphVertex end2, double cost)
        {
            end1.AddEdge(end2, cost);
            end2.AddEdge(end1, cost);
        }

        /// <summary>
        /// Oblicza najkrótszą ścieżkę algorytmem Dijkstry. Ścieżka zawiera wierzchołek początkowy, końcowy oraz pośrednie.
        /// </summary>
        /// <param name="start">Wierzchołek początkowy.</param>
        /// <param name="goal">Wierzchołek końcowy.</param>
        /// <returns>Najkrótsza ścieżka pomiędzy dwoma wierzchołkami.</returns>
        public GraphPath Dijkstra(GraphVertex start, GraphVertex goal)
        {
            // jeżeli ścieżka jest od tego samego wierzchołka do tego samego zwróć pustą ścieżkę z kosztem zero
            if (start == goal)
                return new GraphPath { TotalCost = 0 };

            PriorityQueueHeap<GraphVertex> queue = new PriorityQueueHeap<GraphVertex>();
            HashSet<GraphVertex> visited = new HashSet<GraphVertex>();
            Dictionary<GraphVertex, GraphVertex> parentMap = new Dictionary<GraphVertex, GraphVertex>();

            int startIndex = -1, goalIndex = -1;

            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i] == start)
                    startIndex = i;
                if (Vertices[i] == goal)
                    goalIndex = i;
            }

            if (startIndex == -1 || goalIndex == -1)
                throw new ArgumentException();

            GraphVertex startVertex = Vertices[startIndex];
            GraphVertex goalVertex = Vertices[goalIndex];
            bool goalFound = false;

            Vertices.ForEach(v => v.DistanceFromStart = double.PositiveInfinity); // dla każdego wierzchołka ustawia początkową etykietę na nieskończoność.

            startVertex.DistanceFromStart = 0;
            queue.Enqueue(startVertex);

            while (queue.Count != 0)
            {
                GraphVertex current = queue.Dequeue();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    if (current == goalVertex)
                    {
                        goalFound = true;
                        break;
                    }
                    foreach (GraphVertex neighbor in current.GetNeighbors())
                    {
                        if (visited.Contains(neighbor))
                            continue;
                        if (current.DistanceFromStart + current.CostToVertex(neighbor) < neighbor.DistanceFromStart)
                        {
                            neighbor.DistanceFromStart = current.DistanceFromStart + current.CostToVertex(neighbor);
                            if (parentMap.ContainsKey(neighbor))
                            {
                                parentMap.Remove(neighbor);
                                parentMap.Add(neighbor, current);
                            }
                            else parentMap.Add(neighbor, current);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            if (!goalFound)
                return new GraphPath { TotalCost = double.PositiveInfinity }; //jeżeli ścieżka nie istnieje zwróć pustą ścieżkę z kosztem nieskończoność

            GraphPath path = new GraphPath();
            GraphVertex temp = goalVertex;

            /// odtwarza ścieżkę za pomocą parentMap
            while (!temp.Equals(startVertex))
            {
                path.Add(temp);
                temp = parentMap[temp];
            }
            path.Add(temp);
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Wylicza wszystke najkrótsze ścieżki pomiędzy wierzchołkami grafu i zapisuje je do słownika FloydPaths.
        /// </summary>
        public void Floyd()
        {
            /// przy pierwszym wywołaniu algorytmu Floyda ustawia zdarzenie, które resetuje flagę wyliczenia floyda na wypadek zmiany wagi któregoś łącza
            /// Jest to zrobione tutaj, ponieważ nie wiemy, czy będziemy w ogóle korzystać z algorytmu Floyda
            Edge.CostChanged += ResetFloydCalculatedFlag;
            FloydPaths = new Dictionary<Tuple<GraphVertex, GraphVertex>, GraphPath>();

            double[,] distances = new double[Vertices.Count, Vertices.Count];
            GraphVertex[,] predecessors = new GraphVertex[Vertices.Count, Vertices.Count];

            for (int i = 0; i < Vertices.Count; i++)
                for (int j = 0; j < Vertices.Count; j++)
                    distances[i, j] = Vertices[i].CostToVertex(Vertices[j]);

            for (int i = 0; i < Vertices.Count; i++)
                for (int j = 0; j < Vertices.Count; j++)
                    if (distances[i, j] != double.PositiveInfinity)
                        predecessors[i, j] = Vertices[i];


            for (int i = 0; i < Vertices.Count; i++)
                for (int j = 0; j < Vertices.Count; j++)
                    for (int k = 0; k < Vertices.Count; k++)
                        if (distances[j, i] + distances[i, k] < distances[j, k])
                        {
                            distances[j, k] = distances[j, i] + distances[i, k];
                            predecessors[j, k] = predecessors[i, k];
                        }

            for (int i = 0; i < Vertices.Count; i++)
            {
                var startVertex = Vertices[i];
                for (int j = 0; j < Vertices.Count; j++)
                {
                    if (i == j) // jeżeli i == j szukamy do samego siebie, dodaj pustą ścieżkę z kosztem 0
                    {
                        FloydPaths.Add(new Tuple<GraphVertex, GraphVertex>(Vertices[i], Vertices[j]), new GraphPath { TotalCost = 0 });
                        continue;
                    }

                    GraphPath path = new GraphPath();
                    var throughVertex = predecessors[i, j];

                    /// brak wpisu w tablicy poprzedników oznacza, że nie ma ścieżki
                    if (throughVertex == null)
                    {
                        FloydPaths.Add(new Tuple<GraphVertex, GraphVertex>(Vertices[i], Vertices[j]),
                            new GraphPath { TotalCost =  Double.PositiveInfinity }); // ustaw koszt na nieskończoność
                        continue;
                    }
                    FloydPaths.Add(new Tuple<GraphVertex, GraphVertex>(Vertices[i], Vertices[j]), ReconstructPath(Vertices[i], Vertices[j], predecessors, new GraphPath(), true));
                }
            }
            FloydDistances = distances;
            WasFloydCalculated = true;
        }
        
        /// <summary>
        /// Rekurencyjna procedura odtwarzająca ścieżkę na podstawie tablicy poprzedników.
        /// </summary>
        /// <param name="start">Wierzchołek początkowy ścieżki.</param>
        /// <param name="end">Wierzchołek końcowy ścieżki.</param>
        /// <param name="predecessors">Tablica poprzedników wyliczona z algorytmu Floyda.</param>
        /// <param name="alreadyDone">Ścieżka obliczona w poprzednim wywołaniu rekurencyjnym.</param>
        /// <param name="isFirstGo">Flaga mówiąca, czy funkcja wywoływana jest pierwszy raz, czy rekurencyjnie.</param>
        /// <returns>Najkrótszą ścieżkę pomiędzy wskazaną parą wierzczhołków.</returns>
        private GraphPath ReconstructPath(GraphVertex start, GraphVertex end, GraphVertex[,] predecessors, GraphPath alreadyDone, bool isFirstGo)
        {
            GraphPath path = new GraphPath();

            // żeby uniknąć podwójnego dodawania tych samych wierzchołków
            if (isFirstGo)
                path.Add(start);

            GraphVertex throughVertex = start;
            while(throughVertex != predecessors[throughVertex.ID - 1, end.ID -1 ])
            {  
                 path = path.Combine(ReconstructPath(throughVertex, predecessors[throughVertex.ID - 1, end.ID - 1], predecessors, path, false));
                 throughVertex = path[path.Count - 1];
            }

            path.Add(end);                                                
            return path;
        }

        /// <summary>
        /// Znajduje minimalne drzewo rozpinające w danym grafie za pomocą algorytmu Prima, i zwraca je jako obiekt SpanningTree.
        /// </summary>
        public SpanningTree MinimumSpanningTree()
        {
            SpanningTree tree = new SpanningTree();
            if (Vertices.Count == 0) // w przypadku pustego grafu zwróć puste drzewo
                return tree;

            tree.AddVertex(Vertices[0]);
            PriorityQueueHeap<Edge> possibleEdges = new PriorityQueueHeap<Edge>(); // najbliższą możliwą ścieżkę dobieramy kolejką priorytetową

            foreach (var v in tree.Vertices[0].GetNeighbors())
                possibleEdges.Enqueue(tree.Vertices[0].GetEdge(v));

            try
            {
                while (tree.Vertices.Count < Vertices.Count)
                {
                    var edge = possibleEdges.Dequeue();
                    if (tree.Vertices.Contains(edge.End1) && !tree.Vertices.Contains(edge.End2))
                    {
                        tree.AddVertex(edge.End2);
                        tree.Edges.Add(edge);
                        foreach (var v in edge.End2.GetNeighbors())
                            possibleEdges.Enqueue(edge.End2.GetEdge(v));
                    }
                }
            }
            catch(ArgumentOutOfRangeException) // wystąpi, jeżeli w grafie będzie istniał wierzchołek niepołączony z żadnym innym. Wtedy MST nie istnieje!
            {
                return null;
            }                
            return tree;
        }

        /// <summary>
        /// Wczytuje graf z pliku tekstowego.
        /// </summary>
        /// <param name="fileLines">Ścieżka do pliku wejściwego.</param>
        public static Graph ReadGraph(string path)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            List<string> fileLines = new List<string>();
            var line = "";
            while ((line = file.ReadLine()) != null)
                fileLines.Add(line);
            file.Close();

            fileLines = fileLines.FindAll(s => s.Length != 0).
                FindAll(s => s[0] != '#'); // wywala linie, które zawierają komentarz albo są puste

            Graph graph = new Graph();
            string currentLine = fileLines[1];
            int i = 1;

            while (!currentLine.Contains("KRAWEDZIE"))
            {
                var numbers = currentLine.Split(null);
                graph.AddVertex(new GraphVertex { ID = int.Parse(numbers[0]), Coordinates = new Point { X = double.Parse(numbers[1]), Y = double.Parse(numbers[2]) },
                    IsCentral = numbers[3] == "-1" ? true : false, ClientsNumber = numbers[3] == "-1" ? 0 : int.Parse(numbers[3]) });
                currentLine = fileLines[++i];
            }
            currentLine = fileLines[++i];

            while (!currentLine.Contains("KABLE"))
            {
                var numbers = currentLine.Split(null);
                graph.AddTwoWayEdge(graph.Vertices[int.Parse(numbers[1]) - 1], graph.Vertices[int.Parse(numbers[2]) - 1], double.Parse(numbers[3]));
                currentLine = fileLines[++i];
            }
            currentLine = fileLines[++i];

            List<int> CableCosts = new List<int>();
            List<int> CableCounts = new List<int>();
            while(i < fileLines.Count)
            {
                var numbers = currentLine.Split(null);
                CableCounts.Add(int.Parse(numbers[1]));
                CableCosts.Add(int.Parse(numbers[2]));
                if (i == fileLines.Count - 1)
                    break;
                currentLine = fileLines[++i];
            }

            Edge.CableCosts = CableCosts.ToArray();
            Edge.CableCounts = CableCounts.ToArray();

            return graph;
        }

        /// <summary>
        /// Jeżeli zostanie zmieniona waga jakiegoś łącza, to ustaw flagę "obliczono Floyda" na false,
        /// ponieważ z nowymi wagami łączy trzeba cały algorytm przeliczyć jeszcze raz.
        /// </summary>
        public void ResetFloydCalculatedFlag(object sender, EdgeChangedEventArgs e)
        {
            WasFloydCalculated = false;
        }

        /// <summary>
        /// Znajduje rozwiązanie problemu optymalizacji sieci dostępowej.
        /// </summary>
        public void GenerateSolution()
        {
            Floyd();
            if (CentralVertex == null)
                CentralVertex = Vertices.Find(v => v.IsCentral);

            GetEdges().ForEach(e => e.RemoveCables());
            foreach (var v in Vertices)
            {
                var smallest = Edge.OptimalCableSet(v.ClientsNumber);
                GraphPath pathToCentral = FloydPaths[new Tuple<GraphVertex, GraphVertex> (v, CentralVertex)];
                pathToCentral.GetEdges().ForEach(e => e.AddCables(smallest));              
            }

            GetEdges().FindAll(e => e.GetCables().Count != 0).ForEach(ec => ec.JoinCables());
        }

        /// <summary>
        /// Przeprowadza algorytm symulowanego wyżarzania.
        /// </summary>
        /// <returns>1. Liczba iteracji przeprowadzonych w algorytmie, 2. Liczba zaakceptowanych lepszych rozwiązań. </returns>
        public Tuple<int, int> SimulatedAnnealing()
        {
            Random random = new Random();
            List<Edge> edges = GetEdges();
            bool LastSolutionAccepted = false;

            /// znajduje maksymalny koszt krawędzi
            var maxCost = edges[0].DiggingCost;
            edges.ForEach(e => maxCost = e.DiggingCost > maxCost ? e.DiggingCost : maxCost);

            double temperature = 50000; // ustawiamy początkową temperaturę wyżarzania
            double coolingFactor = 0.05; // o ile będziemy ochładzać system przy każdej iteracji. Dla T = 1e7 0.00016117 da 1e5 iteracji

            Dictionary<GraphVertex, List<int>> OptimalCableSets = new Dictionary<GraphVertex, List<int>>();
            Vertices.ForEach(v => OptimalCableSets.Add(v, Edge.OptimalCableSet(v.ClientsNumber)));

            Dictionary<Edge, List<int>> CablesMap = new Dictionary<Edge, List<int>>();
            edges.ForEach(e => CablesMap.Add(e, e.GetCables()));

            List<GraphVertex> NonEmptyVertices = Vertices.FindAll(v => v.ClientsNumber != 0);

            int it = 0, accepted = 0;

            var oldNetworkCost = CalculateNetworkCost();

            Stopwatch watch = new Stopwatch();

            watch.Start();
            while (temperature > 1 && watch.ElapsedMilliseconds < 1e4)
            {
                if(LastSolutionAccepted)
                    oldNetworkCost = CalculateNetworkCost();

                /// losuje losową krawędź i przypisuje jej losową wagę
                var edge = edges[random.Next(edges.Count)];
                var oldEdgeCost = edge.DiggingCost;

                edge.DiggingCost = random.NextDouble() * 2 * maxCost; // przypisuje koszt krawędzi jako losową liczbę z przedziału (0, 2max)
                edge.End2.GetEdge(edge.End1).DiggingCost = edge.DiggingCost;

                List<Edge> modifiedEdges = new List<Edge>();

                GenerateSolution();

                edge.DiggingCost = oldEdgeCost;
                edge.End2.GetEdge(edge.End1).DiggingCost = oldEdgeCost;

                if (LastSolutionAccepted = IsAccepted(oldNetworkCost, CalculateNetworkCost(), temperature, random))
                {
                    accepted++;
                    foreach (var e in GetEdges())
                    {
                        CablesMap[e] = e.GetCables();
                    }
                }

                ///jeżeli ostatnie rozwiązanie nie zostało zaakceptowane, przywróć wcześniejsze rozwiązanie
                if (!LastSolutionAccepted)
                   foreach (var e in GetEdges())
                   {
                       e.RemoveCables();
                       e.AddCables(CablesMap[e]);
                   }

                temperature *= (1 - coolingFactor);
                it++;                
            }
            return new Tuple<int,int>(it, accepted);
        }

        /// <summary>
        /// Mówi, czy dane rozwiązanie w problemie symulowanego wyżarzania zostanie zaakceptowane.
        /// </summary>
        /// <param name="oldEnergy">Obecna energia systemu.</param>
        /// <param name="newEnergy">Energia systemu w rozwiązaniu proponowanym.</param>
        /// <param name="temperature">Temperatura systemu.</param>
        public bool IsAccepted(double oldEnergy, double newEnergy, double temperature, Random random)
        {
            if (newEnergy == oldEnergy)
                return false;

            if (newEnergy < oldEnergy)
                return true;

            else
            {
                if (Math.Exp((oldEnergy - newEnergy) / temperature) < random.NextDouble())
                    return false;
                else return true;
            }
        }

        /// <summary>
        /// Usuwa krawędź skierowaną z grafu.
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(Edge edge)
        {
            edge.End1.Neighbors.Remove(edge.End2);
        }

        /// <summary>
        /// Znajduje listę wszystkich krawędzi należących do grafu.
        /// </summary>
        /// <returns>Lista wszystkich krawędzi należących do grafu.</returns>
        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();
            foreach(var v in Vertices)
                foreach(var n in v.Neighbors.Keys)
                {
                    if (!edges.Contains(v.GetEdge(n)))
                        edges.Add(v.GetEdge(n));
                }
            return edges;
        }

        public double CalculateNetworkCost()
        {
            double cost = 0;
            GetEdges().ForEach(e => cost += e.TotalCost());
            return cost;
        }

        public void OnEdgeAdded(EventArgs e)
        {
            EdgeAdded?.Invoke(this, e);
        }

    }
}
