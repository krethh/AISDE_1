using System;
using System.Collections.Generic;
using System.Windows;

namespace AISDE_1
{
    public class Graph
    {
        /// <summary>
        /// Zbiór wierzchołków należących do grafu.
        /// </summary>
        public List<GraphVertex> Vertices { get; }

        /// <summary>
        /// Zbiór ścieżek wyliczonych w algorytmie Floyda. Każda para wierzchołków jest kluczem dla najkrótszej ścieżki.
        /// </summary>
        public Dictionary<Tuple<GraphVertex, GraphVertex>, GraphPath> FloydPaths { get; set; }

        /// <summary>
        /// Mówi, czy był już wywoływany algorytm Floyda.
        /// </summary>
        public bool WasFloydCalculated { get; set; }

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

            Vertices[fromIndex].AddEdge(Vertices[toIndex], cost);
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

            PriorityQueue<GraphVertex> queue = new PriorityQueue<GraphVertex>();
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

            while (queue.Count() != 0)
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
                path.Insert(0, temp);
                temp = parentMap[temp];
            }
            path.Insert(0, startVertex);
            return path;
        }

        /// <summary>
        /// Przypisuje każdemu wierzchołkowi grafu pozycję na ekranie na podstawie rozmiaru okienka
        /// oraz ilości wierzchołków w grafie. Wierzchołki są w pierwszej kolejności rozmieszczane
        /// na okręgu o promieniu równym: krótszy z wymiarów ekranu - 100 (50 px marginesu).
        /// </summary>
        /// <param name="height">Wysokość okna.</param>
        /// <param name="width">Szerokość okna.</param>
        public void SetVertexDisplayCoordinates(int height, int width)
        {
            /// dodaje niezbędne marginesy
            height = height - 100;
            width = width - 100;

            Point[] coordinates = new Point[Vertices.Count];
            for (int i = 0; i < Vertices.Count; i++)
                coordinates[i] = new Point();

            Point center = new Point(height / 2, width / 2);
            double radius;
            if (height < width)
                radius = height;
            else radius = width;
            radius /= 2;


            double rotationAngle = 2 * Math.PI / Vertices.Count; // wylicza kąt obrotu na okręgu;  

            //kalkuluje współrzędne wszystkich punktów 
            for (int i = 0; i < Vertices.Count; i++)
            {
                coordinates[i].Y = center.X + radius * Math.Cos(i * rotationAngle);
                coordinates[i].X = center.Y + radius * Math.Sin(i * rotationAngle);
            }
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].Coordinates.X != 0 || Vertices[i].Coordinates.Y != 0) continue; //jeżeli wcześniej przypisano wierzchołkom współrzędne to nie przypisuj ich znowu
                Vertices[i].Coordinates = coordinates[i];
            }

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
                    GraphPath path = new GraphPath();
                    var throughVertex = predecessors[i, j];

                    /// brak wpisu w tablicy poprzedników może oznaczać dwie rzeczy: albo nie ma 
                    /// ścieżki, albo ścieżka jest do samego siebie
                    if (throughVertex == null)
                    {
                        FloydPaths.Add(new Tuple<GraphVertex, GraphVertex>(Vertices[i], Vertices[j]),
                            new GraphPath { TotalCost = (i == j ? 0 : Double.PositiveInfinity) }); // jeżeli szukamy ścieżki do samego siebie ustaw na 0, w.p.p nieskończoność
                        continue;
                    }
                    FloydPaths.Add(new Tuple<GraphVertex, GraphVertex>(Vertices[i], Vertices[j]), ReconstructPath(Vertices[i], Vertices[j], predecessors, new GraphPath(), true));
                }
            }
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
            PriorityQueue<Edge> possibleEdges = new PriorityQueue<Edge>(); // najbliższą możliwą ścieżkę dobieramy kolejką priorytetową

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
          
            while(!currentLine.Contains("LACZA"))
            {
                var numbers = currentLine.Split(null); // rozdziela według spacji
                graph.AddVertex(new GraphVertex { Coordinates = new Point(Double.Parse(numbers[1]), Double.Parse(numbers[2])) });

                currentLine = fileLines[++i];
            }

            while (!currentLine.Contains("LACZA"))
                currentLine = fileLines[++i];

            currentLine = fileLines[++i];

            while (!currentLine.Contains("ALGORYTM"))
            {
                var numbers = currentLine.Split(null);
                graph.AddEdge(graph.Vertices[ Int32.Parse(numbers[1]) -1], graph.Vertices[ Int32.Parse(numbers[2]) -1],  0);
                currentLine = fileLines[++i];
            }
            
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

    }
}
