using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TestTracker.Core.Data.Model;

namespace TestTracker.Controls.Grid
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewerDataGrid : UserControl
    {
        private Logger _logger;
        public DocumentViewerDataGrid()
        {
            InitializeComponent();
        }

        public void DataBind(List<TestResultDocument> source)
        {
            _documentViewerDataGrid.ItemsSource = source;
            _documentViewerDataGrid.Items.Refresh();
        }

        protected void View_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var fileToOpen = button.CommandParameter.ToString();

                var process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = fileToOpen
                };

                process.Start();
                process.WaitForExit();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
