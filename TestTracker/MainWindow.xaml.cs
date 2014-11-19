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

namespace TestTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 

        private int MIN_TIMEOUT = 7; 

        #endregion

        private TestQueue _testQueueRunning = null;
        private TestStuff _testStuffRunning = null;
        private Logger _logger;
        private bool _isClickRunTest;
        private bool _isValidRunDMMaster;
        Stopwatch _stopwatch;


        public MainWindow()
        {
            InitializeComponent();
            BindControls();

            this.DataContext = this;
            _logger = LogManager.GetCurrentClassLogger();
            _isValidRunDMMaster = true;
            _stopwatch = new Stopwatch();
            SetUpTimer();
        }

        private void ChangeFilePatch_Click(object sender, RoutedEventArgs e)
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

        private void ChangeFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isValidRunDMMaster = true;
        }

        private void ChangeUlinkFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as System.Windows.Controls.TextBox;
            BindControls(textBox.Text);
        }


        private void ChangeUlinkFolder_Click(object sender, RoutedEventArgs e)
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

        private void ChechkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(true);
        }

        private void UnCheckedAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(false);
        }

        private void ToogleExpand_Click(object sender, RoutedEventArgs e)
        {
            ToogleExapnd();
        }

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            _isClickRunTest = true;
            _isValidRunDMMaster = true;

            LoadingAdorner.IsAdornerVisible = true;
            _mainForm.IsEnabled = false;
        }

        private void Datafixer_Click(object sender, RoutedEventArgs e)
        {
            var testQueueRepository = new TestQueueRepository();
            var hasRunning = testQueueRepository.HasRunning();
            var hasProcessing = testQueueRepository.SelectTestQueueProcessing();
            //if there is no queue processing, make one
            if(hasProcessing != null)
            {
                _testQueueRunning = testQueueRepository.MakeQueueRunning(hasProcessing.TestQueueId);
            }
            else
            {
                //if there is no queue running, make one
                if (!hasRunning)
                {
                    _testQueueRunning = testQueueRepository.MakeQueueRunning();
                }
            }
        }

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
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_isClickRunTest)
            {
                CreateTestQueue();
            }

            var testQueueRepository = new TestQueueRepository();
            var hasRunning = testQueueRepository.HasRunning();

            //if there is running queue, it's avalable to call Console App
            if (hasRunning && _isValidRunDMMaster)
            {
                _messageBox.ShowOff();
                CallConsoleApp();
            }

            //if there is a stopped queue, it's busy, and start back in 7 minutes
            var isStopeed = _testQueueDataGrid.DataBind();
            if(isStopeed && !_stopwatch.IsRunning)
            {
                _messageBox.ShowOff();
                _messageBox.ShowMessage(MessageType.Warning, "All network lienses are busy, please wait for serveral minutes");
                _isValidRunDMMaster = false;
                _stopwatch.Start();
            }

            if (_stopwatch.Elapsed.Seconds > 10 && _testQueueRunning != null)
            {
                _messageBox.ShowOff();
                //update against running test quese status from STOPPED to RUNNING
                _testQueueRunning = testQueueRepository.SelectQueueStopped();
                _testQueueRunning.TestStatusId = (int)EnumTestStatus.Running;
                testQueueRepository.UpdateAndSave(_testQueueRunning);

                _isValidRunDMMaster = true;
                _stopwatch.Stop();
                _stopwatch = new Stopwatch();

                string shortName = _testQueueRunning.ScriptName.Substring(_testQueueRunning.ScriptName.LastIndexOf("\\", _testQueueRunning.ScriptName.Length - 1) + 1);
                _messageBox.ShowMessage(MessageType.Info, string.Format("In Process... Script Name: {0}", shortName), _testQueueRunning.ScriptName);
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
                    testQueueRepository.Insert(testQueue);
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
            _logger.Info("Begin call Console App exe");
            try
            {
                var testQueueRepository = new TestQueueRepository();
                _testQueueRunning = testQueueRepository.SelectQueueRunning();

                _logger.Info(String.Format("Retrieve Test Queue with ID: {0}", _testQueueRunning.TestQueueId));

                if (!File.Exists(_testQueueRunning.ScriptName))
                {
                    _messageBox.ShowMessage(MessageType.Warning, string.Format("File {0} is not exist", _testQueueRunning.ScriptName));
                    _isValidRunDMMaster = false;
                    return;
                }
                if (!File.Exists(_filePathTextBox.Text))
                {
                    _messageBox.ShowMessage(MessageType.Warning, string.Format("File {0} is not exist", _filePathTextBox.Text));
                    _isValidRunDMMaster = false;
                    return;
                }
                _messageBox.ShowOff();

                var testStuffRepository = new TestStuffRepository();
                _testStuffRunning = testStuffRepository.SelectByID(_testQueueRunning.TestStuffId);
                _logger.Info(String.Format("Retrieve Test Stuff with ID: {0}", _testStuffRunning.TestStuffId));

                //Run Script to defect device
                //RunScriptDefectDevice();
                //Run Test Script User chosen
                RunTestScript();
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
                fileName = currentDirectory + @"\ConsoleApp\TestTracker.ConsoleApp.exe";
            }
            startinfo.FileName = fileName;
            startinfo.RedirectStandardOutput = true;

            string arguments = string.Format(@"-i ""{0}"" -f ""{1}"" -s ""{2}"" -v ""{3}"" -d ""{4}"" -p ""{5}""", _testQueueRunning.TestQueueId.ToString(), fileDMPatch, _testQueueRunning.ScriptName, _testStuffRunning.VerdorId, _testStuffRunning.DeviceId, _testStuffRunning.Port);
            _logger.Info("dir path: " + fileName + " " + arguments);
            startinfo.Arguments = arguments;

            //Assign Procesing status
            AssignNewStatus(EnumTestStatus.Processing);
            string shortName = _testQueueRunning.ScriptName.Substring(_testQueueRunning.ScriptName.LastIndexOf("\\", _testQueueRunning.ScriptName.Length - 1) + 1);
            _messageBox.ShowMessage(MessageType.Info, string.Format("In Process... Script Name: {0}", shortName), _testQueueRunning.ScriptName);

            using (Process process = Process.Start(startinfo))
            {
                process.Start();
                process.Kill();
            }
        }

        //private void RunScriptDefectDevice()
        //{
        //    string fileDMPatch = _filePathTextBox.Text;
        //    string fileScriptPath = @"\UlinkScripts\IdentifyDevice-V1.Log";
        //    ProcessStartInfo startinfo = new ProcessStartInfo();
        //    startinfo.UseShellExecute = false;
        //    startinfo.CreateNoWindow = true;
        //    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    startinfo.FileName = fileDMPatch;
        //    startinfo.RedirectStandardOutput = true;

        //    startinfo.Arguments = string.Format(@"/s:{0} /v:{1} /d:{2} /p:{3} /l:/e", fileScriptPath, _testStuffRunning.VerdorId, _testStuffRunning.DeviceId, _testStuffRunning.Port);

        //    using (Process process = Process.Start(startinfo))
        //    {
        //        process.Start();
        //        process.Kill();
        //    }
        //}

        private void AssignNewStatus(EnumTestStatus newStatus)
        {
            var testQueueRepository = new TestQueueRepository();
            testQueueRepository.UpdateStatus(_testQueueRunning.TestQueueId, newStatus);
        }

        #endregion
    }
}
