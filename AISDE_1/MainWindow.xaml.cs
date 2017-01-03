using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Design;

namespace AISDE_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool EdgeAdded;

        public MainWindow()
        {
            InitializeComponent();
            EdgeAdded = false;
        }

        private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            var dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.FileName.ToString();
            }
            TextFileDirectoryTextBox.Text = path;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var graph = Graph.ReadGraph(TextFileDirectoryTextBox.Text);
                graph.GenerateStartingSolution();
                GraphDisplayWindow graphWindow = new GraphDisplayWindow { GraphToDisplay = graph };
                graphWindow.DisplayGraph();
                graphWindow.Show();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

    }
}
