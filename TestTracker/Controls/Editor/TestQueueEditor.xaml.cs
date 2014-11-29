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
using TestTracker.HandleEvents;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;
using NLog;

namespace TestTracker.Controls.Editor
{
    /// <summary>
    /// Interaction logic for TestQueueEditor.xaml
    /// </summary>
    public partial class TestQueueEditor : UserControl
    {
        private Logger _logger;
        public event EventHandler<InVoke> Feedback;
        private TestQueue _testQueue;

        public static readonly DependencyProperty TestQueueIdProperty =
             DependencyProperty.Register("TestQueueId", typeof(int), typeof(TestQueueEditor));

        public int TestQueueId
        {
            get { return (int)GetValue(TestQueueIdProperty); }
            set { SetValue(TestQueueIdProperty, value); }
        }

        public TestQueueEditor()
        {
            InitializeComponent();
            _logger = LogManager.GetCurrentClassLogger();
        }
        public override void OnApplyTemplate()
        {
            var testQueueRepository = new TestQueueRepository();
            _testQueue = testQueueRepository.RetrieveTestQueue(TestQueueId);

            BindData();

        }

        private void BindData()
        {
            //retrieve Test Stuff
            var testStuffRepository = new TestStuffRepository();
            var testStuff = testStuffRepository.SelectByID(_testQueue.TestStuffId);

            string tagSelectedItem = string.Format("{0}-{1}", testStuff.VerdorId, testStuff.DeviceId);
            foreach(var item in _platformCombobox.Items.Cast<ComboBoxItem>())
            {
                if(item.Tag.ToString() == tagSelectedItem)
                {
                    item.IsSelected = true;
                }
            }
            foreach (var item in _port.Items.Cast<ComboBoxItem>())
            {
                if (item.Tag.ToString() == testStuff.Port)
                {
                    item.IsSelected = true;
                }
            }
        }
        protected void Save_Click(object sender, RoutedEventArgs e)
        {
            string[] platform = ((ComboBoxItem)this._platformCombobox.SelectedItem).Tag.ToString().Split('-');
            string verdorId = platform[0];
            string deviceId = platform[1];
            string port = ((ComboBoxItem)this._port.SelectedItem).Tag.ToString();
            string computerName = System.Environment.MachineName;

            TestStuff testStuff = new TestStuff();
            testStuff.DeviceId = deviceId;
            testStuff.VerdorId = verdorId;
            testStuff.Port = port;
            testStuff.ComputerName = computerName;

            try
            {
                int testStuffId;
                //get testStuffId if it had already, if not add new one
                var testStuffRepository = new TestStuffRepository();
                TestStuff hasAlready = testStuffRepository.Select(deviceId, verdorId, port, computerName);

                if (hasAlready != null)
                {
                    testStuffId = hasAlready.TestStuffId;
                }
                //If not, add new Test Stuff
                else
                {
                    //Add this testStuff
                    int newTestStuffId;
                    testStuffRepository.Insert(testStuff, out newTestStuffId);
                    testStuffId = newTestStuffId;
                }

                //assign this test queue to new testStuff
                var newtestQueue = _testQueue;
                newtestQueue.TestStuffId = testStuffId;
                var testQueueRepository = new TestQueueRepository();
                testQueueRepository.UpdateAndSaveTestQueue(newtestQueue);
                RaiseFeedback(true);
            }
            catch(Exception ex)
            {
                _logger.Error("There is an error when editing Test Queue", ex);
                RaiseFeedback(false);
            }

        }

        private void RaiseFeedback(bool p)
        {
            EventHandler<InVoke> handler = Feedback;
            if (handler != null)
            {
                handler(null, new InVoke(p));
            }
        }
    }
}
