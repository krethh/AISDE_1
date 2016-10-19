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

        public GraphDisplayWindow()
        {
            InitializeComponent();
            VertexToShape = new Dictionary<GraphVertex, Ellipse>(); 
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

                    graphGrid.Children.Add(edge);
                }

                foreach (var v in graph.Vertices)
                {
                    var ellipse = new Ellipse();

                    ellipse.Stroke = System.Windows.Media.Brushes.Black;
                    ellipse.StrokeThickness = 1;
                    ellipse.Fill = System.Windows.Media.Brushes.DarkBlue;
                    ellipse.HorizontalAlignment = HorizontalAlignment.Left;
                    ellipse.VerticalAlignment = VerticalAlignment.Top;

                    ellipse.Width = 20;
                    ellipse.Height = 20;

                    ellipse.Margin = new Thickness(v.Coordinates.X, v.Coordinates.Y, 0, 0);

                    graphGrid.Children.Add(ellipse);
                    VertexToShape.Add(v, ellipse);
                }
            }

        }

        public void ColorPathRed(GraphPath path)
        {
            path.ForEach(v => ((Ellipse)VertexToShape[v]).Stroke = System.Windows.Media.Brushes.Red);
        }
    }
    }

