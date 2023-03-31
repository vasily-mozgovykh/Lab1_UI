using Microsoft.Win32;
using SplinesDataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewData viewData = new ViewData();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewData;
            functionSelectorComboBox.ItemsSource = Enum.GetValues(typeof(FRawEnum));
        }

        private void SaveRawData(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            try
            {
                if (dialog.ShowDialog() == true)
                {
                    viewData.Save(dialog.FileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        
        private void CalculateRawDataFromFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() != true)
                    return;

                viewData.Load(dialog.FileName);
                viewData.CalculateSpline();

                FillRawDataNodesListBox();
                splineDataItemsListBox.Items.Refresh();

                integralTextBlock.Text = viewData.GetIntegral().ToString("0.000");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void CalculateRawDataFromControls(object sender, RoutedEventArgs e)
        {
            try
            {
                viewData.RawDataFromControls();
                viewData.CalculateSpline();

                FillRawDataNodesListBox();
                splineDataItemsListBox.Items.Refresh();

                integralTextBlock.Text = viewData.GetIntegral().ToString("0.000");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FillRawDataNodesListBox()
        {
            rawDataNodesListBox.Items.Clear();

            for (int i = 0; i < viewData.NodesCount; i++)
            {
                (double, double) xf = viewData.GetNodeValue(i);
                string x = xf.Item1.ToString("0.000");
                string f = xf.Item2.ToString("0.000");
                rawDataNodesListBox.Items.Add($"f({x}) = {f}");
            }

            rawDataNodesListBox.Items.Refresh();
        }
    }
}
