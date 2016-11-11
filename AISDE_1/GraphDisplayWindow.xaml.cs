using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AISDE_1
{
    /// <summary>
    /// Interaction logic for GraphDisplayWindow.xaml
    /// </summary>
    public partial class GraphDisplayWindow : Window
    {
        public Graph graph { get; set; }
        public Dictionary<GraphVertex, Ellipse> VertexToShape { get; set; } // mapuje wierzchołki do kółek na ekranie
        public Dictionary<Ellipse, GraphVertex> ShapeToVertex { get; set; } // robi na odwrót niż powyżej.
        public Dictionary<Edge, Line> EdgeToShape { get; set; } // mapuje krawędzie do kresek na ekranie
        public Dictionary<Line, Edge> ShapeToEdge { get; set; } // robi na odwrót niż powyżej       

        public Rectangle RectangleDisplayed { get; set; } // etykieta wierzchołka, która aktualnie jest wyświetlana
        public TextBlock TextBlockDisplayed { get; set; } // tekst w powyższej etykiecie

        public GraphVertex StartVertex { get; set; }
        public GraphVertex EndVertex { get; set; }

        private bool dragged;

        // uchwyt do elipsy, która jest przeciągana po ekranie.
        private Ellipse EllipseMoved;

        public GraphDisplayWindow()
        {
            InitializeComponent();
            VertexToShape = new Dictionary<GraphVertex, Ellipse>();
            ShapeToVertex = new Dictionary<Ellipse, GraphVertex>();
            EdgeToShape = new Dictionary<Edge, Line>();
            ShapeToEdge = new Dictionary<Line, Edge>();
            RectangleDisplayed = null;
            StartVertex = null;
            EndVertex = null;
        }

        public void DisplayGraph()
        {
            SetVertexDisplayCoordinates(graph, 1200, 768);

            foreach (var vertex in graph.Vertices)
            {
                foreach (var neighbor in vertex.GetNeighbors())
                {
                    Edge toDraw = vertex.GetEdge(neighbor);
                    var edge = new Line();
                    edge.MouseEnter += shape_MouseEnter;
                    edge.MouseLeave += shape_MouseLeave;
                    edge.MouseRightButtonDown += edge_MouseRightDown;

                    edge.Stroke = Brushes.Black;

                    edge.X1 = vertex.Coordinates.X + 6.25; // +5 dla promienia wierzchołka
                    edge.Y1 = vertex.Coordinates.Y + 6.25;

                    edge.X2 = neighbor.Coordinates.X + 6.25;
                    edge.Y2 = neighbor.Coordinates.Y + 6.25;

                    edge.StrokeThickness = (toDraw.IsUndirected() ? 5.0 : 2.5); // jeżeli krawędź nieskierowana, to pomaluj ją grubiej

                    canvas.Children.Add(edge);
                    EdgeToShape.Add(toDraw, edge);
                    ShapeToEdge.Add(edge, toDraw);
                }
            }

            foreach (var v in graph.Vertices)
            {
                var ellipse = new Ellipse();

                ellipse.MouseLeftButtonDown += ellipse_MouseDown;
                ellipse.MouseEnter += shape_MouseEnter;
                ellipse.MouseLeave += shape_MouseLeave;
                ellipse.MouseRightButtonDown += ellipse_MouseRightDown;

                ellipse.Stroke = System.Windows.Media.Brushes.Black;
                ellipse.StrokeThickness = 1;
                ellipse.Fill = System.Windows.Media.Brushes.DarkBlue;
                ellipse.HorizontalAlignment = HorizontalAlignment.Left;
                ellipse.VerticalAlignment = VerticalAlignment.Top;

                ellipse.Width = 20;
                ellipse.Height = 20;

                Canvas.SetLeft(ellipse, v.Coordinates.X);
                Canvas.SetTop(ellipse, v.Coordinates.Y);

                canvas.Children.Add(ellipse);
                VertexToShape.Add(v, ellipse);
                ShapeToVertex.Add(ellipse, v);
            }

            /// potrzebne do obsługi łącz nieskierowanych; jeżeli łącze jest nieskierowane to ma większą nieprzezroczystość;
            /// dzięki temu kolorowanie ścieżek jest widoczne zawsze. Gdyby tego nie było, to pokolorowana ścieżka 1=>2 mogłaby
            /// się schować pod niekolorowaną ścieżką 2=>1.
            foreach (var edge in EdgeToShape.Keys)
            {
                if (edge.End2.HasEdgeTo(edge.End1))
                    EdgeToShape[edge].Opacity = 0.5;
            }
        }

        /// <summary>
        /// Wyświetla okienko zmiany wagi danej krawędzi po kliknięciu prawym na tę krawędź.
        /// </summary>
        private void edge_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            var line = (Line)sender;
            var edge = ShapeToEdge[line];

            if (edge.IsUndirected())
            {
                var window = new UndirectedEdgePropertiesWindow(edge, edge.End2.GetEdge(edge.End1));
                window.Show();
            }
            else
            {
                var window = new DirectedEdgePropertiesWindow(edge);
                window.Show();
            }
        }

        /// <summary>
        /// Maluje daną elipsę na kolor startu albo końca ścieżki.
        /// </summary>
        private void ellipse_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            var shape = (Ellipse)sender;
            var vertex = ShapeToVertex[shape];
            if (StartVertex == null)
            {
                StartVertex = vertex;
                shape.Fill = Brushes.OrangeRed;
            }
            else if (StartVertex != null && EndVertex == null)
            {
                EndVertex = vertex;
                shape.Fill = Brushes.Olive;
            }
            else if (StartVertex != null && EndVertex != null)
            {
                VertexToShape[EndVertex].Fill = Brushes.DarkBlue;
                EndVertex = null;
                VertexToShape[StartVertex].Fill = Brushes.DarkBlue;
                StartVertex = vertex;
                shape.Fill = Brushes.OrangeRed;
            }
        }

        /// <summary>
        /// Jeżeli kursor znika z wierzchołka/krawędzi, to przestań wyświetlać etykietę
        /// </summary>
        private void shape_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas.Children.Remove(RectangleDisplayed);
            canvas.Children.Remove(TextBlockDisplayed);
            RectangleDisplayed = null;
            TextBlockDisplayed = null;
        }

        /// <summary>
        /// Wyświetla etykietę wierzchołka/krawędzi, jeżeli na tymże jest kursor.
        /// </summary>
        private void shape_MouseEnter(object sender, MouseEventArgs e)
        {
            if (RectangleDisplayed != null)
                return;

            GraphVertex vertex = null;
            Edge line = null;

            // sprawdza na co użytkownik najechał myszką
            if (sender.GetType() == typeof(Ellipse))
            {
                var shape = (Ellipse)sender;
                vertex = ShapeToVertex[shape];
            }

            else
            {
                var shape = (Line)sender;
                line = ShapeToEdge[shape];
            }

            var pos = Mouse.GetPosition(this);

            Rectangle rect = new Rectangle();
            RectangleDisplayed = rect;

            Canvas.SetLeft(rect, pos.X);
            Canvas.SetTop(rect, pos.Y - 50);
            rect.Fill = Brushes.LightYellow;
            rect.StrokeThickness = 1;
            rect.Stroke = Brushes.Black;
            rect.Width = line != null ?
                    (line.IsUndirected() ?
                        (line.Cost > line.End2.GetEdge(line.End1).Cost ? 100 + line.Cost.ToString().Length * 6.5
                : line.End2.GetEdge(line.End1).Cost.ToString().Length * 6.5 + 100) : 100 + line.Cost.ToString().Length * 6.5) : 40; //wylicza długość etykiety

            rect.Height = (sender.GetType() == typeof(Line) && line.IsUndirected()) ? 34 : 17; // dla nieskierowanego łącza potrzeba wyższej etykiety

            TextBlock text = new TextBlock();
            TextBlockDisplayed = text;

            if (vertex != null)
                text.Text = "ID: " + vertex.ID.ToString();
            else
            {
                text.Text = "V1: " + line.End1.ID.ToString() + ", V2: " + line.End2.ID.ToString() + ", c = " + line.Cost;
                if (line.IsUndirected())
                    text.Text += "\nV2: " + line.End2.ID.ToString() + ", V1: " + line.End1.ID.ToString() + ", c = " + line.End2.GetEdge(line.End1).Cost;
            }
            Canvas.SetLeft(text, pos.X + 2);
            Canvas.SetTop(text, pos.Y - 50);

            canvas.Children.Add(rect);
            canvas.Children.Add(text);
        }

        /// <summary>
        /// Koloruje daną ścieżkę na ekranie na czerwono - zielono.
        /// </summary>
        public void ColorPath(GraphPath path)
        {
            path.FindAll(v => v != StartVertex && v != EndVertex). // zostaw wierzchołki początkowe i końcowe ścieżki, żeby je odróżnić od pośrednich
                ForEach(v => ((Ellipse)VertexToShape[v]).Fill = System.Windows.Media.Brushes.Red);
            path.GetEdges().ForEach(e => EdgeToShape[e].Stroke = Brushes.DarkTurquoise);
        }

        /// <summary>
        /// W przypadku kliknięcia na elipsę ustawia tą elipsę jako obiekt przeciągany.
        /// </summary>
        private void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EllipseMoved = sender as Ellipse;
                dragged = true;
            }
        }

        /// <summary>
        /// Obsługuje przeciąganie wierzchołków.
        /// </summary>
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragged && Mouse.LeftButton == MouseButtonState.Pressed && EllipseMoved != null)
            {
                var pos = Mouse.GetPosition(this);
                Canvas.SetLeft(EllipseMoved, pos.X - 10);
                Canvas.SetTop(EllipseMoved, pos.Y - 10); // - 10 żeby kursor łapał na środku okręgu a nie na lewym górnym czubku

                var lines = GetLinesOutgoingOfShape(EllipseMoved);
                foreach (var line in lines)
                {
                    line.X1 = pos.X;
                    line.Y1 = pos.Y;
                }
                lines = GetLinesIngoingToShape(EllipseMoved);
                foreach (var line in lines)
                {
                    line.X2 = pos.X;
                    line.Y2 = pos.Y;
                }
            }
        }

        /// <summary>
        /// Zwraca listę kresek na ekranie wychodzących z danej elipsy.
        /// </summary>
        private List<Line> GetLinesOutgoingOfShape(Ellipse ellipse)
        {
            List<Line> lines = new List<Line>();
            var vertex = ShapeToVertex[ellipse];

            foreach (var v in vertex.GetNeighbors())
            {
                var line = vertex.GetEdge(v);
                lines.Add(EdgeToShape[line]);
            }

            return lines;
        }

        /// <summary>
        /// Zwraca listę kresek na ekranie wchodzących do danej elipsy.
        /// </summary>
        private List<Line> GetLinesIngoingToShape(Ellipse ellipse)
        {
            List<Line> lines = new List<Line>();
            var vertex = ShapeToVertex[ellipse];

            foreach (var v in graph.Vertices)
            {
                if (v.HasEdgeTo(vertex))
                {
                    var line = v.GetEdge(vertex);
                    lines.Add(EdgeToShape[line]);
                }
            }

            return lines;
        }

        /// <summary>
        /// Handler podniesienia przycisku; zatrzymuje przeciąganie elipsy po ekranie.
        /// </summary>
        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragged = false;
            EllipseMoved = null;
        }

        /// <summary>
        /// Wylicza MST dla grafu i koloruje ścieżki i węzły należące do MST.
        /// </summary>
        private void mstButton_Click(object sender, RoutedEventArgs e)
        {
            SpanningTree mst = graph.MinimumSpanningTree();
            if (mst == null)
            {
                // jeżeli mst == null, znaczy, że nie istnieje drzewo rozpinające - funkcja MinimumSpanningTree grafu zwraca null.
                MessageBox.Show("Nie istnieje minimalne drzewo rozpinające - graf nie jest spójny.", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RestoreDefaultColorsAndStartEndVertex();
            mst.Edges.ForEach(edge => EdgeToShape[edge].Stroke = Brushes.IndianRed);
        }

        /// <summary>
        /// Koloruje ścieżkę wyznaczoną z algorytmu Dijkstry.
        /// </summary>
        private void pathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AreCorrectPathParameters()) return;
            // wymaż z grafu wcześniej pomalowane ścieżki;
            RestoreDefaultColorsAndStartEndVertex();

            GraphPath path = graph.Dijkstra(StartVertex, EndVertex);
            if (path.IsEmpty())
            {
                MessageBox.Show("Ścieżka nie istnieje.", "Brak ścieżki", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ColorPath(path);
        }

        /// <summary>
        /// Przywraca domyślne kolory grafu (niebieski, czarny).
        /// </summary>
        private void RestoreDefaultColorsAndStartEndVertex()
        {
            foreach (var line in ShapeToEdge.Keys)
                line.Stroke = Brushes.Black;
            foreach (var ellipse in ShapeToVertex.Keys)
                if (ShapeToVertex[ellipse] != StartVertex && ShapeToVertex[ellipse] != EndVertex)
                    ellipse.Fill = Brushes.DarkBlue;
        }

        /// <summary>
        /// Koloruje ścieżkę wyliczoną z algorytmu Floyda.
        /// </summary>
        private void floydPathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!graph.WasFloydCalculated)
                graph.Floyd();

            if (!AreCorrectPathParameters()) return;
            RestoreDefaultColorsAndStartEndVertex();

            GraphPath path = graph.FloydPaths[new Tuple<GraphVertex, GraphVertex>(StartVertex, EndVertex)];
            if (path.IsEmpty())
            {
                MessageBox.Show("Ścieżka nie istnieje.", "Brak ścieżki", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ColorPath(path);
        }

        /// <summary>
        /// Sprawdza, czy wierzchołki ścieżki są dobrze zaznaczone.
        /// </summary>
        private bool AreCorrectPathParameters()
        {
            // nie licz ścieżki jeżeli nie są zaznaczone dwa wierzchołki
            if (EndVertex == null || StartVertex == null)
            {
                MessageBox.Show("Wierzchołek początkowy i końcowy nie są poprawnie zaznaczone.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                foreach (var shape in ShapeToVertex.Keys)
                    shape.Fill = Brushes.DarkBlue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ustawia wagi wszystkich łączy na losową liczbę z przedziału 0-10.
        /// </summary>
        private void randomCostsButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            foreach (var v in graph.Vertices)
                foreach (var n in v.GetNeighbors())
                    v.GetEdge(n).Cost = random.Next(9) + 1;
        }

        /// <summary>
        /// Koloruje przekazane krawędzie.
        /// </summary>
        public void ColorEdges(List<Edge> edges)
        {
            foreach (var edge in edges)
            {
                if (EdgeToShape.ContainsKey(edge))
                {
                    Line line = EdgeToShape[edge];
                    line.Stroke = Brushes.Red;
                }
            }
        }

        /// <summary>
        /// Przypisuje każdemu wierzchołkowi grafu pozycję na ekranie na podstawie rozmiaru okienka
        /// oraz ilości wierzchołków w grafie. Wierzchołki są w pierwszej kolejności rozmieszczane
        /// na okręgu o promieniu równym: krótszy z wymiarów ekranu - 100 (50 px marginesu).
        /// </summary>
        /// <param name="height">Wysokość okna.</param>
        /// <param name="width">Szerokość okna.</param>
        private void SetVertexDisplayCoordinates(Graph graph, double width, double height)
        {
            /// dodaje niezbędne marginesy
            height = height - 100;
            width = width - 100;

            Point[] coordinates = new Point[graph.Vertices.Count];
            for (int i = 0; i < graph.Vertices.Count; i++)
                coordinates[i] = new Point();

            Point center = new Point(height / 2, width / 2);
            double radius;
            if (height < width)
                radius = height;
            else radius = width;
            radius /= 2;


            double rotationAngle = 2 * Math.PI / graph.Vertices.Count; // wylicza kąt obrotu na okręgu;  

            //kalkuluje współrzędne wszystkich punktów 
            for (int i = 0; i < graph.Vertices.Count; i++)
            {
                coordinates[i].Y = center.X + radius * Math.Cos(i * rotationAngle);
                coordinates[i].X = center.Y + radius * Math.Sin(i * rotationAngle);
            }
            for (int i = 0; i < graph.Vertices.Count; i++)
            {
                if (graph.Vertices[i].Coordinates.X != 0 || graph.Vertices[i].Coordinates.Y != 0)
                    continue; //jeżeli wcześniej przypisano wierzchołkom współrzędne to nie przypisuj ich znowu
                graph.Vertices[i].Coordinates = coordinates[i];
            }
        }
    }
}

