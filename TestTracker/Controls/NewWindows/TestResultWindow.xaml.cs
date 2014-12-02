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
using TestTracker.Core.Data.Repository;

namespace TestTracker.Controls.NewWindows
{
    /// <summary>
    /// Interaction logic for TestResultWindow.xaml
    /// </summary>
    public partial class TestResultWindow : UserControl
    {
        private int _testQueueId;
        public TestResultWindow(int testQueueId)
        {
            InitializeComponent();
            _testQueueId = testQueueId;
            DataBind();
        }

        private void DataBind()
        {
            //Bind Test Resuslt summary
            TestQueueRepository testQueueRepo = new TestQueueRepository();
            var testQueue = testQueueRepo.RetrieveTestQueue(_testQueueId);

            //Bind Test Result 
            TestResultRepository testResultRepo = new TestResultRepository();
            var listTestResult = testResultRepo.RetrieveTestResultByTestQueueId(_testQueueId);


            string shortName = testQueue.ScriptName.Substring(testQueue.ScriptName.LastIndexOf("\\", testQueue.ScriptName.Length - 1) + 1);
            _scriptNameLabel.Content = shortName;
            _scriptNameLabel.ToolTip = testQueue.ScriptName;

            var deviceName = listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.DeviceName)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.DeviceName)).DeviceName : "unknown";
            _deviceNameLabel.Content = deviceName;

            var serialNumber = listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.SerialNumber)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.SerialNumber)).SerialNumber : "unknown";
            _serialNumberLabel.Content = serialNumber;

            var fwRevision = listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.FWRevision)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.FWRevision)).FWRevision : "unknown";
            _FWRevisionLabel.Content = fwRevision;

            var hBAName = listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.HBAName)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.HBAName)).HBAName : "unknown";
            _hBANameLabel.Content = hBAName;

            var totalLBA = listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.TotalLBA)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.TotalLBA)).TotalLBA : "unknown";
            _totalLBALabel.Content = totalLBA;

            var capacity =  listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.Capacity)) != null ? listTestResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.Capacity)).Capacity : "unknown";
            _capacityLabel.Content = capacity;

            //Bind Test Unit Result
            TestUnitResultRepository testUnitResultRepo = new TestUnitResultRepository();
            var listTestUnitResult = testUnitResultRepo.RetrieveTestUnitResultByTestQueue(_testQueueId);

            _testUnitResultDataGrid.DataBind(listTestUnitResult);

            //Bind Test Result Document
            TestResultDocumentRepository testResultDocumentRepo = new TestResultDocumentRepository();
            var listTestResultDoc = testResultDocumentRepo.RetrieveTestResultDocumentByTestQueueId(_testQueueId);

            _documentViewerDataGrid.DataBind(listTestResultDoc);



            int passNumber = listTestUnitResult.Where(x => x.TestValue.Contains("PASS")).Count();
            int failNumber = listTestUnitResult.Where(x => x.TestValue.Contains("FAIL")).Count();
            int nANumber = listTestUnitResult.Where(x => x.TestValue.Contains("N/A")).Count();
            _totalTestResultLabel.Content = string.Format("{0} Passes / {1} Failed / {2} N/A", passNumber.ToString(), failNumber.ToString(), nANumber.ToString()); 
        }

    }
}
