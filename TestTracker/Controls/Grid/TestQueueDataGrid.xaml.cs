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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestTracker.Controls.Grid
{
    /// <summary>
    /// Interaction logic for TestQueueDataGrid.xaml
    /// </summary>
    public partial class TestQueueDataGrid : UserControl
    {
        public TestQueueDataGrid()
        {
            InitializeComponent();

            DataContext = new TestQueueViewModel();
        }

        public void DataBind()
        {
            DataContext = new TestQueueViewModel();
            _testQueueDataGrid.ItemsSource = ((DataContext) as TestQueueViewModel).TestQueues;
            _testQueueDataGrid.Items.Refresh();
        }
    }
}
