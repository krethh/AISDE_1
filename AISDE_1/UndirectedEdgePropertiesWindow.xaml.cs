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
    /// Interaction logic for UndirectedEdgePropertiesWindow.xaml
    /// </summary>
    public partial class UndirectedEdgePropertiesWindow : Window
    {
        public Edge Edge1 { get; set; }
        public Edge Edge2 { get; set; }    

        public UndirectedEdgePropertiesWindow(Edge Edge1, Edge Edge2)
        {
            this.Edge1 = Edge1;
            this.Edge2 = Edge2;
            InitializeComponent();
            edge1TextBlock.Text = "V" + Edge1.End1.ID + " => " + "V" + Edge1.End2.ID;
            edge2TextBlock.Text = "V" + Edge2.End1.ID + " => " + "V" + Edge2.End2.ID;
            edge1CostValue.Text = Edge1.Cost.ToString();
            edge2CostValue.Text = Edge2.Cost.ToString();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            Edge1.Cost = double.Parse(edge1CostValue.Text);
            Edge2.Cost = double.Parse(edge2CostValue.Text);
            this.Close();
        }

        /// <summary>
        /// Sprawdza, czy wprowadzony to TextBoxa tekst jest cyfrą.
        /// </summary>
        private bool isNumericalInput(System.Windows.Input.KeyEventArgs e)
        {
            if (((int)e.Key > 33 && (int)e.Key < 44))
                return true;
            return false;
        }

        /// <summary>
        /// Jeżeli wcisnięty klawisz nie jest cyfrą, to nie pozwala wprowadzić jego wartości do textboxa.
        /// </summary>
        private void edge1CostValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isNumericalInput(e))
                e.Handled = ((int)e.Key != 2);
        }

        private void edge2CostValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isNumericalInput(e))
                e.Handled = ((int)e.Key != 2);
        }
    }
}
