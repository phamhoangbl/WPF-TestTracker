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
using TestTracker.Core.Data.Model;

namespace TestTracker.Controls.Grid
{
    /// <summary>
    /// Interaction logic for TestUnitResultDataGrid.xaml
    /// </summary>
    public partial class TestUnitResultDataGrid : UserControl
    {
        public TestUnitResultDataGrid()
        {
            InitializeComponent();
        }

        public void DataBind(List<TestUnitResult> source)
        {
            _testUnitResultDataGrid.ItemsSource = source;
            _testUnitResultDataGrid.Items.Refresh();
        }
    }
}
