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
        private bool dragged;
        private Ellipse EllipseMoved;

        public GraphDisplayWindow()
        {
            InitializeComponent();
            VertexToShape = new Dictionary<GraphVertex, Ellipse>();
            ShapeToVertex = new Dictionary<Ellipse, GraphVertex>();
            EdgeToShape = new Dictionary<Edge, Line>();
        }                  

        public void DisplayGraph()
        {
            graph.SetVertexDisplayCoordinates(768, 1200);

            foreach (var vertex in graph.Vertices)
            {
                foreach (var neighbor in vertex.GetNeighbors())
                {
                    Edge toDraw = vertex.GetEdge(neighbor);
                    var edge = new Line();
                    edge.Stroke = Brushes.Black;

                    edge.X1 = vertex.Coordinates.X + 6.25; // +5 dla promienia wierzchołka
                    edge.Y1 = vertex.Coordinates.Y + 6.25;

                    edge.X2 = neighbor.Coordinates.X + 6.25;
                    edge.Y2 = neighbor.Coordinates.Y + 6.25;
                    edge.StrokeThickness = 2;

                    canvas.Children.Add(edge);
                    EdgeToShape.Add(toDraw, edge);
                }
            }

            foreach (var v in graph.Vertices)
            { 
                var ellipse = new Ellipse();

                ellipse.MouseLeftButtonDown += ellipse_MouseDown;

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
        }

        public void ColorPath(GraphPath path)
        {
            path.ForEach(v => ((Ellipse)VertexToShape[v]).Fill = System.Windows.Media.Brushes.Red);
            path.GetEdges().ForEach(e => EdgeToShape[e].Stroke = Brushes.Green);
        }

        private void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EllipseMoved = sender as Ellipse;
                dragged = true;
            }
        }

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

        private List<Line> GetLinesOutgoingOfShape(Ellipse ellipse)
        {
            List<Line> lines = new List<Line>();
            var vertex = ShapeToVertex[ellipse];

            foreach(var v in vertex.GetNeighbors())
            {
                var line = vertex.GetEdge(v);
                lines.Add(EdgeToShape[line]);
            }

            return lines;
        }

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

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragged = false;
            EllipseMoved = null;
        }
    }
}

