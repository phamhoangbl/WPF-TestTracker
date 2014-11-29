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
using TestTracker.Core.Utils;
using TestTracker.HandleEvents;

namespace TestTracker.Controls.Grid
{
    /// <summary>
    /// Interaction logic for TestQueueDataGrid.xaml
    /// </summary>
    public partial class TestQueueDataGrid : UserControl
    {
        public event EventHandler<MessageArgs> FinishedUpdate;
        public TestQueueDataGrid()
        {
            InitializeComponent();

            DataContext = new TestQueueViewModel();
        }

        // return true if there is any Queue have Status = Stop, else return 1
        public bool DataBind()
        {
            DataContext = new TestQueueViewModel();
            var testQueues = ((DataContext) as TestQueueViewModel).TestQueues;

            _testQueueDataGrid.ItemsSource = testQueues;
            _testQueueDataGrid.Items.Refresh();

            return testQueues.SourceCollection.Cast<TestQueue>().ToList().Any(x=>x.TestStatusId == (int)EnumTestStatus.Stopped);
        }

        //Custome event Test Queue Editor
        protected void Feedback_Received(object sender, InVoke e)
        {
            try
            {
                DataBind();
                RaiseFinishedUpdate("Updated Test Queuesuccessfully.", true);
            }
            catch(Exception)
            {
                RaiseFinishedUpdate("Fails to update Test Queue successfully.", false);
            }
        }

        //Custom event Show Message box
        private void RaiseFinishedUpdate(string p, bool isSuccess)
        {
            EventHandler<MessageArgs> handler = FinishedUpdate;
            if (handler != null)
            {
                handler(null, new MessageArgs(p, isSuccess));
            }
        }

    }

}
