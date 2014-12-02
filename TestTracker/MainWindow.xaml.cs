using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using NLog;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;
using TestTracker.Core.Utils;
using TestTracker.Controls.Messagebox;
using TestTracker.HandleEvents;
using System.Threading;

namespace TestTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region const string

        private const string STR_FAILED_TO_CONNECTION_VPN = "Failed to connect server, make sure your VPN is opended";
        private const string STR_ALL_NETWORK_LIENSE_ARE_BUSY = "All network lienses are busy, please wait for serveral minutes";
        private const string STR_WRONG_HBA_CONFIG = "Wrong HBA Config, check device configuration";
        private const string STR_WRONG_PORT = "Please check if the device on the selected port works well";
        private const string STR_UNCOMPLETED = "There is an error when trying to process, please try again";

        #endregion


        #region Events 

        private TestQueue _testQueueRunning = null;
        private TestStuff _testStuffRunning = null;
        private Logger _logger;
        private bool _isClickRunTest;
        private bool _isValidRunDMMaster;
        private bool _isRun;
        private bool _isProcessedSomeTestQueue = false;
        Stopwatch _stopwatch;
        System.Windows.Threading.DispatcherTimer _dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            BindControls();

            this.DataContext = this;
            _logger = LogManager.GetCurrentClassLogger();
            _isValidRunDMMaster = true;
            _stopwatch = new Stopwatch();
            _runTestButton.IsEnabled = false;
            SetUpTimer();
        }

        protected void ChangeFilePatch_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Applications (*.exe)|*.exe";

            // Display OpenFileDialog by calling ShowDialog method
            DialogResult result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result.ToString() == "OK")
            {
                // Open document
                string filename = dlg.FileName;
                _filePathTextBox.Text = filename;
                _isValidRunDMMaster = true;
            }
        }

        protected void ChangeFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isValidRunDMMaster = true;
        }

        protected void ChangeUlinkFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as System.Windows.Controls.TextBox;
            BindControls(textBox.Text);
        }

        protected void ChangeUlinkFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select ULINK folder in your compture";
                dlg.SelectedPath = "~/Desktop";
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _folderPathTextBox.Text = dlg.SelectedPath;
                    BindControls(_folderPathTextBox.Text);
                }
            }
        }

        protected void ChechkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(true);
        }

        protected void UnCheckedAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(false);
        }

        protected void ToogleExpand_Click(object sender, RoutedEventArgs e)
        {
            ToogleExapnd();
        }

        protected void RunTest_Click(object sender, RoutedEventArgs e)
        {
            _isClickRunTest = true;
            _isValidRunDMMaster = true;

            LoadingAdorner.IsAdornerVisible = true;
            _mainForm.IsEnabled = false;
            _dispatcherTimer.Start();

            _runTestButton.IsEnabled = false;
            _stopTestButton.IsEnabled = true;
        }

        protected void StopTest_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();

            _stopTestButton.IsEnabled = false;
            _runTestButton.IsEnabled = true;
        }

        #endregion


        #region Custom Events

        protected void Finished_Invoike(object sender, MessageArgs e)
        {
            if(e.IsSuccess)
            {
                _messageBox.ShowMessage(MessageType.Success, e.Message);
                _isValidRunDMMaster = true;
            }
            else
            {
                _messageBox.ShowMessage(MessageType.Error, e.Message);
                _isValidRunDMMaster = true;
            }
        }


        #endregion


        #region Private Metods

        private void BindControls(string ulinkFolderStr = null)
        {
            BindListFileScriptCheckbox(ulinkFolderStr);
            _isValidRunDMMaster = true;
        }

        private void BindListFileScriptCheckbox(string ulinkFolderStr)
        {
            if ( _scriptFileTreeView == null || _scriptFileTreeView.Items == null)
            {
                return;
            }

            _scriptFileTreeView.Items.Clear();
            string testScriptFolderDir;
            //Use Folder default is Desktop
            if (String.IsNullOrEmpty(ulinkFolderStr))
            {
                string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                testScriptFolderDir = pathDesktop + "\\ULINK_Protocol_v3.4_Virtium";
            }
            //Use custom folder
            else
            {
                testScriptFolderDir = ulinkFolderStr;
            }
            if(!Directory.Exists(testScriptFolderDir)) 
            {
                _scriptFileTreeView.Visibility = Visibility.Hidden;
            }
            else
            {
                _scriptFileTreeView.Visibility = Visibility.Visible;
                string[] testScriptFolderPaths = Directory.GetDirectories(testScriptFolderDir, "*", System.IO.SearchOption.AllDirectories);

                //Get file scripts in the parent directory
                var parentScripts = Directory.GetFiles(testScriptFolderDir, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".srt")).ToList();

                CreateScriptNodes(GetFileName(testScriptFolderDir), testScriptFolderDir, parentScripts);

                //Get file scripts in the child directory
                foreach(var testScriptFolderPath in testScriptFolderPaths)
                {
                    string testScriptFolder = GetFileName(testScriptFolderPath);

                    //get all file .srt
                    var childScripts = Directory.GetFiles(testScriptFolderPath, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".srt")).ToList();

                    CreateScriptNodes(testScriptFolder, testScriptFolderPath, childScripts);
                }

                //if treeview have no any file .srt, make an alert to user
                if (_scriptFileTreeView.Items.Count == 0)
                {
                    System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                    label.Content = "There is no file script";
                    _scriptFileTreeView.Items.Add(label);
                }
            }
        }

        private void CreateScriptNodes(string testScriptFolder, string testScriptFolderPath, List<string> childScripts)
        {
            TreeViewItem treeNode = new TreeViewItem();
            treeNode.Margin = new Thickness(10, 10, 10, 10);
            treeNode.Header = testScriptFolder;
            treeNode.ToolTip = testScriptFolderPath;

            if (testScriptFolder == "Main")
            {
                treeNode.IsExpanded = true;
            }

            //Add list checkbox to treeNode
            foreach (var file in childScripts)
            {
                var checkbox = new System.Windows.Controls.CheckBox();
                checkbox.Content = GetFileName(file);
                checkbox.ToolTip = file;
                checkbox.Margin = new Thickness(3, 3, 3, 3);

                string name = @"_scriptCheckbox" + checkbox.Content.ToString().Replace(".srt", "");
                checkbox.Name = name;

                treeNode.Items.Add(checkbox);
            }
            if (treeNode.Items.Count != 0)
            {
                _scriptFileTreeView.Items.Add(treeNode);
            }
        }

        private void CheckAllOrNot(bool isCheckAll)
        {
            //Find add script checkbox
            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                foreach (var checkbox in treeViewItem.Items)
                {
                    if (checkbox.GetType() == typeof(System.Windows.Controls.CheckBox))
                    {
                        var scriptCheckbox = checkbox as System.Windows.Controls.CheckBox;
                        scriptCheckbox.IsChecked = isCheckAll;
                    }
                }
            }

            if(isCheckAll)
            {
                _checkAllButton.IsEnabled = false;
                _unCheckedAllButton.IsEnabled = true;
            }
            else
            {
                _checkAllButton.IsEnabled = true;
                _unCheckedAllButton.IsEnabled = false;
            }
        }

        private void ToogleExapnd()
        {
            bool isExpanded = true;
            if (_toogleExpandButton.Content.ToString() == "Collapse")
            {
                isExpanded = false;
                _toogleExpandButton.Content = "Expand";
            }
            else
            {
                _toogleExpandButton.Content = "Collapse";
            }

            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                treeViewItem.IsExpanded = isExpanded;
            }
        }

        private string GetFileName(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("\\", filePath.Length - 1) + 1);
        }

        private List<string> GetFilecriptNames()
        {
            List<string> names = new List<string>();
            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                foreach (var checkbox in treeViewItem.Items)
                {
                    if (checkbox.GetType() == typeof(System.Windows.Controls.CheckBox))
                    {
                        var scriptCheckbox = checkbox as System.Windows.Controls.CheckBox;
                        if (scriptCheckbox.IsChecked != null)
                        {
                            if ((bool)scriptCheckbox.IsChecked)
                            {
                                names.Add(scriptCheckbox.ToolTip.ToString());
                            }
                        }
                    }
                }
            }
            return names;
        }

        private void SetUpTimer()
        {
            _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            _dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            int waitTime = 25;
            string message = string.Empty;

            if (_isClickRunTest)
            {
                CreateTestQueue();
            }

            var testQueueRepository = new TestQueueRepository();
            //if there is not completed queue, it's avalable to call Console App
            var testQueue = testQueueRepository.RetrieveTestQueueNotCompleted();
            if (testQueue != null && _isValidRunDMMaster)
            {
                _messageBox.ShowOff();

                //isRun is a flag to determine when AppConsole done (have change status), if AppConsole not done, then no call it again. 
                if (!_isRun)
                {
                    _testQueueRunning = testQueue;
                    CallConsoleApp();
                }
                //if failed to validate user input, do nothing
                if(!_isValidRunDMMaster)
                {
                    return;
                }

                _stopwatch.Start();

                //per 25 seconds, check against status of the running TestQueue
                if (_stopwatch.Elapsed.Seconds > waitTime)
                {
                    //check testqueue status is updated
                    _testQueueRunning = testQueueRepository.RetrieveTestQueue(_testQueueRunning.TestQueueId);
                    int newStatus = _testQueueRunning.TestStatusId;

                    var isStopeed = IsFailToRun(out message);
                    if (isStopeed)
                    {
                        _logger.Info(message);
                        _messageBox.ShowMessage(MessageType.Warning, message);
                        _isRun = false;
                    }
                    else if (_testQueueRunning.TestStatusId == (int)EnumTestStatus.Completed)
                    {
                        string successMessage = string.Format("Processed script {0} sucessfully", _testQueueRunning.ScriptName);
                        _logger.Info(successMessage);
                        _messageBox.ShowMessage(MessageType.Success, successMessage);
                        _isRun = false;
                    }
                    else if(_testQueueRunning.TestStatusId == (int)EnumTestStatus.Running || _testQueueRunning.TestStatusId == (int)EnumTestStatus.Processing)
                    {
                        _isRun = true;
                    }

                    //refesh grid
                    _testQueueDataGrid.DataBind();

                    //refesh stopwatch
                    _stopwatch.Stop();
                    _stopwatch = new Stopwatch();
                }
            }
            //if isRun = false, that mean test status = completed, then do refesh
            else if (_isProcessedSomeTestQueue)
            {
                //refesh grid
                string successMessage = string.Format("Processed script {0} sucessfully", _testQueueRunning.ScriptName);
                _logger.Info(successMessage);
                _messageBox.ShowMessage(MessageType.Success, successMessage);

                _testQueueDataGrid.DataBind();
                _isProcessedSomeTestQueue = false;
            }

        }

        private void CreateTestQueue()
        {
            //TODO: validation here
            List<string> scriptNames = GetFilecriptNames();
            string[] platform = ((ComboBoxItem)this._platformCombobox.SelectedItem).Tag.ToString().Split('-');
            string verdorId = platform[0];
            string deviceId = platform[1];
            string port = ((ComboBoxItem)this._port.SelectedItem).Tag.ToString();
            string computerName = System.Environment.MachineName;
            //Process Creating a test queue
            foreach (var scriptName in scriptNames)
            {
                _logger.Info(String.Format("Creating Test Queue: scriptName: {0}, verdorId: {1}, deviceId: {2}, port {3}", scriptName, verdorId, deviceId, port));
                try
                {
                    int testStuffId;
                    var testStuffRepository = new TestStuffRepository();
                    TestStuff hasAlready = testStuffRepository.Select(deviceId, verdorId, port, computerName);
                    if (hasAlready == null)
                    {
                        //create a test stuff
                        TestStuff testStuff = new TestStuff();
                        testStuff.DeviceId = deviceId;
                        testStuff.VerdorId = verdorId;
                        testStuff.Port = port;
                        testStuff.ComputerName = computerName;
                        testStuffRepository.Insert(testStuff, out testStuffId);
                    }
                    else
                    {
                        testStuffId = hasAlready.TestStuffId;
                    }
                    _logger.Info(string.Format("Created Test Stuff with Id {0}", testStuffId));

                    //create test queue
                    var testQueueRepository = new TestQueueRepository();
                    TestQueue testQueue = new TestQueue();
                    testQueue.ScriptName = scriptName;
                    testQueue.StartedTime = DateTime.UtcNow;
                    testQueue.FinishedTime = null;
                    testQueue.TestStuffId = testStuffId;
                    testQueue.TestResultId = null;

                    //if there is queue has running, then skip pending, if not then set status = running
                    var hasRunning = testQueueRepository.HasRunning();
                    if(hasRunning)
                    {
                        testQueue.TestStatusId = (int)EnumTestStatus.Pending;
                    }
                    else
                    {
                        testQueue.TestStatusId = (int)EnumTestStatus.Running;
                    }
                    testQueueRepository.InsertTestQueue(testQueue);
                }
                catch(Exception ex)
                {
                    _logger.Error("Exception when creating a test queue", ex);
                }
            }
            _testQueueDataGrid.DataBind();
            _isClickRunTest = false;
            LoadingAdorner.IsAdornerVisible = false;
            _mainForm.IsEnabled = true;
        }

        private void CallConsoleApp()
        {
            var testQueueRepository = new TestQueueRepository();

            //failed to validate Test Queue
            if (!File.Exists(_filePathTextBox.Text))
            {
                _messageBox.ShowMessage(MessageType.Warning, string.Format("File {0} is not exist", _filePathTextBox.Text));
                _isValidRunDMMaster = false;
                return;
            }

            if (!File.Exists(_testQueueRunning.ScriptName))
            {
                _messageBox.ShowMessage(MessageType.Warning, string.Format("File {0} is not exist", _testQueueRunning.ScriptName));
                _isValidRunDMMaster = false;
                return;
            }

            string shortName = _testQueueRunning.ScriptName.Substring(_testQueueRunning.ScriptName.LastIndexOf("\\", _testQueueRunning.ScriptName.Length - 1) + 1);
            _messageBox.ShowMessage(MessageType.Info, string.Format("In Process... Script Name: {0}", shortName), _testQueueRunning.ScriptName);

            _logger.Info("Begin call Console App exe");
            try
            {
                _logger.Info(String.Format("Retrieve Running Test Queue with ID: {0}", _testQueueRunning.TestQueueId));

                var testStuffRepository = new TestStuffRepository();
                _testStuffRunning = testStuffRepository.SelectByID(_testQueueRunning.TestStuffId);
                _logger.Info(String.Format("Retrieve Test Stuff with ID: {0}", _testStuffRunning.TestStuffId));

                RunTestScript();

                _isProcessedSomeTestQueue = true;
            }
            catch (Exception ex)
            {
                _logger.Error("error exception when trying to call dm master", ex);
                AssignNewStatus(EnumTestStatus.Uncompleted);
                _messageBox.ShowMessage(MessageType.Error, string.Format("Error exception when trying to execute the file path, please check your network"));
            }
        }

        private void RunTestScript()
        {
            // Run Multi script Setup the process with the ProcessStartInfo class.
            // Retrieve Queues have status = Running
            string fileDMPatch = _filePathTextBox.Text;

            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;

            string fileName = string.Empty;
            var currentDirectory  = Directory.GetCurrentDirectory();
            if(currentDirectory.Contains(@"bin\Debug"))
            {
                fileName = currentDirectory.Replace(@"\bin\Debug", @"\ConsoleApp\TestTracker.ConsoleApp.exe");
            }
            else
            {
                fileName = currentDirectory + @"\consoleapp\testtracker.consoleapp.exe";
            }
            startinfo.FileName = fileName;
            startinfo.RedirectStandardOutput = true;

            string arguments = string.Format(@"-i ""{0}"" -f ""{1}"" -s ""{2}"" -v ""{3}"" -d ""{4}"" -p ""{5}""", _testQueueRunning.TestQueueId.ToString(), fileDMPatch, _testQueueRunning.ScriptName, _testStuffRunning.VerdorId, _testStuffRunning.DeviceId, _testStuffRunning.Port);
            _logger.Info("dir path: " + fileName + " " + arguments);
            startinfo.Arguments = arguments;

            //Assign Procesing status
            AssignNewStatus(EnumTestStatus.Processing);
            _testQueueDataGrid.DataBind();

            string shortName = _testQueueRunning.ScriptName.Substring(_testQueueRunning.ScriptName.LastIndexOf("\\", _testQueueRunning.ScriptName.Length - 1) + 1);
            _messageBox.ShowMessage(MessageType.Info, string.Format("In Process... Script Name: {0}", shortName), _testQueueRunning.ScriptName);


            Process.Start(startinfo);
            _logger.Info("Call Application Console.");
            //after call console, set isRun = true
            _isRun = true;
        }

        private bool IsFailToRun(out string message)
        {
            message = string.Empty;
            bool isFailed = false;

            switch(_testQueueRunning.TestStatusId)
            {
                case (int)EnumTestStatus.FailConnection:
                    {
                        message = STR_FAILED_TO_CONNECTION_VPN;
                        isFailed = true;
                        break;
                    }
                case (int)EnumTestStatus.BusyConnection:
                    {
                        message = STR_ALL_NETWORK_LIENSE_ARE_BUSY;
                        isFailed = true;
                        break;
                    }
                case (int)EnumTestStatus.WrongHBAConfig:
                    {
                        message = STR_WRONG_HBA_CONFIG;
                        isFailed = true;
                        break;
                    }
                case (int)EnumTestStatus.WrongPort:
                    {
                        message = STR_WRONG_PORT;
                        isFailed = true;
                        break;
                    }
                case (int)EnumTestStatus.Uncompleted:
                    {
                        message = STR_UNCOMPLETED;
                        isFailed = true;
                        break;
                    }
            }
            return isFailed;
        }

        private void AssignNewStatus(EnumTestStatus newStatus)
        {
            var testQueueRepository = new TestQueueRepository();
            testQueueRepository.UpdateTestQueueStatus(_testQueueRunning.TestQueueId, newStatus);
        }

        #endregion
    }
}
