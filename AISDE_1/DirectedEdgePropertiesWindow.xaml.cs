﻿using System;
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
    /// Interaction logic for EdgePropertiesWindow.xaml
    /// </summary>
    public partial class DirectedEdgePropertiesWindow : Window
    {
        public Edge EdgeBeingChanged { get; set; }

        public DirectedEdgePropertiesWindow(Edge EdgeBeingChanged)
        {
            this.EdgeBeingChanged = EdgeBeingChanged;
            InitializeComponent();
            costValueTextBox.Text = EdgeBeingChanged.DiggingCost.ToString();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            EdgeBeingChanged.DiggingCost = Double.Parse(costValueTextBox.Text);
            this.Close();
        }

        private bool isNumericalInput(System.Windows.Input.KeyEventArgs e)
        {
            if (((int)e.Key > 33 && (int)e.Key < 44))
                return true;
            return false;
        }

        private void costValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isNumericalInput(e))
                e.Handled = ((int)e.Key != 2);
        }   
    }
}
